using UnityEngine;
public static class Constants
{
    public const float MUSIC_TIME = 223.0f;

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
}
