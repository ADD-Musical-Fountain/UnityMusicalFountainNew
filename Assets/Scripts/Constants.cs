using UnityEngine;
using System;
using System.Collections;
public static class Constants
{
    public const float MUSIC_TIME = 223.0f;
    public const float EXPLOSIONLAUNCHERSYSTEM_RADIUS = 40.0f;
    public const float APPEAR_SPEED = 0.05f;
    public const float DISAPPEAR_SPEED = 0.10f;
    public const int PARTICLE_TYPE_NUM = 2;
    public const int LAUNCH_TIME = 4;

    public static Mesh GetSphereMesh()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.localScale = new Vector3(0.1f, 1.1f, 0.1f);
        Mesh mesh = GameObject.Instantiate(obj.GetComponent<MeshFilter>().mesh);
        return mesh;
    }

    public static Tuple<Vector4, Vector4> GetFireColors()
    {
        Vector4 color1 = new Vector4(250.0f, 47.0f, 13.0f);
        color1 /= 256.0f;
        Vector4 color2 = new Vector4(241.0f, 111.0f, 15.0f);
        color2 /= 256.0f;
        return Tuple.Create(color1, color2);
    }

    public static Tuple<Vector4, Vector4> GetIceColors()
    {
        Vector4 color1 = new Vector4(49.0f, 153.0f, 250.0f);
        color1 /= 256.0f;
        Vector4 color2 = new Vector4(28.0f, 47.0f, 233.0f);
        color2 /= 256.0f;
        return Tuple.Create(color1, color2);
    }
}
