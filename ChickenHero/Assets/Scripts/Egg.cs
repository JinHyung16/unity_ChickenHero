using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Egg : MonoBehaviour
{
    /// <summary>
    /// Pooling �� ������Ʈ�� Ȱ��ȭ�� true�� �� �� ȣ��
    /// </summary>
    private void OnEnable()
    {
        Debug.Log("���� �Ǿ����ϴ�.");
    }
}
