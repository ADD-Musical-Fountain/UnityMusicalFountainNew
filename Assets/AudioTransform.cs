using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;
using MusicEventNameSpace;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using ELSNameSpace;

public class AudioTransform : MonoBehaviour
{
    // audio
    public AudioSource thisAudioSource;
    private float[] spectrumData = new float[1024];
    public bool startPlay = false;
    public List<Vector3> curveControlPoint = new List<Vector3>();
    public GameObject WaterBallPrefab;
    public GameObject _SplashPrefab;
    public GameObject _SpillPrefab;
    // public GameObject waterBall;
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

    int mainCircleNum = 32;
    int subCircleNum = 32;
    int curveNum = 32;

    float volume;

    private List<float> frequencyBands = new List<float>();
    public Material trailMaterial;
    public Mesh[] meshes;

    // particle system attribute
    public int particleCount = 100;
    public float particleSpeed = 8f;
    public float particleSize = 1.7f;
    public float particleLifetime = 1f;
    public float particleSpread = 0.2f;

    private Stopwatch stopwatch;

    public GameObject particleSystemPrefab;
    // public GameObject particleSystemPrefabBottom;
    public GameObject bottomLight;

    private EventEmitter emitter;
    private List<ParticleSystem> ADDList = new List<ParticleSystem>();
    private ExplosionLauncherSystem explosion;

    void Start()
    {
        // initial emitter
        emitter = new EventEmitter();
        emitter.getEventList();
    }

