using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ΰ��� �г�(ĵ����)�� ���� ������ Application�� ������ �� interface
/// </summary>
public interface IGameStateBasicModule
{
    void Init();
    void OnEnter(); //start �Լ� ����
    void AdvanceTime(float dt_sec); //update �Լ� ����
    void OnExit();
    void Dispose();
}