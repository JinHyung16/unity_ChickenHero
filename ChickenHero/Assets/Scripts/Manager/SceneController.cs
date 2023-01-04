using UnityEngine.SceneManagement;
using HughGeneric;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

sealed class SceneController : Singleton<SceneController>
{
    [SerializeField] private GameObject loadingCanvas;
    private CanvasGroup loadingCanvasGroup;

    [SerializeField] private TMP_Text loadingGaugeTxt;

    //���� �ε� �ð�
    private float minLoadDuration = 4.0f;

    //�ε� �ð� �� �ּڰ��� ���� ����
    private float loadRatio;

    //fake �ε� �ð�
    private float fakeLoadTime;
    private float fakeLoadRatio;

    private void Start()
    {
        loadingCanvasGroup = loadingCanvas.GetComponent<CanvasGroup>();
        loadingCanvasGroup.alpha = 1.0f;

        loadingCanvas.SetActive(false);
    }

    /// <summary>
    /// Scene ��ȯ�� ȣ��Ǵ� �񵿱� �Լ�
    /// Build Settings �κп� �ش� �̸��� Scene�� �ö� �־���Ѵ�.
    /// </summary>
    /// <param name="sceneName"> �ε��� Scene �̸��� �޴´�</param>
    /// <returns> UniTaskVoid�� �����ϴµ� �̴� async void�� ���� �׷��� ���ɸ� ���� </returns>
    public async UniTaskVoid GoToScene(string sceneName)
    {
        loadingCanvas.SetActive(true);

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
            loadRatio = Mathf.Min(loadSceneAsync.progress + 0.1f, fakeLoadRatio);

            //text update�ϱ�
            loadingGaugeTxt.text = (loadRatio * 100).ToString("F0") + "%";
            
            if (loadRatio >= 1.0f)
            {
                loadingCanvasGroup.DOFade(1, 1.0f);
                loadingCanvas.SetActive(false);
                break;
            }

            await UniTask.Yield();
        }

        loadSceneAsync.allowSceneActivation = true;
    }
}
