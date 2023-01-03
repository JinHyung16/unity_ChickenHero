using UnityEngine.SceneManagement;
using HughGeneric;
using Cysharp.Threading.Tasks;
using UnityEngine;

sealed class SceneController : LazySingleton<SceneController>
{
    //���� �ε� �ð�
    private float minLoadDuration = 4.0f;

    //�ε� �ð� �� �ּڰ��� ���� ����
    public float LoadRatio { get; private set; }

    //fake �ε� �ð�
    private float fakeLoadTime;
    private float fakeLoadRatio;

    /// <summary>
    /// Scene ��ȯ�� ȣ��Ǵ� �񵿱� �Լ�
    /// Build Settings �κп� �ش� �̸��� Scene�� �ö� �־���Ѵ�.
    /// </summary>
    /// <param name="sceneName"> �ε��� Scene �̸��� �޴´�</param>
    /// <returns> UniTaskVoid�� �����ϴµ� �̴� async void�� ���� �׷��� ���ɸ� ���� </returns>
    public async UniTaskVoid GoToScene(string sceneName)
    {
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
        if (loadSceneAsync == null)
        {
            return;
        }

        loadSceneAsync.allowSceneActivation = false;
        while (!loadSceneAsync.isDone)
        {
            //fake �ε� �ð� ����ϱ�
            fakeLoadTime += Time.deltaTime;
            fakeLoadRatio = fakeLoadTime / minLoadDuration;

            //���� �ε� �ð��� fake �ε� �ð� �� �ּڰ����� �ε��� �����ϱ�
            LoadRatio = Mathf.Min(loadSceneAsync.progress + 0.1f, fakeLoadRatio);

            if (LoadRatio >= 1.0f)
            {
                break;
            }

            await UniTask.Yield();
        }

        loadSceneAsync.allowSceneActivation = true;
    }
}
