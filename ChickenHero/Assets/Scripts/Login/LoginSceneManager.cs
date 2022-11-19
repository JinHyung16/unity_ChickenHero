using UnityEngine;
using HughLibrary;

public class LoginSceneManager : MonoBehaviour
{
    /// <summary>
    /// Prefab���� ���� �ν����� â���� ���� Button�� ��� �Ҵ����̴�
    /// </summary>
    
    public GameObject LoginCanvas;

    private void Awake()
    {
        if (LoginCanvas == null)
        {
            LoginCanvas = Resources.Load<GameObject>("LoginCanvas/Canvas");
        }
    }

    private void Start()
    {
        LoginCanvas.transform.parent = this.gameObject.transform;
    }
}
