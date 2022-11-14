using UnityEngine;
using UnityEngine.SceneManagement;
using HughLibrary;

public class SceneController : LazySingleton<SceneController>
{
    public void GoToScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Lobby":
                SceneManager.LoadScene(sceneName);
                break;
            default:
#if UNITY_EDITOR
                Debug.Log("<color=red><b> Do not exist scene !!! </b></color>");
#endif
                break;
        }
    }
}