    // Update is called once per frame
    void Update()
    {

        if (curveControlPoint.Count < 4)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    curveControlPoint.Add(hit.point);

                    Vector3 waterballPos = new Vector3(hit.point[0], 0.5f, hit.point[2]);
                    GameObject waterballObject = Instantiate(WaterBallPrefab);
                    ParticleSystem waterball = waterballObject.GetComponent<ParticleSystem>();
                    waterball.transform.position = waterballPos;

                    GameObject splasObejct = Instantiate(_SplashPrefab);
                    ParticleSystem splas = splasObejct.GetComponent<ParticleSystem>();
                    splas.transform.position = hit.point;
                    splas.Play();
                    Debug.Log("the position of selected control point:" + hit.point);
                }
            }
        }
        if (curveControlPoint.Count == 4 && startPlay == false)
        {
            generateCurveFountain();
            generateCurve2Fountain();
            generateSubCircleFountain();
            generateMainCircleFountain();
            startPlay = true;
            stopwatch = new Stopwatch();
            stopwatch.Start();
            // delay to play audio
            finalPose();
            thisAudioSource.PlayDelayed(2f);
        }
        processEmitter();
        audioAttribute();
    }
    public void finalPose()

    {
        Vector3 finalPos = new Vector3((curveControlPoint[1][0] + curveControlPoint[2][0]) / 2, 0, (curveControlPoint[1][2] + curveControlPoint[2][2]) / 2);
        stopMainCircle(true, true);
        stopSubCircle(true, true);
        stopCurve1();
        stopCurve2();
        ParticleSystem A1 = buildFountain(new Vector3(finalPos[0], 0, finalPos[2] + 150), 15, 400);
        ParticleSystem A2 = buildFountain(new Vector3(finalPos[0], 0, finalPos[2] + 50), 15, 400);
        ParticleSystem A3 = buildFountain(new Vector3(finalPos[0], 100, finalPos[2] + 175), 8, 400);
        A1.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, -0.1f));
        A2.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, 0.1f));
        A3.transform.rotation = Quaternion.LookRotation(Vector3.up * 2 + new Vector3(0, 0, -2f));

        ParticleSystem D1 = buildFountain(new Vector3(finalPos[0], 0, finalPos[2] + 0), 15, 400);
        ParticleSystem D2 = buildFountain(new Vector3(finalPos[0], 200, finalPos[2] + 0), 5, 400, 4);
        ParticleSystem D3 = buildFountain(new Vector3(finalPos[0], 0, finalPos[2] + 0), 15, 400);

        D2.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, -0.8f));
        D3.transform.rotation = Quaternion.LookRotation(Vector3.up*0.5f + new Vector3(0, 0, -1f));
        var d3main = D3.main;
        d3main.gravityModifier = 0;
        d3main.startLifetime = new ParticleSystem.MinMaxCurve(1,1.5f);

        ParticleSystem D4 = buildFountain(new Vector3(finalPos[0], 0, finalPos[2] - 130), 15, 400);
        ParticleSystem D5 = buildFountain(new Vector3(finalPos[0], 200, finalPos[2] - 130), 5, 400, 4);
        ParticleSystem D6 = buildFountain(new Vector3(finalPos[0], 0, finalPos[2] - 130), 15, 400);

        D5.transform.rotation = Quaternion.LookRotation(Vector3.up + new Vector3(0, 0, -0.8f));
        D6.transform.rotation = Quaternion.LookRotation(Vector3.up * 0.5f + new Vector3(0, 0, -1f));
        var d6main = D6.main;
        d6main.gravityModifier = 0;
        d6main.startLifetime = new ParticleSystem.MinMaxCurve(1, 1.5f);

        ADDList.Add(A1);
        ADDList.Add(A2);
        ADDList.Add(A3);
        ADDList.Add(D1);
        ADDList.Add(D2);
        ADDList.Add(D3);
        ADDList.Add(D4);
        ADDList.Add(D5);
        ADDList.Add(D6);
        stopADD();

        explosion = gameObject.AddComponent<ExplosionLauncherSystem>();
        explosion.initialSetup(8);
        // explosion.transform.position = Vector3.zero;
        explosion.Activate(0, finalPos + new Vector3(40, 10, 0), ParticleType.FIRE);
        explosion.Activate(1, finalPos + new Vector3(0, 10, -40), ParticleType.ICE);
        explosion.Activate(2, finalPos + new Vector3(-40, 10, 0), ParticleType.FIRE);
        explosion.Activate(3, finalPos + new Vector3(0, 10, 40), ParticleType.ICE);
        explosion.Activate(4, finalPos + new Vector3(0, 10, 60), ParticleType.FIRE);
        explosion.Activate(5, finalPos + new Vector3(0, 10, -60), ParticleType.ICE);
        explosion.Activate(6, finalPos + new Vector3(0, 10, 80), ParticleType.FIRE);
        explosion.Activate(7, finalPos + new Vector3(0, 10, -80), ParticleType.ICE);

        for(int i = 0; i < 8; i++)
        {
            explosion.EmitParticle(i);
        }
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

        for (int i = 0; i < 8; i++)
        {
            explosion.EmitParticle(i);
        }
    }

    public void processEmitter()
    {
        if (emitter.eventList.Count == 0)
        {
            return;
        }
        if (stopwatch.Elapsed.TotalSeconds < emitter.eventList[0].duration)
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
            case "START":
                setCurve1Velocity(volume-2);
                setCurve2Velocity(volume-2);
                setMainCircleVelocity(volume - 2);
                setInnerMainCircleVelocity(volume - 2);
                setSubCircleVelocity1(volume - 2);
                setSubCircleVelocity2(volume-2);
                break;
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
                crossCurveLeftRightBy2(5, 2, 0);
                setCurve2Velocity(volume-2);
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "C2":
                crossCurveLeftRightBy2(5, 2, 1);
                setCurve2Velocity(volume-2);
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "C3":
                crossCurveLeftRightBy2(5, 2, 2);
                setCurve2Velocity(volume-2);
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
                setCurve1Velocity(volume);
                shakeCurveLeftRightCross(2, 2);
                stopCurve2();
                stopMainCircle(true, true);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "G":
                setMainCircleVelocity(volume+2);
                mainCircleInclineDynamic();
                spiltCurveLeftRight(2, 2);
                stopCurve2();
                stopSubCircle(true, true);
                stopADD();
                break;
            case "H":
                mainCircleInclineTangent();
                setSubCircleVelocity1(volume+2);
                setSubCircleVelocity2(volume+2);
                emitFirework();
                stopCurve2();
                setDirectionUpCurve1();
                setCurve1Velocity(8);
                stopADD();
                break;
            case "I":
                mainCircleIncline();
                innerMainCircleSin();
                stopCurve2();
                setDirectionUpCurve1();
                emitFirework();
                setCurve1Velocity(volume);
                stopSubCircle(true, true);
                stopADD();
                break;
            case "FINAL":
                stopMainCircle(true, true);
                stopCurve1();
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

    void emitFirework()
    {
        for (int i = 0; i < 8; i++)
        {
            explosion.EmitParticle(i);
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
                main.startSpeed = volume-2;
                emission.enabled = true;
                float sinValue = Mathf.Sin(shakeCurveLeftRightBy2Timer * velocity) * 0.05f;

                Vector3 newDirection = Vector3.up + left + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else if (i % 6 == 1)
            {
                main.startSpeed = volume-2;
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
                main.startSpeed = volume-2;
                Vector3 newDirection = Vector3.up + left + sinValue * radius * xzDirection;
                fountain.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else if (i % 6 == step + 1)
            {
                emission.enabled = true;
                Vector3 xzDirection = new Vector3(0, 0, 1);
                Vector3 right = 0.3f * xzDirection.normalized;
                main.startSpeed = volume-2;
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
                main.startSpeed = volume-2;
                if (i % 10 == 2)
                {
                    main.startSpeed = volume;
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
            main.startSpeed = volume;
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
            v.startSpeed = volume + sinValue;
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
            main.startSpeed = volume;
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
            main.startSpeed = volume;
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
            v.startSpeed = volume + sinValue;
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
            main.startSpeed = volume;

            xzDirection = (circleCenterMain - mainCircleTransform[i].position).normalized;

            Vector3 outDir = xzDirection * -1f;


            Vector3 newDirection = Vector3.up * 2 + xzDirection * -0.2f + outDir * sinValue;
            Console.WriteLine(newDirection.ToString());
            mainCircleTransform[i].rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void setInnerMainCircleVelocity(float speed = 1)
    {
        int len = innerMainCircleFounatin.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = innerMainCircleFounatin[i].main;
            ParticleSystem.EmissionModule emission = innerMainCircleFounatin[i].emission;
            emission.enabled = true;
            main.startSpeed = speed;
        }
    }

    void setMainCircleVelocity(float speed = 4)
    {
        int len = mainCircleFounatin.Count;
        for (int i = 0; i < len; i++)
        {
            ParticleSystem.MainModule main = mainCircleFounatin[i].main;
            ParticleSystem.EmissionModule emission = mainCircleFounatin[i].emission;
            emission.enabled = true;
            main.startSpeed = speed;
        }
    }


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

    public void audioAttribute()
    {
        thisAudioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
        // frequency bands
        int frequencyBandsNum = 8;
        int sampleCount = 1024 / frequencyBandsNum;
        volume = 0;
        for (int i = 0; i < frequencyBandsNum; i++)
        {
            float average = 0;
            for (int j = i * sampleCount; j < (i + 1) * sampleCount; j++)
            {
                average = average + spectrumData[j];
            }
            if (frequencyBands.Count < frequencyBandsNum)
            {
                frequencyBands.Add(average);
            }
            else
            {
                frequencyBands[i] = average;
            }
            volume += frequencyBands[i];
        }
        Debug.Log("the current volume:" + volume);
        if (volume < 4)
        {
            volume += 8;
        }
        else if (volume > 4 && volume < 8)
        {
            volume += 4;
        }
        else if (volume > 8 && volume < 12)
        {
            volume = volume;
        }
        else
        {
            volume = 12;
        }
        Debug.Log("the modify volume:" + volume);
        // volumelist.Add(volume);
        // Debug.Log("the max of volume:"+Mathf.Max(volumelist.ToArray()));
    }
    public Vector3 cubicBezier(float t)
    {
        Vector3 a = curveControlPoint[0];
        Vector3 b = curveControlPoint[1];
        Vector3 c = curveControlPoint[2];
        Vector3 d = curveControlPoint[3];

        float B0 = (1 - t) * (1 - t) * (1 - t);
        float B1 = 3 * t * (1 - t) * (1 - t);
        float B2 = 3 * t * t * (1 - t);
        float B3 = t * t * t;

        float x = B0 * a[0] + B1 * b[0] + B2 * c[0] + B3 * d[0];
        float z = B0 * a[2] + B1 * b[2] + B2 * c[2] + B3 * d[2];
        float y = 0;
        Vector3 position = new Vector3(x, y, z);
        return position;
    }
    public void generateCurveFountain()
    {
        for (int i = 0; i < curveNum; i++)
        {
            float t = (i * 1.0f) / curveNum;
            Vector3 posi = cubicBezier(t);
            ParticleSystem fountain = buildFountain(posi);
            curveMainList.Add(fountain.main);
            curveFountain.Add(fountain);
        }
    }


    public void generateCurve2Fountain()
    {
        for (int i = 0; i < curveNum; i++)
        {
            float t = (i * 1.0f) / curveNum;
            Vector3 posi = cubicBezier(t);
            ParticleSystem fountain = buildFountain(posi);
            curveFountain2.Add(fountain);
        }
    }
    public void generateSubCircleFountain()
    {
        float angle = 0f;
        float angleStep = 360f / (subCircleNum);
        float subCircleRadius = 50f;

        Vector3 subCircleStartPos1 = new Vector3((curveControlPoint[0][0] + curveControlPoint[1][0]) / 2, 0, (curveControlPoint[0][2] + curveControlPoint[1][2]) / 2);
        circleCenterSub1 = subCircleStartPos1;

        for (int i = 0; i < subCircleNum; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * subCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * subCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + subCircleStartPos1);
            subCircleTransform1.Add(fountain.transform);
            subCircleFountain1.Add(fountain);
            angle += angleStep;
            Debug.Log("sub circle fountain position:" + posi);
        }

        angle = 0f;
        Vector3 subCircleStartPos2 = new Vector3((curveControlPoint[2][0] + curveControlPoint[3][0]) / 2, 0, (curveControlPoint[2][2] + curveControlPoint[3][2]) / 2);
        circleCenterSub2 = subCircleStartPos2;
        for (int i = 0; i < subCircleNum; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * subCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * subCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + subCircleStartPos2);
            subCircleTransform2.Add(fountain.transform);
            subCircleFountain2.Add(fountain);
            angle += angleStep;
        }
    }
    public void generateMainCircleFountain()
    {
        float angle = 0f;
        float angleStep = 360f / (mainCircleNum);
        float mainCircleRadius = 80f;

        Vector3 mainCircleSratPos = new Vector3((curveControlPoint[1][0] + curveControlPoint[2][0]) / 2, 0, (curveControlPoint[1][2] + curveControlPoint[2][2]) / 2);
        circleCenterMain = mainCircleSratPos;

        for (int i = 0; i < mainCircleNum; i++)
        {
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
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * mainCircleRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * mainCircleRadius;
            Vector3 posi = new Vector3(x, 0, z);
            ParticleSystem fountain = buildFountain(posi + mainCircleSratPos);
            innerMainCircleFounatin.Add(fountain);
            angle += angleStep;
        }
    }
}

