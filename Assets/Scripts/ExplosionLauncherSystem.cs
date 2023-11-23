using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLauncherSystem : MonoBehaviour
{
    [SerializeField]
    public int amount;

    private GameObject[] launchers;
    private Queue launch_time_table;

    // Start is called before the first frame update
    void Start()
    {
        if (amount <= 3)
            amount = 4;

        launchers = new GameObject[amount];
        launch_time_table = Constants.GetLaunchTimeTable();

        var typelist = GenerateTypeArray(amount);

        for(int i = 0; i < amount; i++)
        {
            GameObject launcher = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            launcher.name = "Launcher" + i;
            launcher.transform.parent = transform;
            launcher.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            float x = Constants.EXPLOSIONLAUNCHERSYSTEM_RADIUS * Mathf.Sin(i * (2.0f * Mathf.PI / amount));
            float z = Constants.EXPLOSIONLAUNCHERSYSTEM_RADIUS * Mathf.Cos(i * (2.0f * Mathf.PI / amount));
            var pos = new Vector3(x, 0, z);
            launcher.transform.position = launcher.transform.position + pos;

            ParticleSystem s = launcher.AddComponent<ParticleSystem>();
            SetLauncherSystem(ref s, typelist[i]);
            launchers[i] = launcher;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(launch_time_table.Count > 0)
        {
            var t = (int)launch_time_table.Peek();
            if(Time.time - t >= 0.0f)
            {
                for(int i = 0; i < amount; i++)
                {
                    var ps = launchers[i].GetComponent<ParticleSystem>();
                    ps.Emit(1);
                }
                launch_time_table.Dequeue();
            }
        }
    }

    private ParticleType[] GenerateTypeArray(int amount)
    {
        ParticleType[] types = new ParticleType[amount];
        for(int i = 0; i < amount; i++)
        {
            types[i] = ParticleType.FIRE;
        }
        return types;
    }

    private void SetLauncherSystem(ref ParticleSystem s, ParticleType type)
    {
        var main = s.main;
        main.duration = 2.0f;
        main.loop = true;
        main.startLifetime = Constants.GetParticleLifeTime();
        main.startSpeed = Constants.GetParticleSpeed();
        main.startSize = Constants.GetParticleSize();
        main.maxParticles = 100;

        var emission = s.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);
        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);

        var shape = s.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;

        var renderer = s.GetComponent<ParticleSystemRenderer>();
        if (type == ParticleType.FIRE)
        {
            renderer.material = Resources.Load<Material>("shaders/FireParticle");
            renderer.trailMaterial = Resources.Load<Material>("shaders/FireExplosionParticle");
            renderer.renderMode = ParticleSystemRenderMode.Mesh;
            renderer.mesh = Constants.GetSphereMesh();
        }

        var trail = s.trails;
        trail.enabled = true;
        trail.mode = ParticleSystemTrailMode.PerParticle;
        trail.ratio = 1.0f;
        trail.lifetime = 0.05f;
        trail.minVertexDistance = 1.0f;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(0.5f, 0.5f);
        curve.AddKey(1f, 0f);
        trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1.0f, curve);

        ParticleSystem subs = GenerateSubExplosionSystem(type);
        subs.transform.parent = s.transform;
        ParticleSystem.SubEmittersModule subEmitters = s.subEmitters;
        subEmitters.enabled = true;
        subEmitters.AddSubEmitter(subs, ParticleSystemSubEmitterType.Death, ParticleSystemSubEmitterProperties.InheritNothing);

        s.Play();
    }

    private ParticleSystem GenerateSubExplosionSystem(ParticleType type)
    {
        GameObject obj = new GameObject("sub");
        ParticleSystem p = obj.AddComponent<ParticleSystem>();

        var main = p.main;
        main.duration = 20.0f;
        main.loop = false;
        main.startLifetime = Constants.GetParticleLifeTime();
        main.startSpeed = Constants.GetParticleSpeed();
        main.startSize = Constants.GetParticleSize();
        main.maxParticles = 1000;

        var emission = p.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(20.0f, 30.0f);
        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);
        emission.burstCount = 1;

        var shape = p.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;

        var renderer = p.GetComponent<ParticleSystemRenderer>();
        if (type == ParticleType.FIRE)
        {
            renderer.material = Resources.Load<Material>("shaders/FireParticle");
            renderer.trailMaterial = Resources.Load<Material>("shaders/FireExplosionParticle");
            renderer.renderMode = ParticleSystemRenderMode.Mesh;
            renderer.mesh = Constants.GetSphereMesh();
        }

        var trail = p.trails;
        trail.enabled = true;
        trail.mode = ParticleSystemTrailMode.PerParticle;
        trail.ratio = 1.0f;
        trail.lifetime = 0.05f;
        trail.minVertexDistance = 1.0f;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(0.5f, 0.5f);
        curve.AddKey(1f, 0f);
        trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1.0f, curve);

        return p;
    }
}
