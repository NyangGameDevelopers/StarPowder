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

    private bool CharacterIsRolling() => State.isRolling;
    private bool CharacterIsMoving() => State.isMoving;
    private bool CharacterIsMovingOnGround() => State.isMoving && State.isGrounded;
    private bool CharacterIsWalking() => State.isWalking;
    private bool CharacterIsRunning() => State.isRunning;

    private bool OnTotalAttackCooldown() => Current.attackCooldown > 0f;
    private bool OnFirstAttackCooldown() => Current.firstAttackCooldown > 0f;
    private bool OnRollingCooldown() => Current.rollCooldown > 0f;

    private bool RollKeyDown() => Input.GetKeyDown(Key.roll);
    private bool JumpKeyDown() => Input.GetKeyDown(Key.jump);
    private bool ChangeModeKeyDown() => Input.GetKeyDown(Key.changeBehaviorMode);
    private bool AttackKeyDown() => Input.GetMouseButtonDown((int)Key.attack);


    private bool CharacterIsOnBattleMode() => State.behaviorMode.Equals(BehaviorMode.Battle);
    private bool CharacterIsOnWitchMode() => State.behaviorMode.Equals(BehaviorMode.Witch);


    private bool CurrentIsFPCamera() => State.currentView == CameraViewOption.FirstPerson;
    private bool CurrentIsTPCamera() => State.currentView == CameraViewOption.ThirdPerson;

    #endregion
}