using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicEventNameSpace
{
    public class MusicEvent
    {
        public float duration;
        public string name;

        public MusicEvent(float dur, string n)
        {
            duration = dur;
            name = n;
        }
    }

    public class EventEmitter
    {
        public List<MusicEvent> eventList;
        public string currentStatus;
        public List<MusicEvent> getEventList()
        {
            List<MusicEvent> list = new List<MusicEvent>();
            list.Add(new MusicEvent(5, "START"));
            list.Add(new MusicEvent(8, "A"));
            list.Add(new MusicEvent(11, "B"));
            list.Add(new MusicEvent(13, "C1"));
            list.Add(new MusicEvent(15, "C2"));
            list.Add(new MusicEvent(17, "C3"));
            list.Add(new MusicEvent(19, "D1"));
            list.Add(new MusicEvent(21, "D2"));
            list.Add(new MusicEvent(23, "D3"));
            list.Add(new MusicEvent(26, "E"));
            list.Add(new MusicEvent(30, "F"));
            list.Add(new MusicEvent(32, "D1"));
            list.Add(new MusicEvent(34, "D2"));
            list.Add(new MusicEvent(36, "D3"));
            list.Add(new MusicEvent(40, "G"));
            list.Add(new MusicEvent(45, "H"));
            list.Add(new MusicEvent(50, "I"));
            list.Add(new MusicEvent(60, "FINAL"));
            eventList = list;
            return list;
        }

        public void setCurrStatus(string name)
        {
            currentStatus = name;
        }

        public void monitorCurrentStatus()
        {
            List<MusicEvent> list = getEventList();
            while (list.Count != 0)
            {
                MusicEvent currEvent = list[0];
                list.RemoveAt(0);
                currentStatus = currEvent.name;
                while (true)
                {
                    if(Time.time > currEvent.duration)
                    {
                        break;
                    }
                }
            }
        }
    }
}

