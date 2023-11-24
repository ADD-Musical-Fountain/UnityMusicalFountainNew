using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;
using MusicEventNameSpace;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class AudioTransform : MonoBehaviour
{
    // audio
    public AudioSource thisAudioSource;
    public GameObject cubePrototype;
    public Transform startPoint;

    private Vector3 circleCenterMain;
    private Vector3 circleCenterSub1;
    private Vector3 circleCenterSub2;
    private List<Transform> mainCircleTransform = new List<Transform>();
    private List<Transform> subCircleTransform1 = new List<Transform>();
    private List<Transform> subCircleTransform2 = new List<Transform>();
    private List<ParticleSystem> mainCircleFounatin = new List<ParticleSystem>();
    private List<ParticleSystem> innerMainCircleFounatin = new List<ParticleSystem>();
    private List<ParticleSystem> subCircleFountain1 = new List<ParticleSystem>();
    private List<ParticleSystem> subCircleFountain2 = new List<ParticleSystem>();
    private List<ParticleSystem> curveFountain = new List<ParticleSystem>();
    private List<ParticleSystem> curveFountain2 = new List<ParticleSystem>();
    private List<ParticleSystem.MainModule> curveMainList = new List<ParticleSystem.MainModule>();
    private float mainTimer;
    private float subTimer1;
    private float subTimer2;
    private float curveTimer;

    public int fountainNum = 64;
    int mainCircleNum;
    int subCircleNum;
    int curveNum;

    private List<float> frequencyBands = new List<float>();
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

    private EventEmitter emitter;
    private List<ParticleSystem> ADDList = new List<ParticleSystem>();

    void Start()
    {
        //generate cube and pose as a circle(should be replace as fountain)
        mainCircleNum = 32;
        subCircleNum = 32;
        curveNum = 48;

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
            mainCircleTransform.Add(fountain.transform);
            mainCircleFounatin.Add(fountain);
            angle += angleStep;
        }

        angle = 0f;
        angleStep = 360f / (mainCircleNum);
        mainCircleRadius = 70f;

        for (int i = 0; i < mainCircleNum; i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * mainCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * mainCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + mainCircleSratPos);
            innerMainCircleFounatin.Add(fountain);
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
            curveMainList.Add(fountain.main);
            curveFountain.Add(fountain);
            angle += angleStep;
        }

        angle = 75f;
        angleStep = 30f / (curveNum);
        for (int i = 0; i < curveNum; i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * curveRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * curveRadius;
            Vector3 posi = new Vector3(x - 5, 0, z - 5);
            ParticleSystem fountain = buildFountain(posi + curveStartPos, 2);
            curveFountain2.Add(fountain);
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
            subCircleTransform1.Add(fountain.transform);
            subCircleFountain1.Add(fountain);
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
            subCircleTransform2.Add(fountain.transform);
            subCircleFountain2.Add(fountain);
            angle += angleStep;
        }
        // Debug.Log("the count: " + count);

        // delay to play audio
        thisAudioSource.PlayDelayed(2f);

        // initial emitter
        emitter = new EventEmitter();
        emitter.getEventList();

        finalPose();
    }

    // Update is called once per frame
    void Update()
    {
        processEmitter();
        Debug.Log(Time.time);
    }

    public void finalPose()
    {
        stopMainCircle(true, true);
        stopSubCircle(true, true);
        stopCurve1();
        stopCurve2();
        ParticleSystem A1 = buildFountain(new Vector3(0, 0, 150), 15, 400);
        ParticleSystem A2 = buildFountain(new Vector3(0, 0, 50), 15, 400);
        ParticleSystem A3 = buildFountain(new Vector3(0, 130, 175), 8, 400);
        A1.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, -0.1f));
        A2.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, 0.1f));
        A3.transform.rotation = Quaternion.LookRotation(Vector3.up * 2 + new Vector3(0, 0, -2f));

        ParticleSystem D1 = buildFountain(new Vector3(0, 0, 0), 15, 400);
        ParticleSystem D2 = buildFountain(new Vector3(0, 200, 0), 5, 400, 4);
        D2.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, -0.8f));

        ParticleSystem D4 = buildFountain(new Vector3(0, 0, -100), 15, 400);
        ParticleSystem D5 = buildFountain(new Vector3(0, 200, -100), 5, 400, 4);
        D5.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, -0.8f));

        ADDList.Add(A1);
        ADDList.Add(A2);
        ADDList.Add(A3);
        ADDList.Add(D1);
        ADDList.Add(D2);
        ADDList.Add(D4);
        ADDList.Add(D5);
        stopADD();
    }

    private void activateFinalPose()
    {
        int len = ADDList.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem particle = ADDList[i];
            ParticleSystem.EmissionModule emission = particle.emission;
            ParticleSystem.MainModule main = particle.main;
            emission.enabled = true;
        }
    }

    public void processEmitter()
    {
        if (emitter.eventList.Count == 0)
        {
            return;
        }
        if (Time.time < emitter.eventList[0].duration)
        {
            if (emitter.eventList[0].name != emitter.currentStatus)
            {
                emitter.setCurrStatus(emitter.eventList[0].name);
            }
        }
        else
        {
            emitter.eventList.RemoveAt(0);
        }
        switch (emitter.currentStatus)
        {
            case "A":
                shakeCurveLeftRightBy2(2, 2);
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "B":
                shakeCurveLeftRightBy2(2, 2, false);
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "C1":
                startEmit(true, true, false, false, false, false);
                crossCurveLeftRightBy2(5, 2, 0);
                setCurve2Velocity(8);
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "C2":
                startEmit(true, true, false, false, false, false);
                crossCurveLeftRightBy2(5, 2, 1);
                setCurve2Velocity(8);
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "C3":
                startEmit(true, true, false, false, false, false);
                crossCurveLeftRightBy2(5, 2, 2);
                setCurve2Velocity(8);
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "D1":
                shakeCurveLeftRightBy4(3, 2, 2);
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "D2":
                shakeCurveLeftRightBy4(3, 2, 1);
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "D3":
                shakeCurveLeftRightBy4(3, 2, 0);
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "E":
                setDirectionUpCurve1();
                fixedCurveBySin();
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "F":
                setCurve1Velocity(12);
                shakeCurveLeftRightCross(2, 2);
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "G":
                setMainCircleVelocity(10);
                mainCircleInclineDynamic();
                spiltCurveLeftRight(2, 2);
                stopCurve2();
                stopSubCircle(true, true);
                stopADD();
                break;
            case "H":
                mainCircleInclineTangent();
                setSubCircleVelocity1(12);
                setSubCircleVelocity2(12);
                stopCurve2();
                stopCurve1();
                stopADD();
                break;
            case "I":
                mainCircleIncline();
                innerMainCircleSin();
                stopCurve2();
                stopCurve1();
                stopSubCircle(true, true);
                stopADD();
                break;
            case "FINAL":
                stopMainCircle(true, true);
                activateFinalPose();
                break;
            default:
                shakeCurveLeftRight(3, 2);
                break;
        }
    }

    public static float Check(float b)
    {
        if (b < 0)
        {
            return 0;
        }
        else
        {
            return b;
        }
    }

    ParticleSystem buildFountain(Vector3 position, float speed = 3, float number = 50, float lifetime = 0)
    {

        GameObject particleSystemObject = Instantiate(particleSystemPrefab);
        // GameObject bottomLightObj = Instantiate(bottomLight);

        ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particleSystem.main;
        particleSystem.transform.rotation = Quaternion.Euler(-90, 0, 0);
        particleSystem.transform.position = position;
        particleSystem.transform.localScale = new Vector3(10, 10, 10);

        if (lifetime != 0)
        {
            mainModule.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, lifetime);
        }

        mainModule.startSpeed = speed;

        //bottomLightObj.transform.position = position;

        var emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = number;

        particleSystem.Play();
        return particleSystem;
    }

    void stopADD()
    {
        int len = ADDList.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem particle = ADDList[i];
            ParticleSystem.EmissionModule emission = particle.emission;
            ParticleSystem.MainModule main = particle.main;
            emission.enabled = false;
        }
    }

    // curve
    // stop
    void stopCurve1()
    {
        int len = curveFountain.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = curveFountain[i].main;
            main.startSpeed = 0;
            ParticleSystem.EmissionModule emission = curveFountain[i].emission;
            emission.enabled = false;
        }
    }
    void stopCurve2()
    {
        int len = curveFountain2.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = curveFountain2[i].main;
            main.startSpeed = 0;
            ParticleSystem.EmissionModule emission = curveFountain2[i].emission;
            emission.enabled = false;
        }
    }

    void setDirectionUpCurve1()
    {
        int len = curveFountain.Count;
        for (int i = 0; i < len; i++)
        {
            curveFountain[i].transform.rotation = Quaternion.LookRotation(Vector3.up);
        }
    }

    void setCurve1Velocity(float speed = 3)
    {
        int len = curveFountain.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = curveFountain[i].main;
            main.startSpeed = speed;
        }
    }

    void setCurve2Velocity(float speed = 3)
    {
        int len = curveFountain2.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = curveFountain2[i].main;
            ParticleSystem.EmissionModule emission = curveFountain2[i].emission;
            emission.enabled = true;
            main.startSpeed = speed;
        }
    }
    //左右喷射，微小摆动，两个一组，间隔四个，最后大家一起发射
    private float shakeCurveLeftRightBy2Timer;
    void shakeCurveLeftRightBy2(float width, float velocity, bool isRight = true)
    {
        int len = curveFountain.Count;
        shakeCurveLeftRightBy2Timer += Time.deltaTime * 1;
        float radius = width;
        Vector3 xzDirection = isRight ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1);
        Vector3 left = xzDirection.normalized * 0.5f;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem.MainModule main = fountain.main;
            ParticleSystem.EmissionModule emission = fountain.emission;
            if (i % 6 == 0)
            {
                main.startSpeed = 8;
                emission.enabled = true;
                float sinValue = Mathf.Sin(shakeCurveLeftRightBy2Timer * velocity) * 0.05f;

                Vector3 newDirection = Vector3.up + left + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else if (i % 6 == 1)
            {
                main.startSpeed = 8;
                emission.enabled = true;
                float sinValue = Mathf.Sin(shakeCurveLeftRightBy2Timer * (velocity + 1)) * 0.05f;
                Vector3 newDirection = Vector3.up + left + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
            {
                main.startSpeed = 0;
                emission.enabled = false;
            }

        }
    }

    //交叉喷射，微小摆动，两个一组，间隔四个，最后大家一起发射
    private float crossCurveLeftRightBy2Timer;
    void crossCurveLeftRightBy2(float width, float velocity, int step = 0)
    {
        int len = curveFountain.Count;
        crossCurveLeftRightBy2Timer += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(crossCurveLeftRightBy2Timer * velocity) * 0.05f;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem.MainModule main = fountain.main;
            ParticleSystem.EmissionModule emission = fountain.emission;
            if (i % 6 == step)
            {
                emission.enabled = true;
                Vector3 xzDirection = new Vector3(0, 0, -1);
                Vector3 left = 0.3f * xzDirection.normalized;
                main.startSpeed = 8;
                Vector3 newDirection = Vector3.up + left + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else if (i % 6 == step + 1)
            {
                emission.enabled = true;
                Vector3 xzDirection = new Vector3(0, 0, 1);
                Vector3 right = 0.3f * xzDirection.normalized;
                main.startSpeed = 8;
                Vector3 newDirection = Vector3.up + right + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
            {
                main.startSpeed = 0;
                emission.enabled = false;
            }

        }
    }


    //左右喷射，微小摆动，四个一组，第三个比较高，间隔八个

    private float shakeCurveLeftRightBy4Timer;
    void shakeCurveLeftRightBy4(float width, float velocity, int step = 0)
    {
        int len = curveFountain.Count;
        shakeCurveLeftRightBy4Timer += Time.deltaTime * 1;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem fountain2 = curveFountain2[i];
            ParticleSystem.MainModule main = fountain.main;
            ParticleSystem.EmissionModule emission = fountain.emission;
            ParticleSystem.MainModule main2 = fountain2.main;
            if (i % 10 == step || i % 10 == step + 1 || i % 10 == step + 2 || i % 10 == step + 3)
            {
                float sinValue = Mathf.Sin(shakeCurveLeftRightBy4Timer * velocity + i) * 0.1f;
                main2.startSpeed = 0;
                Vector3 xzDirection = new Vector3(0, 0, -1);
                Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
                emission.enabled = true;
                main.startSpeed = 8f;
                if (i % 10 == 2)
                {
                    main.startSpeed = 10f;
                }
            }
            else
            {
                main.startSpeed = 0;
                emission.enabled = false;
                main2.startSpeed = 2;

            }

        }
    }

    //左右交叉，四个左四个右，微小摆动
    private float shakeCurveLeftRightCrossTimer;
    void shakeCurveLeftRightCross(float width, float velocity)
    {
        stopCurve2();
        int len = curveFountain.Count;
        shakeCurveLeftRightCrossTimer += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(shakeCurveLeftRightCrossTimer * velocity) * 0.05f;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem.MainModule main = fountain.main;
            ParticleSystem.EmissionModule emission = fountain.emission;
            emission.enabled = true;
            main.startSpeed = 8f;
            if (i % 8 == 0 || i % 8 == 1 || i % 8 == 2 || i % 8 == 3)
            {
                Vector3 xzDirection = new Vector3(0, 0, -1);
                Vector3 left = xzDirection.normalized * 0.3f;
                Vector3 newDirection = Vector3.up + left + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
            {
                Vector3 xzDirection = new Vector3(0, 0, 1);
                Vector3 right = xzDirection.normalized * 0.3f;
                Vector3 newDirection = Vector3.up + right + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }

        }
    }

    //正弦函数摆动
    void updateCurveBySin()
    {
        curveTimer += Time.deltaTime * 1;
        int len = curveMainList.Count;
        for (int i = 0; i < len; i++)
        {
            float sinValue = Check(Mathf.Sin(curveTimer + i) * 3);
            float initV = (3 / len) * i;
            ParticleSystem.MainModule v = curveMainList[i];
            v.startSpeed = (initV + sinValue) % 3;
        }
    }

    // 正弦函数固定发射
    void fixedCurveBySin()
    {
        int len = curveMainList.Count;
        for (int i = 0; i < len; i++)
        {
            float sinValue = Math.Abs(Mathf.Sin((float)(i * Math.PI / 6)) * 3);
            ParticleSystem.MainModule v = curveMainList[i];
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem.EmissionModule emission = fountain.emission;
            emission.enabled = true;
            v.startSpeed = 8 + sinValue;
        }
    }

    //全部左右摇摆
    private float shakeCurveLeftRightTimer;
    void shakeCurveLeftRight(float width, float velocity)
    {
        int len = curveFountain.Count;
        shakeCurveLeftRightTimer += Time.deltaTime * 1;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            float sinValue = Mathf.Sin(shakeCurveLeftRightTimer * velocity + i) * 0.1f;
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem.MainModule main = fountain.main;
            ParticleSystem.EmissionModule emission = fountain.emission;
            emission.enabled = true;
            main.startSpeed = 8f;
            Vector3 xzDirection = new Vector3(0, 0, -1);
            Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
            fountain.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    // 左边向左，右边向右
    void spiltCurveLeftRight(float width, float velocity)
    {
        int len = curveFountain.Count;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem.MainModule main = fountain.main;
            ParticleSystem.EmissionModule emission = fountain.emission;
            emission.enabled = true;
            main.startSpeed = 8f;
            Vector3 xzDirection = i < len / 2 ? new Vector3(0, 0, 1) * 0.2f : new Vector3(0, 0, -1) * 0.2f;
            Vector3 newDirection = Vector3.up + radius * xzDirection;
            fountain.transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    // main circle
    // stop
    void stopMainCircle(bool inner, bool outside)
    {
        int len = mainCircleFounatin.Count;
        for (int i = 0; i < len; i++)
        {
            if (inner)
            {
                ParticleSystem.MainModule main = mainCircleFounatin[i].main;
                ParticleSystem.EmissionModule emission = mainCircleFounatin[i].emission;
                emission.enabled = false;
                main.startSpeed = 0;
            }
            if (outside)
            {
                ParticleSystem.MainModule main = innerMainCircleFounatin[i].main;
                ParticleSystem.EmissionModule emission = innerMainCircleFounatin[i].emission;
                emission.enabled = false;
                main.startSpeed = 0;
            }

        }
    }
    // 外环向外/向里

    void mainCircleIncline(bool outside = true)
    {
        int len = mainCircleFounatin.Count;
        float radius = 0.5f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection;
            ParticleSystem.EmissionModule emission = mainCircleFounatin[i].emission;
            ParticleSystem.MainModule main = mainCircleFounatin[i].main;
            emission.enabled = true;
            main.startSpeed = 8;
            if (outside)
            {
                xzDirection = -1 * (circleCenterMain - mainCircleTransform[i].position).normalized;

            }
            else
            {
                xzDirection = (circleCenterMain - mainCircleTransform[i].position).normalized;
            }
            Vector3 newDirection = Vector3.up + radius * xzDirection;
            Console.WriteLine(newDirection.ToString());
            mainCircleTransform[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private float innerMainCircleSinTimer;
    void innerMainCircleSin()
    {
        innerMainCircleSinTimer += Time.deltaTime * 1;
        int len = innerMainCircleFounatin.Count;
        for (int i = 0; i < len; i++)
        {
            float sinValue = Check(Mathf.Sin((float)((i + innerMainCircleSinTimer) * Math.PI / 6)) * 3);
            ParticleSystem.MainModule v = innerMainCircleFounatin[i].main;
            ParticleSystem.EmissionModule emission = innerMainCircleFounatin[i].emission;
            emission.enabled = true;
            v.startSpeed = 8 + sinValue;
        }
    }

    private float mainCircleInclineDynamicTimer;
    void mainCircleInclineDynamic(bool outside = true)
    {
        mainCircleInclineDynamicTimer += Time.deltaTime * 1;
        float sinValue = Math.Abs(Mathf.Sin(mainCircleInclineDynamicTimer * 1));
        int len = mainCircleFounatin.Count;
        float radius = 0.5f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection;
            ParticleSystem.EmissionModule emission = mainCircleFounatin[i].emission;
            ParticleSystem.MainModule main = mainCircleFounatin[i].main;
            emission.enabled = true;
            main.startSpeed = 8;

            xzDirection = (circleCenterMain - mainCircleTransform[i].position).normalized;

            Vector3 outDir = xzDirection * -1f;


            Vector3 newDirection = Vector3.up*2 + xzDirection * -0.2f + outDir * sinValue;
            Console.WriteLine(newDirection.ToString());
            mainCircleTransform[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }

    // 设置小圈速度
    void setInnerMainCircleVelocity(float speed = 1)
    {
        int len = innerMainCircleFounatin.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = innerMainCircleFounatin[i].main;
            main.startSpeed = speed;
        }
    }

    // 设置大圈速度
    void setMainCircleVelocity(float speed = 4)
    {
        int len = mainCircleFounatin.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = mainCircleFounatin[i].main;
            main.startSpeed = speed;
        }
    }

    // 大圈摇摆
    private float updateMainCircleDirectionTimer;
    void updateMainCircleDirection()
    {
        updateMainCircleDirectionTimer += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(updateMainCircleDirectionTimer * 1) * 0.1f;
        int len = mainCircleTransform.Count;
        float radius = 5f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection = (circleCenterMain - mainCircleTransform[i].position).normalized;
            Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
            Console.WriteLine(newDirection.ToString());
            mainCircleTransform[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private float updateInnerMainCircleDirectionTimer;
    void updatInnereMainCircleDirection()
    {
        updateInnerMainCircleDirectionTimer += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(updateInnerMainCircleDirectionTimer * 1) * 0.1f;
        int len = mainCircleTransform.Count;
        float radius = 5f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection = (circleCenterMain - mainCircleTransform[i].position).normalized;
            Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
            Console.WriteLine(newDirection.ToString());
            innerMainCircleFounatin[i].transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    // 向切线方向倾斜

    private float mainCircleInclineTangentTimer;
    void mainCircleInclineTangent(bool outside = true)
    {
        int len = mainCircleFounatin.Count;
        float radius = 0.5f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection = (circleCenterMain - mainCircleTransform[i].position).normalized;
            Vector3 tangent = Vector3.Cross(xzDirection, Vector3.up);
            Vector3 newDirection = Vector3.up + radius * tangent;
            mainCircleTransform[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }

    // sub circle
    // stop
    void stopSubCircle(bool isSub1, bool isSub2)
    {
        int len = mainCircleFounatin.Count;
        if (isSub1)
        {
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.MainModule main = subCircleFountain1[i].main;
                main.startSpeed = 0;
                ParticleSystem.EmissionModule emission = subCircleFountain1[i].emission;
                emission.enabled = false;

            }
        }
        if (isSub2)
        {
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.MainModule main = subCircleFountain2[i].main;
                main.startSpeed = 0;
                ParticleSystem.EmissionModule emission = subCircleFountain2[i].emission;
                emission.enabled = false;
            }
        }

    }

    void setSubCircleVelocity1(float speed = 8)
    {
        int len = subCircleFountain1.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = subCircleFountain1[i].main;
            ParticleSystem.EmissionModule emission = subCircleFountain1[i].emission;
            emission.enabled = true;
            main.startSpeed = speed;
        }
    }

    void setSubCircleVelocity2(float speed = 8)
    {
        int len = subCircleFountain2.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = subCircleFountain2[i].main;
            ParticleSystem.EmissionModule emission = subCircleFountain2[i].emission;
            emission.enabled = true;
            main.startSpeed = speed;
        }
    }

    // 小圈摇摆
    void updateSubCircleDirection1()
    {
        subTimer1 += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(subTimer1 * 1) * 0.1f;
        int len = subCircleTransform1.Count;
        float radius = 3f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection = (circleCenterSub1 - subCircleTransform1[i].position).normalized;
            Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
            Console.WriteLine(newDirection.ToString());
            subCircleTransform1[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }
    void updateSubCircleDirection2()
    {
        subTimer2 += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(subTimer2 * 1) * 0.1f;
        int len = subCircleTransform2.Count;
        float radius = 3f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection = (circleCenterSub2 - subCircleTransform2[i].position).normalized;
            Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
            Console.WriteLine(newDirection.ToString());
            subCircleTransform2[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void startEmit(bool curve1, bool curve2, bool mainCircle1, bool mainCircle2, bool sub1, bool sub2)
    {
        if (curve1)
        {
            int len = curveFountain.Count;
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.EmissionModule emission = curveFountain[i].emission;
                emission.enabled = true;
            }
        }
        if (curve2)
        {
            int len = curveFountain2.Count;
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.EmissionModule emission = curveFountain2[i].emission;
                emission.enabled = true;
            }
        }
        if (mainCircle1)
        {
            int len = mainCircleFounatin.Count;
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.EmissionModule emission = mainCircleFounatin[i].emission;
                emission.enabled = true;
            }
        }
        if (mainCircle2)
        {
            int len = innerMainCircleFounatin.Count;
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.EmissionModule emission = innerMainCircleFounatin[i].emission;
                emission.enabled = true;
            }
        }
        if (sub1)
        {
            int len = subCircleFountain1.Count;
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.EmissionModule emission = subCircleFountain1[i].emission;
                emission.enabled = true;
            }
        }
        if (sub2)
        {
            int len = subCircleFountain2.Count;
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.EmissionModule emission = subCircleFountain2[i].emission;
                emission.enabled = true;
            }
        }
    }
}