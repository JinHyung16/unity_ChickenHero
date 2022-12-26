using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PowerCardData", order = 1)]
public class PowerCardData : ScriptableObject
{
    public Sprite powerCardSprite;
    public string powerCardName;
    public string powerCardDescription;
    public int cardPower;
    public int weight;
}
