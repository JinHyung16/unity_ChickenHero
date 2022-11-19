using System.Collections.Generic;
using UnityEngine;
using HughLibrary;

public class PoolManager : Singleton<PoolManager>
{
    public GameObject eggPrefab;
    public int eggCount = 0;

    private Dictionary<string, GameObject> EggDictionary;

    private void Awake()
    {
        InitDictionary();
        InitPrefabCount();
        Pooling();
    }

    private void Start()
    {
    }

    private void InitDictionary()
    {
        EggDictionary = new Dictionary<string, GameObject>();
    }

    private void InitPrefabCount() 
    {
        eggCount = 50;
    }

    private void Pooling()
    {
        for (int i = 0; i < eggCount; i++)
        {
            var egg = Instantiate(eggPrefab);
            egg.SetActive(false);
            egg.tag = "egg";
            EggDictionary.Add("egg", egg);
        }
    }

    public GameObject GetObject(string name)
    {
        GameObject obj = null;
        switch (name)
        {
            case "egg":
                EggDictionary.TryGetValue("egg", out obj);
                break;
        }

        return obj;
    }
}
