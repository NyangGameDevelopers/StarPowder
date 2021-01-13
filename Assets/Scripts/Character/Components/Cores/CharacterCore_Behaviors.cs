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
        INode ordinary =
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
                IfNotAction(CharacterIsUnableToMove, Input_ChangeBehaviorMode),

                Selector // Actions
                (
                    Condition(CharacterIsDead),
                    Condition(CharacterIsStunned),
                    Condition(CharacterIsBinded),
                    Condition(CharacterIsRolling),

                    Sequence
                    (
                        NotCondition(OnAttackCooldown),
                        IfAction(JumpKeyDown, Jump),
                        Action(PlayUpperIdleAnimation)
                    ),

                    IfAction(RollKeyDown, RollWASD),

                    Sequence
                    (
                        Condition(CharacterIsGrounded),
                        IfAction(AttackKeyDown, Attack)
                    ),

                    Action(MoveWASD)
                ),
                Selector // Animations
                (
                    Condition(CharacterIsDead),
                    IfAction(CharacterIsStunned, PlayStunAnimation),
                    IfAction(CharacterIsBinded,  PlayBindAnimation),
                    IfAction(CharacterIsRolling, PlayRollAnimation),

                    Condition(CharacterIsPlayingAttackMotion),
                    IfAction(CharacterIsMovingOnGround, PlayMoveAnimation),
                    Action(PlayIdleAnimation)
                )
            );

        INode meleeWeapon =
            Parallel
            (

            );

        INode rangedWeapon =
            Parallel
            (

            );

        _currentBehavior = ordinary;
    }

    #endregion
}