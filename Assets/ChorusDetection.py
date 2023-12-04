#!/usr/bin/env python
# -*- coding: utf-8 -*-
# @Date    : 2023-11-22 14:11:32
# @Author  : YANG Jie
# @Email   : jyangdm@connect.ust.hk

import os
import librosa
import numpy as np
import scipy.signal
from abc import ABCMeta, abstractmethod
from math import sqrt
import librosa.display 
import matplotlib.pyplot as plt
import cv2
# Denoising size in seconds
SMOOTHING_SIZE_SEC = 2.5
# Number of samples to consider in one chunk.
# Smaller values take more time, but are more accurate
N_FFT = 2**14
# For line detection
LINE_THRESHOLD = 0.15
MIN_LINES = 8
NUM_ITERATIONS = 8
# an error proportional to the length of the clip
OVERLAP_PERCENT_MARGIN = 0.2
# Beat analysis
BINS_PER_OCTAVE = 12
N_OCTAVES = 7

class SimilarityMatrix(object):
    __metaclass__ = ABCMeta

    def __init__(self, chroma, sample_rate):
        self.chroma = chroma
        # sample_rate of the audio, almost always 22050
        self.sample_rate = sample_rate  
        self.matrix = self.compute_similarity_matrix(chroma)

    @abstractmethod
    def compute_similarity_matrix(self, chroma):
        pass

    def display(self):
        librosa.display.specshow(
            self.matrix,
            y_axis='time',
            x_axis='time',
            sr=self.sample_rate / (N_FFT / 2048))
        plt.colorbar()
        plt.set_cmap("PuBu")
        # plt.xlim(0, 300)
        # plt.savefig('test1.png')
        plt.show()

    def getMatrix(self):
        return self.matrix


class TimeTimeSimilarityMatrix(SimilarityMatrix):
    def compute_similarity_matrix(self, chroma):
        # (12 x n x 1)
        broadcast_x = np.expand_dims(chroma, 2)  
        # (12 x 1 x n)
        broadcast_y = np.swapaxes(np.expand_dims(chroma, 2), 1, 2)  
        time_time_matrix = 1 - (np.linalg.norm((broadcast_x - broadcast_y), axis=0) / sqrt(12))
        return time_time_matrix


class TimeLagSimilarityMatrix(SimilarityMatrix):
    def compute_similarity_matrix(self, chroma):
        num_samples = chroma.shape[1]
        broadcast_x = np.repeat(np.expand_dims(chroma, 2), num_samples + 1, axis=2)
        circulant_y = np.tile(chroma, (1, num_samples + 1)).reshape(12, num_samples, num_samples + 1)
        time_lag_similarity = 1 - (np.linalg.norm((broadcast_x - circulant_y), axis=0) / sqrt(12))
        time_lag_similarity = np.rot90(time_lag_similarity, k=1, axes=(0, 1))
        return time_lag_similarity[:num_samples, :num_samples]

    def denoise(self, time_time_matrix, smoothing_size):
        n = self.matrix.shape[0]
        horizontal_smoothing_window = np.ones((1, smoothing_size)) / smoothing_size
        horizontal_moving_average = scipy.signal.convolve2d(self.matrix, horizontal_smoothing_window, mode="full")
        left_average = horizontal_moving_average[:, 0:n]
        right_average = horizontal_moving_average[:, smoothing_size - 1:]
        max_horizontal_average = np.maximum(left_average, right_average)

        # Get the vertical strength at every sample
        vertical_smoothing_window = np.ones((smoothing_size, 1)) / smoothing_size
        vertical_moving_average = scipy.signal.convolve2d(self.matrix, vertical_smoothing_window, mode="full")
        down_average = vertical_moving_average[0:n, :]
        up_average = vertical_moving_average[smoothing_size - 1:, :]

        # Get the diagonal strength of every sample from the time_time_matrix.
        # The key insight is that diagonal averages in the time lag matrix are horizontal
        # lines in the time time matrix
        diagonal_moving_average = scipy.signal.convolve2d(time_time_matrix, horizontal_smoothing_window, mode="full")
        ur_average = np.zeros((n, n))
        ll_average = np.zeros((n, n))
        for x in range(n):
            for y in range(x):
                ll_average[y, x] = diagonal_moving_average[x - y, x]
                ur_average[y, x] = diagonal_moving_average[x - y, x + smoothing_size - 1]

        non_horizontal_max = np.maximum.reduce([down_average, up_average, ll_average, ur_average])
        non_horizontal_min = np.minimum.reduce([up_average, down_average, ll_average, ur_average])

        # If the horizontal score is stronger than the vertical score, it is considered part of a line
        # and we only subtract the minimum average. Otherwise subtract the maximum average
        suppression = (max_horizontal_average > non_horizontal_max) * non_horizontal_min + (
            max_horizontal_average <= non_horizontal_max) * non_horizontal_max

        # Filter it horizontally to remove any holes, and ignore values less than 0
        denoised_matrix = scipy.ndimage.gaussian_filter1d(np.triu(self.matrix - suppression), smoothing_size, axis=1)
        denoised_matrix = np.maximum(denoised_matrix, 0)
        denoised_matrix[0:5, :] = 0

        self.matrix = denoised_matrix


