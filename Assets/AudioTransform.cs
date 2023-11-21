using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class AudioTransform : MonoBehaviour
{
    // audio
    public AudioSource thisAudioSource;

    // spectrum data from audio, reflect to fountain
    // private List<float> spectrumData = new List<float>(fountainNum);
    private float[] spectrumData = new float[8192];
    //fountain object - use particle system
    public GameObject cubePrototype;
    // the given start point for pose the fountain
    public Transform startPoint;

    private Vector3 circleCenterMain;
    private Vector3 circleCenterSub1;
    private Vector3 circleCenterSub2;
    private Transform[] mainCircleTransform = new Transform[16];
    private float time;
    // the numbers of fountain
    public int fountainNum = 64;
    int mainCircleNum;
    int subCircleNum;
    int curveNum;
    // the scale transform of the fountain
    private List<Transform> fountainTransforms = new List<Transform>();
    private List<float> frequencyBands = new List<float>();
    private List<ParticleSystem.MainModule> velocitys = new List<ParticleSystem.MainModule>();
    private List<Vector3> fountatinPosition = new List<Vector3>();
    // Start is called before the first frame update
    public Material trailMaterial;
    public Mesh[] meshes;

    // particle system attribute
    public int particleCount = 100;
    public float particleSpeed = 8f;
    public float particleSize = 1.7f;
    public float particleLifetime = 1f;
    public float particleSpread = 0.2f;

    public Vector3 particlePosition = new(-20f, 0f, 0f);

    public GameObject particleSystemPrefab;
    // public GameObject particleSystemPrefabBottom;
    public GameObject bottomLight;
    void Start()
    {
        //generate cube and pose as a circle(should be replace as fountain)
        mainCircleNum = fountainNum / 4;
        subCircleNum = fountainNum - fountainNum / 2;
        curveNum = fountainNum / 4;

        float angle = 0f;
        float angleStep = 360f / (mainCircleNum);
        float mainCircleRadius = 80f;

        Vector3 mainCircleSratPos = startPoint.position;
        circleCenterMain = mainCircleSratPos;
        int count = 0;

        for (int i = 0; i < mainCircleNum; i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * mainCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * mainCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + mainCircleSratPos);
            mainCircleTransform[i] = fountain.transform;
            if (velocitys.Count < mainCircleNum) {
                velocitys.Add(fountain.main);
            } else {
                velocitys[count] = fountain.main;
            }
            if (fountainTransforms.Count < mainCircleNum) {
                fountainTransforms.Add(fountain.transform);
            } else {
                fountainTransforms[count] = fountain.transform;
            }
            angle += angleStep;
            count += 1;
        }

        angle = 75f;
        angleStep = 30f / (curveNum);
        float curveRadius = 2000f;

        Vector3 curveStartPos = new Vector3(-curveRadius - mainCircleRadius, 0, 0);
        // curveStartPos = startPoint.position;
        curveStartPos = curveStartPos + startPoint.position;

        for (int i = 0; i < curveNum; i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * curveRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * curveRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + curveStartPos);
            if (velocitys.Count < mainCircleNum+curveNum) {
                velocitys.Add(fountain.main);
            } else {
                velocitys[count] = fountain.main;
            }
            if (fountainTransforms.Count < mainCircleNum+curveNum) {
                fountainTransforms.Add(fountain.transform);
            } else {
                fountainTransforms[count] = fountain.transform;
            }
            angle += angleStep;
            count += 1;
        }

        angle = 0f;
        angleStep = 360f / (subCircleNum);
        float subCircleRadius = 50f;

        Vector3 subCircleStartPos1 = new Vector3(-mainCircleRadius - subCircleRadius * 1.5f, 0, mainCircleRadius * 2);
        subCircleStartPos1 = subCircleStartPos1 + startPoint.position;
        circleCenterSub1 = subCircleStartPos1;


        for (int i = 0; i < subCircleNum; i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * subCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * subCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + subCircleStartPos1);
            if (velocitys.Count < mainCircleNum+curveNum+subCircleNum) {
                velocitys.Add(fountain.main);
            } else {
                velocitys[count] = fountain.main;
            }
            if (fountainTransforms.Count < mainCircleNum+curveNum+subCircleNum) {
                fountainTransforms.Add(fountain.transform);
            } else {
                fountainTransforms[count] = fountain.transform;
            }
            angle += angleStep;
            count += 1;
        }

        angle = 0f;
        Vector3 subCircleStartPos2 = new Vector3(-mainCircleRadius + subCircleRadius * 1.5f, 0, -mainCircleRadius * 2);
        subCircleStartPos2 = subCircleStartPos2 + startPoint.position;
        circleCenterSub2 = subCircleStartPos2;

        for (int i = 0; i < subCircleNum; i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * subCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * subCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + subCircleStartPos2);
            if (velocitys.Count < mainCircleNum+curveNum+subCircleNum*2) {
                velocitys.Add(fountain.main);
            } else {
                velocitys[count] = fountain.main;
            }
            if (fountainTransforms.Count < mainCircleNum+curveNum+subCircleNum*2) {
                fountainTransforms.Add(fountain.transform);
            } else {
                fountainTransforms[count] = fountain.transform;
            }
            angle += angleStep;
            count += 1;
        }
        // Debug.Log("the count: " + count);

        fountainNum = count;


        // delay to play audio
        thisAudioSource.PlayDelayed(2f);
    }

    // Update is called once per frame
    void Update()
    {
        // Spectrum2Scale();
        UpdateMainCircleDirection();

    }

    void UpdateMainCircleDirection()
    {
        time += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(time * 1) * 0.2f;
        int len = mainCircleTransform.Length;
        float radius = 5f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection = (circleCenterMain - mainCircleTransform[i].position).normalized;
            Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
            Console.WriteLine(newDirection.ToString());
            mainCircleTransform[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }
    void Spectrum2Scale()
    {
        thisAudioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        // frequency bands
        int frequencyBandsNum = fountainNum/4;
        int sampleCount = 8192 / fountainNum;
        for (int i = 0; i < frequencyBandsNum; i++)
        {
            float average = 0;
            for (int j = i * sampleCount; j < (i + 1) * sampleCount; j++)
            {
                average = average + spectrumData[j]*200;
            }
            if (frequencyBands.Count < frequencyBandsNum) {
                frequencyBands.Add(average);
            } else {
                frequencyBands[i] = average;
            }

        }

        // normalize to range (0, 10)

        float min_frequency = Mathf.Min(frequencyBands.ToArray());
        float max_frequency = Mathf.Max(frequencyBands.ToArray());
        for (int i = 0; i < frequencyBandsNum; i++) {
            // if (min_frequency < 10) {
            //     frequencyBands[i] = (frequencyBands[i] - min_frequency) / (max_frequency-min_frequency) * (max_speed-min_speed) + min_frequency;
            // } else {
            //     frequencyBands[i] = (frequencyBands[i] - min_frequency) / (max_frequency-min_frequency) * (max_speed-min_speed) + 10;
            // }
            if (min_frequency < 10) {
                frequencyBands[i] = (frequencyBands[i] - min_frequency) / (max_frequency-min_frequency) * 10f + min_frequency;
            } else {
                frequencyBands[i] = (frequencyBands[i] - min_frequency) / (max_frequency-min_frequency) * 10f + 10;
            }
        }
        // Debug.Log("THE number of frequencyBands:" + frequencyBands.Count);
        // Debug.Log("the frequency: max-" + Mathf.Max(spectrumData)+ "; min-" + Mathf.Min(spectrumData));

        max_frequency = Mathf.Max(frequencyBands.ToArray());
        min_frequency = Mathf.Min(frequencyBands.ToArray());
        Debug.Log("the frequency brands: max-" + max_frequency+ "; min-" + min_frequency);

        int count = 0;
        for (int i = 0; i < mainCircleNum; i++) {
            ParticleSystem.MainModule module_v = velocitys[i];
            module_v.startSpeed = max_frequency;
            count += 1;
        }
        for (int i = 0; i < curveNum; i++)
        {   
            // velocitys[count].startSpeed = Mathf.Lerp((fountainTransforms[count].localScale.y)*0.01f, frequencyBands[i]*10f, 0.5f);
            ParticleSystem.MainModule module_v = velocitys[count];
            module_v.startSpeed = min_speed + Mathf.Lerp(fountainTransforms[count].localScale.y, frequencyBands[i], 0.5f);
            count += 1;
        }

        for (int i = 0; i < subCircleNum*2; i++)
        {   
            // can be inspired by other condition
            if (min_frequency > 2) {
                ParticleSystem.MainModule module_v = velocitys[count];
                module_v.startSpeed = min_frequency;
            } else {
                ParticleSystem.MainModule module_v = velocitys[count];
                module_v.startLifetime = 0;
                module_v.startSpeed = 0;
            }
            
            count += 1;
        }
        

    }

    ParticleSystem buildFountain(Vector3 position)
    {

        GameObject particleSystemObject = Instantiate(particleSystemPrefab);
        // GameObject particleSystemObjectBottom = Instantiate(particleSystemPrefabBottom);
        GameObject bottomLightObj = Instantiate(bottomLight);

        ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particleSystem.main;
        particleSystem.transform.rotation = Quaternion.Euler(-90, 0, 0);
        particleSystem.transform.position = position;
        particleSystem.transform.localScale = new Vector3(10, 10, 10);

        //ParticleSystem particleSystemBottom = particleSystemObjectBottom.GetComponent<ParticleSystem>();
        //particleSystemBottom.transform.position = position;
        //particleSystemBottom.transform.localScale = new Vector3(5, 5, 5);

        bottomLightObj.transform.position = position;
        // var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        //renderer.trailMaterial = trailMaterial;
        //renderer.renderMode = ParticleSystemRenderMode.Mesh;
        //renderer.SetMeshes(meshes);

        //mainModule.startSize = particleSize;
        //mainModule.startSpeed = 8f;
        //mainModule.startLifetime = 3f;
        //mainModule.startColor = Color.white;
        //mainModule.gravityModifier = 0.2f;
        //mainModule.simulationSpace = ParticleSystemSimulationSpace.World;


        //var force = new ParticleSystem.MinMaxCurve(-5f);
        //var forceOverLifetime = particleSystem.forceOverLifetime;
        //forceOverLifetime.enabled = true;
        //forceOverLifetime.space = ParticleSystemSimulationSpace.World;
        //forceOverLifetime.x = new ParticleSystem.MinMaxCurve(0f);
        //forceOverLifetime.y = force;
        //forceOverLifetime.z = new ParticleSystem.MinMaxCurve(0f);

        //var shape = particleSystem.shape;
        // shape.shapeType = ParticleSystemShapeType.Box;
        //shape.shapeType = ParticleSystemShapeType.Cone;
        //shape.radius = particleSpread;
        //shape.angle = 5f;

        var emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 50;

        //var emission2 = particleSystemBottom.emission;
        //emission2.enabled = true;
        //emission2.rateOverTime = 5;

        //var velocityOverLifetime = particleSystem.velocityOverLifetime;
        //velocityOverLifetime.enabled = true;
        //velocityOverLifetime.y = 10f;
        //velocityOverLifetime.space = ParticleSystemSimulationSpace.World;

        //// curve
        //AnimationCurve curve = new AnimationCurve();
        //curve.AddKey(0f, 1f);
        //curve.AddKey(0.5f, 0.5f);
        //curve.AddKey(1f, 0f);

        //// trails
        //ParticleSystem.TrailModule trail = particleSystem.trails;
        //trail.enabled = true;
        //trail.lifetime = new ParticleSystem.MinMaxCurve(0.25f);
        //trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, curve);

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        for (int j = 0; j < 100; j++)
        {
            particleSystem.Emit(emitParams, 1);
        }

        particleSystem.Play();
        // particleSystemBottom.Play();
        return particleSystem;
    }
}