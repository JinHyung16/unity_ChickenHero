using UnityEngine.SceneManagement;
using HughLibrary;

public class SceneController : LazySingleton<SceneController>
{
    /// <summary>
    /// Scene 전환을 관리하는 클래스
    /// </summary>
    ///<param name=”sceneName”>sceneName은 실제 Build에 올릴 Scene이름과, switch문에서 사용할 이름이 같아야합니다.</param>
    public void GoToScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Lobby":
                SceneManager.LoadScene(sceneName);
                break;
            case "MultiPlay":
                SceneManager.LoadScene(sceneName);
                break;
            case "SinglePlay":
                SceneManager.LoadScene(sceneName);
                break;
            default:
                break;
        }
    }
}
