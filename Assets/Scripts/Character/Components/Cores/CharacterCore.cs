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
    INode _root;
    INode _inputParallel;
    INode _actionSelector;
    INode _animationSelector;

    private void Awake()
    {
        InitializeComponents();
        InitializeValues();
    }

    private void Start()
    {
        MakeNodes();
        StartCoroutines();
    }

    private void Update()
    {
        _root.Run();
    }

    private void StartCoroutines()
    {
        StartCoroutine(CameraZoomRoutine());
    }

    private void MakeNodes()
    {
        _inputParallel = 
            Parallel
            (
                Action(Input_ChangeCamView)
                ,Action(Input_SetCursorVisibleState)
                ,Action(Input_RotatePlayerCamera)
                ,Action(Input_CameraZoom)
                ,Action(Input_CalculateKeyMoveDir)
            );

        _actionSelector =
            Selector
            (
                Action(MoveByKeyboard)
            );

        _animationSelector =
            Selector
            (
                IfAction(PlayerIsWalking, PlayWalkAnimation)
                ,IfAction(PlayerIsRunning, PlayRunAnimation)
                ,Action(PlayIdleAnimation)
            );

        _root =
            Parallel
            (
                _inputParallel
                ,_actionSelector
                ,_animationSelector
            );
    }
}
