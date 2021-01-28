using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

// 날짜 : 2021-01-08 PM 9:22:10
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                        Core - Condition Nodes
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
    private bool CharacterIsWalking() => State.isWalking;
    private bool CharacterIsRunning() => State.isRunning;

    private bool OnTotalAttackCooldown() => Current.toolCooldown > 0f;
    private bool OnToolCooldown() => Current.toolCooldown > 0f;
    private bool OnRollingCooldown() => Current.rollCooldown > 0f;

    private bool MoveKeyDown() =>
        Input.GetKey(Key.moveForward) ||
        Input.GetKey(Key.moveBackward) ||
        Input.GetKey(Key.moveLeft) ||
        Input.GetKey(Key.moveRight);
    private bool RollKeyDown() => Input.GetKeyDown(Key.roll);
    private bool JumpKeyDown() => Input.GetKeyDown(Key.jump);
    private bool ChangeModeKeyDown() => Input.GetKeyDown(Key.changeBehaviorMode);
    private bool AttackKeyDown() => Input.GetMouseButtonDown((int)Key.attack);


    private bool CharacterIsEquippedMode()
        => State.behaviorMode.Equals(BehaviorMode.Equip) || State.behaviorMode.Equals(BehaviorMode.Witch);
    private bool CharacterIsWitchMode() => State.behaviorMode.Equals(BehaviorMode.Witch);
    private bool CharacterIsOnVehicleMode() => State.behaviorMode.Equals(BehaviorMode.OnVehicle);


    private bool CurrentIsFPCamera() => Current.cameraView == CameraViewOption.FirstPerson;
    private bool CurrentIsTPCamera() => Current.cameraView == CameraViewOption.ThirdPerson;

    #endregion
}