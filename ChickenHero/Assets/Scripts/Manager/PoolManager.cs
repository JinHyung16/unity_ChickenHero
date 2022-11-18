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
            EggDictionary.Add("egg", Instantiate(eggPrefab));
        }
    }
}
