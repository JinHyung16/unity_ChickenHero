using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowEvent : MonoBehaviour
{
    ///<summary>
    ///화면을 터치했을 때, 물건을 던지는 행위에 관련한 클래스
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
                //계란 던지기
            }
        }
    }
}
