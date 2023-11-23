using System;
using UnityEngine;

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
    // the numbers of fountain, should be 2^n between 64 to 8192
    public int fountainNum = 64;
    // the scale transform of the fountain
    private Transform[] fountainTransforms = new Transform[64];
    private float[] frequencyBands = new float[2048];
    private ParticleSystem.MainModule[] velocitys = new ParticleSystem.MainModule[64];
    private Vector3[] fountatinPosition = new Vector3[2048];

    private Vector3 circleCenterMain;
    private Vector3 circleCenterSub1;
    private Vector3 circleCenterSub2;
    private Transform[] mainCircleTransform = new Transform[16];
    private float time;
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
        int mainCircleNum = fountainNum / 4;
        int subCircleNum = fountainNum / 6;
        int curveNum = fountainNum / 4;

        float angle = 0f;
        float angleStep = 360f / (mainCircleNum);
        float mainCircleRadius = 80f;

        Vector3 mainCircleSratPos = startPoint.position;
        circleCenterMain = mainCircleSratPos;

        for (int i = 0; i < mainCircleNum; i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * mainCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * mainCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + mainCircleSratPos);
            velocitys[i] = fountain.main;
            fountainTransforms[i] = fountain.transform;
            mainCircleTransform[i] = fountain.transform;
            angle += angleStep;
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
            velocitys[i] = fountain.main;
            fountainTransforms[i] = fountain.transform;
            angle += angleStep;
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
            velocitys[i] = fountain.main;
            fountainTransforms[i] = fountain.transform;
            angle += angleStep;
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
            velocitys[i] = fountain.main;
            fountainTransforms[i] = fountain.transform;
            angle += angleStep;
        }



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
        int count = 0;
        int sampleCount = 8192 / fountainNum;
        for (int i = 0; i < fountainNum; i++)
        {
            float average = 0;
            for (int j = i * sampleCount; j < (i + 1) * sampleCount; j++)
            {
                average = average + spectrumData[count] * (count + 1);
                count = count + 1;
            }
            average = average / count;
            frequencyBands[i] = average * 10;
        }


        for (int i = 0; i < fountainNum; i++)
        {
            velocitys[i].startSpeed = Mathf.Lerp((fountainTransforms[i].localScale.y) * 0.01f, frequencyBands[i] * 100f, 0.5f);
        }
    }

    ParticleSystem buildFountain(Vector3 position)
    {

        GameObject particleSystemObject = Instantiate(particleSystemPrefab);
        GameObject bottomLightObj = Instantiate(bottomLight);

        ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particleSystem.main;
        particleSystem.transform.rotation = Quaternion.Euler(-90, 0, 0);
        particleSystem.transform.position = position;
        particleSystem.transform.localScale = new Vector3(10, 10, 10);

        bottomLightObj.transform.position = position;

        var emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 50;

        particleSystem.Play();
        return particleSystem;
    }
}