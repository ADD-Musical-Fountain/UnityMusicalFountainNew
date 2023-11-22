using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{

    [SerializeField]
    public ParticleType type;

    [SerializeField]
    public int amount;

    public ExplosionSystem(ParticleType type, int amount){
        this.type = type;
        this.amount = amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (type == ParticleType.FIRE)
        {
            var particleSystem = gameObject.AddComponent<ParticleSystem>();
            SetParticleSystem(ref particleSystem);
            var renderer = gameObject.GetComponent<ParticleSystemRenderer>();
            renderer.material = Resources.Load<Material>("shaders/FireParticle");
            particleSystem.transform.parent = this.transform;
            particleSystem.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetParticleSystem(ref ParticleSystem p)
    {

        var main = p.main;
        main.duration = 20.0f;
        main.loop = true;
        main.startLifetime = Constants.getParticleLifeTime();
        main.startSpeed = Constants.getParticleSpeed();
        main.startSize = Constants.getParticleSize();



        var emission = p.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);
        emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);

        var shape = p.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
    }
}
