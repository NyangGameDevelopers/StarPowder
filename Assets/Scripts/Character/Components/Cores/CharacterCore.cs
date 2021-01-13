using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

using static Rito.BehaviorTree.NodeHelper;

// 2021-01-07 21:56
// 작성자 : Rito

/// <summary> 플레이어 캐릭터 핵심 컴포넌트 스크립트 </summary>
[RequireComponent(typeof(Rigidbody))]
public partial class CharacterCore : MonoBehaviour
{
    private void Awake()
    {
        MakeBehaviorNodes();
        InitializeComponents();
        InitializeValues();
    }

    private void Start()
    {
        StartCoroutines();
    }

    private void Update()
    {
        _currentBehavior.Run();

        if (Input.GetKeyDown(KeyCode.U)) DoStun(2f);
        if (Input.GetKeyDown(KeyCode.I)) DoBind(2f);
        if (Input.GetKeyDown(KeyCode.O)) Die();
        if (Input.GetKeyDown(KeyCode.P)) Revive();
    }

    private void StartCoroutines()
    {
        StartCoroutine(CameraZoomRoutine());
    }
}
