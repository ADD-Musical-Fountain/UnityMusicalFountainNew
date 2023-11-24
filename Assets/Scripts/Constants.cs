using UnityEngine;
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
        Mesh mesh = Object.Instantiate(obj.GetComponent<MeshFilter>().mesh);
        return mesh;
    }

    public static Queue GetLaunchTimeTable()
    {
        Queue q = new Queue();
        q.Enqueue(5);
        q.Enqueue(10);
        q.Enqueue(15);
        q.Enqueue(20);
        q.Enqueue(25);
        q.Enqueue(30);
        q.Enqueue(35);
        q.Enqueue(40);

        return q;
    }
}
