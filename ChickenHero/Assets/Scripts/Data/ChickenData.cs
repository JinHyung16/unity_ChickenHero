using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerData", order = 2)]
public class ChickenData : ScriptableObject
{
    public Sprite playerSprite;
    public string chickenName;
    public int chickenHP;
}
