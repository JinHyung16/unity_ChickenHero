using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인게임 패널(캔버스)에 따라 켜지는 Application이 가져야 할 interface
/// </summary>
public interface IGameStateBasicModule
{
    void Init();
    void OnEnter(); //start 함수 역할
    void AdvanceTime(float dt_sec); //update 함수 역할
    void OnExit();
    void Dispose();
}