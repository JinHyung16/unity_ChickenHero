using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    public void CameraShaking()
    {
        CameraShakeEffect().Forget();
    }

    private async UniTaskVoid CameraShakeEffect()
    {
        Vector3 originPosition = cameraTransform.localPosition;
        float elapsedTime = 0.0f;
        
        while (elapsedTime < 0.5f)
        {
            float xOffset = UnityEngine.Random.Range(-0.2f, 0.2f) * 0.2f;
            float yOffset = UnityEngine.Random.Range(-0.2f, 0.2f) * 0.2f;

            cameraTransform.localPosition = new Vector3(xOffset, yOffset, originPosition.z);

            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }
        cameraTransform.localPosition = originPosition;
    }
}
