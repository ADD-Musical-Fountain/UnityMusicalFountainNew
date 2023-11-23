using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSystemLauncher : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    public ParticleType type;

    private void Start()
    {
        GameObject particleSystemObject = new GameObject("ExplosionSystemLauncher");
        ParticleSystem particleSystem = particleSystemObject.AddComponent<ParticleSystem>();
        particleSystem.transform.rotation = Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f));
    }

    private void Update()
    {
        
    }
}
