using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HughUtility;

public class UIObserver : MonoBehaviour, Observer
{
    public UIType ObserverNotifyCanvas(UIType type)
    {
        return type;
    }
}
