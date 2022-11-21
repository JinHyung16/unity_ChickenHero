using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowEvent : MonoBehaviour, IPointerDownHandler
{
    ///<summary>
    ///화면을 터치했을 때, 물건을 던지는 행위에 관련한 클래스
    ///</summary>

    private void Update()
    {
        //TouchScreen();
    }

    /// <summary>
    /// 제작해서 사용중인 터치 방식
    /// </summary>
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
    
    /// <summary>
    /// Unity에서 제공하는 IPointerDownHandler interface에 속한 함수
    /// Touch받을 GameObject는 반득시 Collider가 붙어있어야한다.
    /// </summary>
    /// <param name="pointer"> pointer 된 지점의 값을 넣어준다 </param>
    public void OnPointerDown(PointerEventData pointer)
    {
        if (pointer.pointerClick.gameObject.CompareTag("Enemy"))
        {
            //계란을 던진다.
        }
    }
}
