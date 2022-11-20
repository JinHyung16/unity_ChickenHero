using UnityEngine;
using HughGeneric;

public class LoginSceneManager : MonoBehaviour
{
    /// <summary>
    /// Prefab으로 만들어서 인스펙터 창에서 직접 Button에 기능 할당중이다
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
