using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticle : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Material m = Resources.Load<Material>("shaders/FireParticle");
        gameObject.GetComponent<Renderer>().material = m;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
