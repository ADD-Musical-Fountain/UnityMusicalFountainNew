using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBase : MonoBehaviour
{
    void Awake()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<BoxCollider>();
        gameObject.AddComponent<MeshRenderer>();
    }
}
