using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public KeyCode poolingKey;

    private void Start()
    {
        poolingKey = KeyCode.A;
    }
    private void Update()
    {
        if (Input.GetKeyDown(poolingKey))
        {
            PoolTest();
        }
        
    }

    private void PoolTest()
    {
        GameObject testEgg = PoolManager.GetInstance.GetPrefab("egg");
        testEgg.transform.position = new Vector2(0, 5);
        testEgg.SetActive(true);
    }
}
