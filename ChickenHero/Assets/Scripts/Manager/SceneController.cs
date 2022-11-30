using UnityEngine.SceneManagement;
using HughGeneric;

sealed class SceneController : LazySingleton<SceneController>
{
    /// <summary>
    /// Scene ��ȯ�� �����ϴ� Ŭ����
    /// </summary>
    ///<param name=��sceneName��>sceneName�� ���� Build�� �ø� Scene�̸���, switch������ ����� �̸��� ���ƾ��մϴ�.</param>
    public void GoToScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Login":
                SceneManager.LoadScene(sceneName);
                break;
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
