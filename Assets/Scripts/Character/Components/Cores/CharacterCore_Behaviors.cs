using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

// 날짜 : 2021-01-12 AM 1:16:19
// 작성자 : Rito

using static Rito.BehaviorTree.NodeHelper;

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                               Behavior Variables
    ***********************************************************************/
    #region .

    private INode _currentBehavior;

    #endregion
    /***********************************************************************
    *                               Behavior Assemble
    ***********************************************************************/
    #region .
    private void MakeBehaviorNodes()
    {
        INode root =
            Parallel
            (
                // Updates
                Action(DeclineCooldownDurationDeltatime),
                Action(CheckDistanceFromGround),

                // Input Actions
                Action(Input_ChangeCamView),
                Action(Input_SetCursorVisibleState),
                Action(Input_RotatePlayer),
                Action(Input_CameraZoom),
                Action(Input_CalculateMoveDirection),

                Sequence // 무기 교체
                (
                    NotCondition(CharacterIsDead),
                    NotCondition(CharacterIsStunned),
                    NotCondition(OnTotalAttackCooldown),
                    Action(Input_ChangeBehaviorMode)
                ),

                Selector // Actions
                (
                    Condition(CharacterIsDead),
                    Condition(CharacterIsStunned),
                    Condition(CharacterIsRolling),

                    // Attack
                    Sequence
                    (
                        Condition(CharacterIsOnBattleMode),
                        Condition(CharacterIsGrounded),
                        IfAction(AttackKeyDown, AttackAndPlayAnimation)
                    ),

                    Condition(CharacterIsBinded),

                    // Jump
                    Sequence
                    (
                        NotCondition(OnFirstAttackCooldown),
                        IfAction(JumpKeyDown, Jump),
                        Action(ResetUpperAnimation),
                        Action(PlayIdleAnimation)
                    ),

                    // Roll
                    Sequence
                    (
                        NotCondition(CharacterIsOnWitchMode),
                        Condition(RollKeyDown),
                        Action(RollWASD)
                    ),

                    Action(MoveWASD)
                ),

                Selector // Animations
                (
                    Condition(CharacterIsDead),
                    IfAction(CharacterIsStunned, PlayResetAndStunAnimation),
                    IfAction(CharacterIsBinded,  PlayBindAnimation),
                    IfAction(CharacterIsRolling, PlayRollAnimation),
                    IfAction(CharacterIsMovingOnGround, PlayMoveAnimation),
                    Action(PlayIdleAnimation)
                )
            );

        _currentBehavior = root;
    }

    #endregion
}