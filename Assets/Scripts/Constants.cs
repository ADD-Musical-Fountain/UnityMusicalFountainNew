using UnityEngine;
using System.Collections;
public static class Constants
{
    public const float MUSIC_TIME = 223.0f;
    public const float EXPLOSIONLAUNCHERSYSTEM_RADIUS = 40.0f;
    public const int PARTICLE_TYPE_NUM = 2;
    public const int LAUNCH_TIME = 4;

    public static ParticleSystem.MinMaxCurve GetParticleLifeTime()
    {
        return new ParticleSystem.MinMaxCurve(1.0f, 1.5f);
    }

    public static ParticleSystem.MinMaxCurve GetParticleSize()
    {
        return new ParticleSystem.MinMaxCurve(0.1f, 0.25f);
    }

    public static ParticleSystem.MinMaxCurve GetParticleSpeed()
    {
        return new ParticleSystem.MinMaxCurve(9.0f, 12.0f);
    }

    public static Mesh GetSphereMesh()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = new Vector3(0.1f, 1.1f, 0.1f);
        Mesh mesh = Object.Instantiate(obj.GetComponent<MeshFilter>().mesh);
        return mesh;
    }

    public static Queue GetLaunchTimeTable()
    {
        Queue q = new Queue();
        q.Enqueue(10);
        q.Enqueue(20);
        q.Enqueue(30);
        q.Enqueue(40);

        return q;
    }
}
