using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowEvent : MonoBehaviour, IPointerDownHandler
{
    ///<summary>
    ///ȭ���� ��ġ���� ��, ������ ������ ������ ������ Ŭ����
    ///</summary>

    private void Update()
    {
        //TouchScreen();
    }

    /// <summary>
    /// �����ؼ� ������� ��ġ ���
    /// </summary>
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
    
    /// <summary>
    /// Unity���� �����ϴ� IPointerDownHandler interface�� ���� �Լ�
    /// Touch���� GameObject�� �ݵ�� Collider�� �پ��־���Ѵ�.
    /// </summary>
    /// <param name="pointer"> pointer �� ������ ���� �־��ش� </param>
    public void OnPointerDown(PointerEventData pointer)
    {
        if (pointer.pointerClick.gameObject.CompareTag("Enemy"))
        {
            //����� ������.
        }
    }
}
