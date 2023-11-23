using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{

    [SerializeField]
    public ParticleType type;

    [SerializeField]
    public AnimationCurve curve;

    private ParticleSystem particlesystem;

    // Start is called before the first frame update
    void Start()
    {
        if (type == ParticleType.FIRE)
        {
            var particleSystem = gameObject.AddComponent<ParticleSystem>();
            SetParticleSystem(ref particleSystem);
            particleSystem.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetParticleSystem(ref ParticleSystem p)
    {

        var main = p.main;
        main.duration = 3.0f;
        main.loop = false;
        main.startLifetime = Constants.GetParticleLifeTime();
        main.startSpeed = Constants.GetParticleSpeed();
        main.startSize = Constants.GetParticleSize();
        main.maxParticles = 1000;

        var emission = p.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(10.0f, 20.0f);
        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);

        var shape = p.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;

        var renderer = GetComponent<ParticleSystemRenderer>();
        SetParticleSystemRenderder(ref renderer);

        var trail = p.trails;
        SetParticleSystemTrail(ref trail);
    }

    private void SetParticleSystemRenderder(ref ParticleSystemRenderer renderer)
    {
        if (type == ParticleType.FIRE)
        {
            renderer.material = Resources.Load<Material>("shaders/FireParticle");
            renderer.trailMaterial = Resources.Load<Material>("shaders/FireExplosionParticle");
            renderer.renderMode = ParticleSystemRenderMode.Mesh;
            renderer.mesh = Constants.GetSphereMesh();
        }
    }

    private void SetParticleSystemTrail(ref ParticleSystem.TrailModule trail)
    {
        trail.enabled = true;
        trail.mode = ParticleSystemTrailMode.PerParticle;
        trail.ratio = 1.0f;
        trail.lifetime = 0.05f;
        trail.minVertexDistance = 1.0f;
        trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1.0f, curve);
    }
}
