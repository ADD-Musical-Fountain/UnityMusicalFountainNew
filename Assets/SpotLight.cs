using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLight : MonoBehaviour
{
    // Start is called before the first frame update
    public int numLights = 5;
    public float lightRange = 10f;
    public float lightIntensity = 10f;
    void Start()
    {

        float angle = 0f;
        float angleStep = 360f / (64 / 2);
        float radius = 131f;

        for (int i = 0; i < (64 / 2); i++)
        {
            // GameObject cube = Object.Instantiate(cubePrototype, p, cubePrototype.transform.rotation) as GameObject;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            Vector3 posi = new Vector3(x, 30, z);
            // 创建Spot Light游戏对象
            GameObject spotLightObj = new GameObject("Spot Light " + i);
            // 添加Spot Light组件
            Light spotLight = spotLightObj.AddComponent<Light>();
            // 设置Spot Light的类型为Spot
            spotLight.type = LightType.Spot;
            // 设置Spot Light的范围和强度
            spotLight.range = 50f;
            spotLight.intensity = 10f;
            spotLight.color = Color.yellow;
            spotLight.lightmapBakeType = LightmapBakeType.Baked;
            // 设置Spot Light的位置和旋转
            spotLightObj.transform.position = posi;
            spotLightObj.transform.rotation = Quaternion.LookRotation(Vector3.down);
            angle += angleStep;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
