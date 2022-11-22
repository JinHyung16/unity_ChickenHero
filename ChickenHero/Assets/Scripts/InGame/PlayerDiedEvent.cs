using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity Event를 이용한 player die시 실행
/// </summary>
[System.Serializable]
public class PlayerDiedEvent : UnityEvent<GameObject>
{
}
