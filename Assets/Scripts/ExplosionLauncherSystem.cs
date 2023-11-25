using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace ELSNameSpace
{
    public enum State
    {
        IDLE = 0,
        APPEARING = 1,
        READY = 2,
        VANISHING = 3
    }
    public enum ParticleType
    {
        FIRE = 0,
        ICE = 1
    }

    public class ExplosionLauncherSystem : MonoBehaviour
    {
        public int amount;
        private GameObject[] launchers;
        private ParticleSystem[] particlesystems;
        private State[] states;

        private float appear_speed;
        private float disappear_speed;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void initialSetup(int a)
        {
            amount = a;

            if (amount <= 3)
                amount = 4;
            if (amount % 2 == 1)
            {
                amount += 1;
            }
            if (amount > 30)
            {
                amount = 30;
            }

            launchers = new GameObject[amount];
            particlesystems = new ParticleSystem[amount];

            states = new State[amount];

            appear_speed = 0.006f;
            disappear_speed = 0.006f;
        }

        public void Activate(int index, Vector3 pos, ParticleType type)
        {
            if (index < 0 || index >= amount)
                throw new Exception("Index out of range");
            Tuple<GameObject, ParticleSystem> t = GenerateLauncherSystem(pos, type);
            t.Item1.transform.parent = transform;
            launchers[index] = t.Item1;
            particlesystems[index] = t.Item2;
            launchers[index].GetComponent<MeshRenderer>().material.SetFloat("_DissolveRatio", 3.2f);
            states[index] = State.IDLE;
        }

        public void EmitParticle(int index)
        {
            if(states[index] == State.IDLE)
            {
                states[index] = State.APPEARING;
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < amount; i++)
            {
                if (states[i] == State.IDLE)
                    continue;
                if(states[i] == State.READY)
                {
                    particlesystems[i].Emit(1);
                    states[i] = State.VANISHING;
                    continue;
                }
                var dissolve = launchers[i].GetComponent<MeshRenderer>().material.GetFloat("_DissolveRatio");
                if (states[i] == State.VANISHING)
                {
                    launchers[i].GetComponent<MeshRenderer>().material.SetFloat("_DissolveRatio", dissolve + disappear_speed);
                }
                else if (states[i] == State.APPEARING)
                {
                    launchers[i].GetComponent<MeshRenderer>().material.SetFloat("_DissolveRatio", dissolve - appear_speed);
                }
                if(launchers[i].GetComponent<MeshRenderer>().material.GetFloat("_DissolveRatio") >= 3.2f)
                {
                    states[i] = State.IDLE;
                } else if (launchers[i].GetComponent<MeshRenderer>().material.GetFloat("_DissolveRatio") <= 0.7f)
                {
                    states[i] = State.READY;
                }
            }
        }

        private ParticleType[] GenerateTypeArray(int amount)
        {
            ParticleType[] types = new ParticleType[amount];
            for (int i = 0; i < amount; i++)
            {
                types[i] = (ParticleType)(i % Constants.PARTICLE_TYPE_NUM);
            }
            return types;
        }

        public Tuple<GameObject, ParticleSystem> GenerateLauncherSystem(Vector3 position, ParticleType type)
        {
            var launcher = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            launcher.name = "Launcher";
            launcher.transform.localScale = Vector3.one * 1.3f;
            launcher.transform.position += position;
            var renderer = launcher.GetComponent<MeshRenderer>();
            if (type == ParticleType.FIRE)
            {
                renderer.material = Resources.Load<Material>("shaders/FireOrigin");
            }
            else if (type == ParticleType.ICE)
            {
                renderer.material = Resources.Load<Material>("shaders/IceOrigin");
            }

            ParticleSystem s = launcher.AddComponent<ParticleSystem>();
            s.transform.parent = launcher.transform;
            s.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

            var main = s.main;
            main.duration = 2.0f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(3.0f, 4.0f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(30.0f, 40.0f);
            main.startSize = new ParticleSystem.MinMaxCurve(2.0f, 2.5f);
            main.maxParticles = 100;

            var emission = s.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);

            var shape = s.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;

            var renderer2 = s.GetComponent<ParticleSystemRenderer>();
            if (type == ParticleType.FIRE)
            {
                renderer2.material = Resources.Load<Material>("shaders/FireParticle");
                renderer2.trailMaterial = Resources.Load<Material>("shaders/BetterTrailFire");
                var colors = Constants.GetFireColors();
                renderer2.trailMaterial.SetVector("_TrailColor1", colors.Item1);
                renderer2.trailMaterial.SetVector("_TrailColor2", colors.Item2);
                renderer2.renderMode = ParticleSystemRenderMode.Mesh;
                renderer2.mesh = Constants.GetSphereMesh();
            }
            else if (type == ParticleType.ICE)
            {
                renderer2.material = Resources.Load<Material>("shaders/IceParticle");
                renderer2.trailMaterial = Resources.Load<Material>("shaders/BetterTrailIce");
                var colors = Constants.GetIceColors();
                renderer2.trailMaterial.SetVector("_TrailColor1", colors.Item1);
                renderer2.trailMaterial.SetVector("_TrailColor2", colors.Item2);
                renderer2.renderMode = ParticleSystemRenderMode.Mesh;
                renderer2.mesh = Constants.GetSphereMesh();
            }

            var trail = s.trails;
            trail.enabled = true;
            trail.mode = ParticleSystemTrailMode.PerParticle;
            trail.ratio = 1.0f;
            trail.lifetime = 0.6f;
            trail.minVertexDistance = 1.0f;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 1f);
            curve.AddKey(0.5f, 0.25f);
            curve.AddKey(1f, 0f);
            trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1.0f, curve);

            Tuple<GameObject, ParticleSystem> subs = GenerateSubExplosionSystem(type);
            subs.Item1.transform.parent = launcher.transform;
            subs.Item2.transform.parent = s.transform;
            ParticleSystem.SubEmittersModule subEmitters = s.subEmitters;
            subEmitters.enabled = true;
            subEmitters.AddSubEmitter(subs.Item2, ParticleSystemSubEmitterType.Death, ParticleSystemSubEmitterProperties.InheritNothing);

            return Tuple.Create(launcher, s);
        }

        public Tuple<GameObject, ParticleSystem> GenerateSubExplosionSystem(ParticleType type)
        {
            GameObject obj = new GameObject("sub");
            ParticleSystem p = obj.AddComponent<ParticleSystem>();
            VisualEffect e = obj.AddComponent<VisualEffect>();
            p.transform.parent = obj.transform;
            e.transform.parent = p.transform;
            e.visualEffectAsset = Resources.Load<VisualEffectAsset>("VGXGraphs/BetterTrailVFX");
            
            if(type == ParticleType.FIRE)
            {
                e.SetVector4("Color1", new Vector4(2.52f, 0.016f, 0.016f, 1f));
            } else if (type == ParticleType.ICE)
            {
                e.SetVector4("Color1", new Vector4(0.12f, 0.87f, 1f, 1f));
            }

            e.enabled = true;

            var main = p.main;
            main.duration = 20.0f;
            main.loop = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(2.0f, 3.2f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(40.0f, 50.0f);
            main.startSize = new ParticleSystem.MinMaxCurve(1.2f, 1.3f);
            main.maxParticles = 1000;

            var emission = p.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(70.0f, 80.0f);
            emission.rateOverDistance = new ParticleSystem.MinMaxCurve(0.0f, 0.0f);
            emission.burstCount = 4;

            var shape = p.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;

            var renderer = p.GetComponent<ParticleSystemRenderer>();
            if (type == ParticleType.FIRE)
            {
                renderer.material = Resources.Load<Material>("shaders/FireParticle");
                renderer.trailMaterial = Resources.Load<Material>("shaders/BetterTrailFire");
                renderer.renderMode = ParticleSystemRenderMode.Mesh;
                renderer.mesh = Constants.GetSphereMesh();
            }
            else if (type == ParticleType.ICE)
            {
                renderer.material = Resources.Load<Material>("shaders/IceParticle");
                renderer.trailMaterial = Resources.Load<Material>("shaders/BetterTrailIce");
                renderer.renderMode = ParticleSystemRenderMode.Mesh;
                renderer.mesh = Constants.GetSphereMesh();
            }

            var trail = p.trails;
            trail.enabled = true;
            trail.mode = ParticleSystemTrailMode.PerParticle;
            trail.ratio = 1.0f;
            trail.lifetime = 0.6f;
            trail.minVertexDistance = 4.0f;

            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 1f);
            curve.AddKey(0.5f, 0.1f);
            curve.AddKey(1f, 0f);
            trail.widthOverTrail = new ParticleSystem.MinMaxCurve(1.0f, curve);

            return Tuple.Create(obj, p);
        }
    }
}