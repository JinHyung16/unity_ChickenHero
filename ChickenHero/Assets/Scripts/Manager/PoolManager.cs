using System.Collections.Generic;
using UnityEngine;
using HughLibrary;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private GameObject eggPrefab;
    [HideInInspector] public int eggCount = 0;

    private List<GameObject> EggStack;

    private void Awake()
    {
        InitDictionary();
        InitPrefabCount();
        Pooling();
    }

    private void InitPrefab()
    {
        if (eggPrefab == null)
        {
            eggPrefab = Resources.Load<GameObject>("Egg/Egg");
        }
    }
    private void InitDictionary()
    {
        EggStack = new List<GameObject>();
    }

    private void InitPrefabCount() 
    {
        eggCount = 50;
    }

    private void Pooling()
    {
        for (int i = 0; i < eggCount; i++)
        {
            GameObject egg = Instantiate(eggPrefab);
            egg.SetActive(false);
            egg.tag = "Egg";
            DontDestroyOnLoad(egg);
        }
    }

    public GameObject GetPrefab(string name)
    {
        switch (name)
        {
            case "egg":
                break;
        }
        return null;
    }
}
