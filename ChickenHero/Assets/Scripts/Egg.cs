using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour
{
    /// <summary>
    /// Pooling 후 오브젝트의 활성화가 true가 될 때 호출
    /// </summary>
    private void OnEnable()
    {
        Debug.Log("생성 되었습니다.");
    }
}
