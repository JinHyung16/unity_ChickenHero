using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowEvent : MonoBehaviour
{
    ///<summary>
    ///ȭ���� ��ġ���� ��, ������ ������ ������ ������ Ŭ����
    ///</summary>

    private void Update()
    {
        TouchScreen();
    }

    private void TouchScreen()
    {
        if (Input.touchCount > 0)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (Physics2D.Raycast(touchPos, (Input.GetTouch(0).position)).collider.CompareTag("Enemy"))
            {
                //��� ������
            }
        }
    }
}