class Line(object):
    def __init__(self, start, end, lag):
        self.start = start
        self.end = end
        self.lag = lag

    def __repr__(self):
        return "Line ({} {} {})".format(self.start, self.end, self.lag)

def local_maxima_rows(denoised_time_lag):
    row_sums = np.sum(denoised_time_lag, axis=1)
    # print(row_sums)
    divisor = np.arange(row_sums.shape[0], 0, -1)
    # print(divisor)
    normalized_rows = row_sums / divisor
    # print(normalized_rows)
    local_maxima_rows = scipy.signal.argrelextrema(normalized_rows, np.greater)
    # print(local_maxima_rows[0])
    return local_maxima_rows[0]


def detect_lines(denoised_time_lag, rows, min_length_samples):
    cur_threshold = LINE_THRESHOLD
    for _ in range(NUM_ITERATIONS):
        line_segments = detect_lines_helper(denoised_time_lag, rows,cur_threshold, min_length_samples)
        if len(line_segments) >= MIN_LINES:
            return line_segments
        cur_threshold *= 0.95
    return line_segments


def detect_lines_helper(denoised_time_lag, rows, threshold, min_length_samples):
    num_samples = denoised_time_lag.shape[0]
    line_segments = []
    cur_segment_start = None
    for row in rows:
        if row < min_length_samples:
            continue
        for col in range(row, num_samples):
            if denoised_time_lag[row, col] > threshold:
                if cur_segment_start is None:
                    cur_segment_start = col
            else:
                if (cur_segment_start is not None) and (col - cur_segment_start) > min_length_samples:
                    line_segments.append(Line(cur_segment_start, col, row))
                cur_segment_start = None
    return line_segments


def count_overlapping_lines(lines, margin, min_length_samples):
    line_scores = {}
    for line in lines:
        line_scores[line] = 0

    # Iterate over all pairs of lines
    for line_1 in lines:
        for line_2 in lines:
            # If line_2 completely covers line_1 (with some margin), line_1 gets a point
            lines_overlap_vertically = (line_2.start < (line_1.start + margin)) and (line_2.end > (line_1.end - margin)) and (abs(line_2.lag - line_1.lag) > min_length_samples)

            lines_overlap_diagonally = ((line_2.start - line_2.lag) < (line_1.start - line_1.lag + margin)) and (
                    (line_2.end - line_2.lag) > (line_1.end - line_1.lag - margin)) and (abs(line_2.lag - line_1.lag) > min_length_samples)

            if lines_overlap_vertically or lines_overlap_diagonally:
                line_scores[line_1] += 1

    return line_scores


def best_segment(line_scores):
    lines_to_sort = []
    for line in line_scores:
        lines_to_sort.append((line, line_scores[line], line.end - line.start))

    lines_to_sort.sort(key=lambda x: (x[1], x[2]))
    best_tuple = lines_to_sort[0]
    return best_tuple[0], lines_to_sort


def draw_lines(num_samples, sample_rate, lines):
    lines_matrix = np.zeros((num_samples, num_samples))
    for line in lines:
        lines_matrix[line.lag:line.lag + 4, line.start:line.end + 1] = 1

    librosa.display.specshow(
        lines_matrix,
        y_axis='time',
        x_axis='time',
        sr=sample_rate / (N_FFT / 2048))
    plt.colorbar()
    plt.set_cmap("PuBu")
    plt.show()


