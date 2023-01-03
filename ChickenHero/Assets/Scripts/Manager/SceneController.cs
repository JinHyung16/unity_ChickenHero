using UnityEngine.SceneManagement;
using HughGeneric;
using Cysharp.Threading.Tasks;
using UnityEngine;

sealed class SceneController : LazySingleton<SceneController>
{
    //실제 로딩 시간
    private float minLoadDuration = 4.0f;

    //로딩 시간 중 최솟값을 담을 변수
    public float LoadRatio { get; private set; }

    //fake 로딩 시간
    private float fakeLoadTime;
    private float fakeLoadRatio;

    /// <summary>
    /// Scene 전환시 호출되는 비동기 함수
    /// Build Settings 부분에 해당 이름의 Scene이 올라가 있어야한다.
    /// </summary>
    /// <param name="sceneName"> 로딩할 Scene 이름을 받는다</param>
    /// <returns> UniTaskVoid를 리턴하는데 이는 async void와 동일 그러나 성능면 우위 </returns>
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
            //fake 로딩 시간 계산하기
            fakeLoadTime += Time.deltaTime;
            fakeLoadRatio = fakeLoadTime / minLoadDuration;

            //실제 로딩 시간과 fake 로딩 시간 중 최솟값으로 로딩률 지정하기
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
