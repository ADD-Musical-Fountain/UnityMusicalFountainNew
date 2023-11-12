using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class firework : MonoBehaviour
{

    public Material particleMaterial;
    public Material trailMaterial;
    public Light pointLight;
    public Color[] colorRange;
    private MainModule main;

    // Start is called before the first frame update
    void Start()
    {
        GameObject particleSystemObject = new GameObject("FireWork");
        ParticleSystem particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
        particleSystem.transform.Rotate(-90, 0, 0);

        // renderer
        var renderer = particleSystem.GetComponent<Renderer>();
        renderer.material = particleMaterial;

        // trail render
        var trailRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        trailRenderer.trailMaterial = trailMaterial;

        // main
        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.startSize = 0.5f;

        // color
        main = mainModule;
        int index = Random.Range(0, colorRange.Length);
        main.startColor = colorRange[index];

        // emission
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 2;

        // shape
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(100, 100, 0);

        // light 
        ParticleSystem.LightsModule light = particleSystem.lights;
        light.enabled = true;
        light.light = pointLight;
        light.ratio = 1;

        // curve
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(0.5f, 0.5f);
        curve.AddKey(1f, 0f);

        // trails
        ParticleSystem.TrailModule trail = particleSystem.trails;
        trail.enabled = true;
        trail.lifetime = new ParticleSystem.MinMaxCurve(0.25f);
        trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, curve);

        // sub
        ParticleSystem subFireWork = buildSubFireWork();
        subFireWork.transform.parent = particleSystem.transform;
        ParticleSystem.SubEmittersModule subEmitters = particleSystem.subEmitters;
        subEmitters.enabled = true;
        subEmitters.AddSubEmitter(subFireWork, ParticleSystemSubEmitterType.Death, ParticleSystemSubEmitterProperties.InheritColor);

        particleSystem.Play();
    }

    // Update is called once per frame
    void Update()
    {
        int index = Random.Range(0, colorRange.Length);
        Debug.Log(colorRange[index]);
        main.startColor = colorRange[index];
    }

    ParticleSystem buildSubFireWork()
    {
        GameObject particleSystemObject = new GameObject("Sub FireWork");
        ParticleSystem particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
        particleSystem.transform.Rotate(-90, 0, 0);

        // renderer
        var renderer = particleSystem.GetComponent<Renderer>();
        renderer.material = particleMaterial;

        // trail render
        var trailRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        trailRenderer.trailMaterial = trailMaterial;

        // main
        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.startSize = 0.2f;
        mainModule.gravityModifier = 0.1f;

        // emission
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.burstCount = 200;

        // shape
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;

        // noise
        ParticleSystem.NoiseModule noise = particleSystem.noise;
        noise.enabled = true;
        noise.positionAmount = new ParticleSystem.MinMaxCurve(0.1f);
        noise.rotationAmount = new ParticleSystem.MinMaxCurve(0.7f);
        noise.sizeAmount = new ParticleSystem.MinMaxCurve(0.18f);

        // curve
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(0.5f, 0.5f);
        curve.AddKey(1f, 0f);

        // trails
        ParticleSystem.TrailModule trail = particleSystem.trails;
        trail.enabled = true;
        trail.lifetime = new ParticleSystem.MinMaxCurve(1f, curve);
        trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1f, curve);


        return particleSystem;
    }
}