def create_chroma(input_file, n_fft=N_FFT):
    y, sr = librosa.load(input_file)
    song_length_sec = y.shape[0] / float(sr)
    S = np.abs(librosa.stft(y, n_fft=n_fft))**2
    chroma = librosa.feature.chroma_stft(S=S, sr=sr)

    return chroma, sr, song_length_sec


def find_chorus(chroma, sr, song_length_sec, clip_length):
    num_samples = chroma.shape[1]

    time_time_similarity = TimeTimeSimilarityMatrix(chroma, sr)
    time_lag_similarity = TimeLagSimilarityMatrix(chroma, sr)

    # Denoise the time lag matrix
    chroma_sr = num_samples / song_length_sec
    # print(chroma_sr, num_samples, song_length_sec)
    smoothing_size_samples = int(SMOOTHING_SIZE_SEC * chroma_sr)
    # time_lag_similarity.display()
    time_lag_similarity.denoise(time_time_similarity.getMatrix(), smoothing_size_samples)
    # time_lag_similarity.display()
    # time_lag_image = cv2.imread('test1.png')
    # edges = cv2.Canny(time_lag_image, 50, 150)
    # lines = cv2.HoughLinesP(edges, rho=1, theta=np.pi/180, threshold=50, minLineLength=50, maxLineGap=10)
    # for line in lines:
    #     x1, y1, x2, y2 = line[0]
    #     cv2.line(time_lag_image, (x1, y1), (x2, y2), (255, 255, 255))
    # cv2.imshow("line", time_lag_similarity.getMatrix())
    # cv2.waitKey(0)
    # cv2.destroyAllWindows()

    # Detect lines in the image
    clip_length_samples = clip_length * chroma_sr
    candidate_rows = local_maxima_rows(time_lag_similarity.getMatrix())
    # print(candidate_rows)
    lines = detect_lines(time_lag_similarity.getMatrix(), candidate_rows, clip_length_samples)
    if len(lines) == 0:
        print("No choruses were detected. Try a smaller search duration")
        return None
    # print(lines)
    lines.sort(key=lambda x: (x.start, x.lag))
    # print(lines)
    chorus_start = []
    for line in lines:
        chorus_start.append(line.start / chroma_sr)
    return chorus_start

if __name__ == '__main__':
    file_path = os.path.join(os.getcwd(),'Seventeen-Very Nice.mp3')

    # chroma, _, sr, _ = create_chroma(file_path)
    # time_time_similarity = TimeTimeSimilarityMatrix(chroma, sr)
    # time_time_similarity.display()
    # time_lag_similarity = TimeLagSimilarityMatrix(chroma, sr)
    # time_lag_similarity.display()
    # draw_lines(chroma.shape[1],sr,lines)

    # print(time_lag_similarity.getMatrix().shape)
    
    chroma, sr, song_length_sec = create_chroma(file_path)
    clip_length = 10
    chorus_start = find_chorus(chroma, sr, song_length_sec, clip_length)
    if chorus_start != None:
        for chorus in chorus_start:
            print("Chorus start at {0:g} min {1:.2f} sec and last for {1:.2f} sec".format(chorus // 60, chorus % 60, clip_length))

    # lines = []
    # for line in lines_to_sort:
    #     lines.append(line[0])
    # print(lines)

    # for line in lines_to_sort:
    #     print("at {0:g} min {1:.2f} sec".format(line // 60, line % 60))

    # y, sr = librosa.load(file_path)
    # C = librosa.amplitude_to_db(np.abs(librosa.cqt(y=y, sr=sr,bins_per_octave=BINS_PER_OCTAVE,n_bins=N_OCTAVES * BINS_PER_OCTAVE)),ref=np.max)

    # tempo, beats = librosa.beat.beat_track(y=y, sr=sr, trim=False)
    # print(beats)
    # for i in range(1, len(beats)):
    #     beats[len(beats)-i] = beats[len(beats)-i] - beats[len(beats)-i-1]

    # print(beats)
    # mfcc = librosa.feature.mfcc(y=y, sr=sr)
    # print(mfcc)
    # Msync = librosa.util.sync(mfcc, beats)
    # 

    # Csync = librosa.util.sync(C, beats, aggregate=np.median)
    # 












