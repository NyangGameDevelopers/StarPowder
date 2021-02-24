using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

using Extensions;

// 날짜 : 2021-01-08 PM 9:22:10
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                               States
    ***********************************************************************/
    #region .
    private bool CharacterIsUnableToMove()
        => State.isDead || State.isStunned || State.isBinded || State.isRolling;

    private bool CharacterIsDead() => State.isDead;
    private bool CharacterIsStunned() => State.isStunned;
    private bool CharacterIsBinded() => State.isBinded;
    private bool CharacterIsGrounded() => State.isGrounded;
    private bool CharacterIsJumping() => !State.isGrounded;

    private bool CharacterIsCasting() => Current.castDuration > 0f;
    private bool CharacterIsRolling() => State.isRolling;
    private bool CharacterIsMoving() => State.isMoving;
    private bool CharacterIsMovingOnGround() => State.isMoving && State.isGrounded;
    private bool CharacterIsRunning() => State.isRunning;
    private bool CharacterIsUsingPancake() => State.isUsingPancake;


    private bool CharacterIsHoldingTool() => Current.toolInHand != null;
    private bool OnToolCooldown() => Current.toolCooldown > 0f;
    private bool OnRollingCooldown() => Current.rollCooldown > 0f;


    private bool CharacterIsChangingMode() => Current.changeModeDuration > 0f;

    private bool CharacterIsNormalMode() => Current.behaviorMode.Equals(BehaviorMode.Normal);
    private bool CharacterIsBattleMode() => Current.behaviorMode.Equals(BehaviorMode.Battle);
    private bool CharacterIsWitchMode() => Current.behaviorMode.Equals(BehaviorMode.Witch);
    private bool CharacterIsOnVehicleMode() => Current.behaviorMode.Equals(BehaviorMode.OnVehicle);
    private bool CharacterIsBuildMode() => Current.behaviorMode.Equals(BehaviorMode.Build);


    private bool CurrentIsFPCamera() => Current.cameraView == CameraViewOption.FirstPerson;
    private bool CurrentIsTPCamera() => Current.cameraView == CameraViewOption.ThirdPerson;

    #endregion
    /***********************************************************************
    *                               Key Inputs
    ***********************************************************************/
    #region .
    // Mouse Buttons
    private bool MouseLeftKeyDown() => Binding[UserAction.MouseLeft].KeyDown();
    private bool MouseRightKeyDown() => Binding[UserAction.MouseRight].KeyDown();
    private bool MouseMiddleKeyDown() => Binding[UserAction.MouseMiddle].KeyDown();
    private bool MouseMiddleKeyUp() => Binding[UserAction.MouseMiddle].KeyUp();

    // Keyboards
    private bool AnyMoveKeysDown() =>
        Binding[UserAction.MoveForward].GetKey() ||
        Binding[UserAction.MoveBackward].GetKey() ||
        Binding[UserAction.MoveLeft].GetKey() ||
        Binding[UserAction.MoveRight].GetKey();
    private bool RollKeyDown() => Binding[UserAction.Dodge].KeyDown();
    private bool JumpKeyDown() => Binding[UserAction.Jump].KeyDown();
    private bool RidingKeyDown() => Binding[UserAction.Temp_RideOnVehicle].KeyDown();

    private bool PancakeKeyDown() => Binding[UserAction.ShowToolSet].KeyDown();
    private bool PancakeKeyUp() => Binding[UserAction.ShowToolSet].KeyUp();

    private bool ChangeBehaviorModeKeyDown() => Binding[UserAction.Toggle_ChangeBehaviorMode].KeyDown();
    private bool ChangeCamViewKeyDown() => Binding[UserAction.Toggle_ChangeCameraView].KeyDown();
    private bool ShowCursorKeyDown() => Binding[UserAction.Toggle_ShowCursor].KeyDown();

    #endregion
}