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

    void Start()
    {
        //generate cube and pose as a circle(should be replace as fountain)
        mainCircleNum = 32;
        subCircleNum = 32;
        curveNum = 32;

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
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateCurveVelocity();
        //UpdataSubCircleVelocity();
        //shakeCurveLeftRight(5, 2);
        // shakeCurveLeftRightBy2(5, 2);
        // shakeCurveLeftRightBy4(5, 2);
        // shakeCurveLeftRightCross(5, 2);
        updateCurveBySin();
        //setInnerMainCircleVelocity(1);
        //stopMainCurve();
        //mainCircleIncline(true);
        //mainCircleInclineTangent();
        //updateMainCircleDirection();
        //stopCurve2();
        stopCurve2();
        stopMainCircle(true, true);
        setInnerMainCircleVelocity(0);
        stopSubCircle(true, true);

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

    ParticleSystem buildFountain(Vector3 position, float speed = 3)
    {

        GameObject particleSystemObject = Instantiate(particleSystemPrefab);
        // GameObject bottomLightObj = Instantiate(bottomLight);

        ParticleSystem particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particleSystem.main;
        particleSystem.transform.rotation = Quaternion.Euler(-90, 0, 0);
        particleSystem.transform.position = position;
        particleSystem.transform.localScale = new Vector3(10, 10, 10);

        mainModule.startSpeed = speed;

        //bottomLightObj.transform.position = position;

        var emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 50;

        particleSystem.Play();
        return particleSystem;
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
        }
    }
    void stopCurve2()
    {
        int len = curveFountain2.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = curveFountain2[i].main;
            main.startSpeed = 0;
        }
    }
    //左右喷射，微小摆动，两个一组，间隔四个，最后大家一起发射
    private float shakeCurveLeftRightBy2Timer;
    void shakeCurveLeftRightBy2(float width, float velocity)
    {
        int len = curveFountain.Count;
        shakeCurveLeftRightBy2Timer += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(shakeCurveLeftRightBy2Timer * velocity) * 0.1f;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            if (i % 6 == 0 || i % 6 == 1)
            {
                Vector3 xzDirection = new Vector3(0, 0, -1);
                Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
            {
                ParticleSystem.MainModule main = fountain.main;
                main.startSpeed = 0;
            }

        }
    }


    //左右喷射，微小摆动，四个一组，第三个比较高，间隔八个

    private float shakeCurveLeftRightBy4Timer;
    void shakeCurveLeftRightBy4(float width, float velocity)
    {
        int len = curveFountain.Count;
        shakeCurveLeftRightBy4Timer += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(shakeCurveLeftRightBy4Timer * velocity) * 0.1f;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            ParticleSystem fountain2 = curveFountain2[i];
            ParticleSystem.MainModule main = fountain.main;
            ParticleSystem.MainModule main2 = fountain2.main;
            if (i % 10 == 0 || i % 10 == 1 || i % 10 == 2 || i % 10 == 3)
            {
                main2.startSpeed = 0;
                Vector3 xzDirection = new Vector3(0, 0, -1);
                Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
                if (i % 10 == 2)
                {
                    main.startSpeed = 4f;
                }
            }
            else
            {
                main.startSpeed = 0;
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
            if (i % 8 == 0 || i % 8 == 1 || i % 8 == 2 || i % 8 == 3)
            {
                Vector3 xzDirection = new Vector3(0, 0, -1);
                Vector3 left = xzDirection.normalized;
                Vector3 newDirection = Vector3.up + left + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
            {
                Vector3 xzDirection = new Vector3(0, 0, 1);
                Vector3 right = xzDirection.normalized;
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

    //全部左右摇摆
    private float shakeCurveLeftRightTimer;
    void shakeCurveLeftRight(float width, float velocity)
    {
        int len = curveFountain.Count;
        shakeCurveLeftRightTimer += Time.deltaTime * 1;
        float sinValue = Mathf.Sin(shakeCurveLeftRightTimer * velocity) * 0.1f;
        float radius = width;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem fountain = curveFountain[i];
            Vector3 xzDirection = new Vector3(0,0,-1);
            Vector3 newDirection = Vector3.up + sinValue * radius * xzDirection;
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
                main.startSpeed = 0;
            }
            if (outside)
            {
                ParticleSystem.MainModule main = innerMainCircleFounatin[i].main;
                main.startSpeed = 0;
            }

        }
    }
    // 外环向外/向里

    private float mainCircleInclineOutsideTimer;
    void mainCircleIncline(bool outside = true)
    {
        mainCircleInclineOutsideTimer += Time.deltaTime * 1;
        float sinValue = Math.Abs(Mathf.Sin(mainCircleInclineOutsideTimer * 1) * 0.1f);
        int len = mainCircleFounatin.Count;
        float radius = 0.5f;
        for (int i = 0; i < len; i++)
        {
            Vector3 xzDirection;
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

    // 设置小圈速度
    void setInnerMainCircleVelocity(float speed=1)
    {
        int len = innerMainCircleFounatin.Count;
        for(int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = innerMainCircleFounatin[i].main;
            main.startSpeed = speed;
        }
    }

    // 设置大圈速度
    void setMainCircleVelocity(float speed = 3)
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
            }
        }
        if (isSub2)
        {
            for (int i = 0; i < len; i++)
            {
                ParticleSystem.MainModule main = subCircleFountain2[i].main;
                main.startSpeed = 0;
            }
        }

    }

    void setSubCircleVelocity1(float speed = 3)
    {
        int len = subCircleFountain1.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = subCircleFountain1[i].main;
            main.startSpeed = speed;
        }
    }

    void setSubCircleVelocity2(float speed = 3)
    {
        int len = subCircleFountain2.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = subCircleFountain2[i].main;
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
}