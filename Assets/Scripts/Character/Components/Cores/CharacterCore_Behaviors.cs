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
                 // 탑승 모드에서는 3인칭 뷰만 지원
                IfNotAction(CharacterIsOnVehicleMode, Input_ChangeCamView),
                Action(Input_SetCursorVisibleState),
                Action(Input_RotatePlayer),
                Action(Input_CameraZoom),
                Action(Input_CalculateMoveDirection),

                // 임시 : 무기 변경 토글
                IfAction(() => !OnToolCooldown() && 
                                CharacterIsEquippedMode() && 
                                Input.GetKeyDown(KeyCode.Z),
                         () => {
                             ToolBox.SwitchNextTool(LeftHand, RightHand);
                             Current.tool = ToolBox.CurrentTool;
                             SetBehaviorMode(BehaviorMode.Equip);
                             }),

                Sequence // 행동 모드 변경 : 일반 <-> 도구 사용 <-> 마녀
                (
                    NotCondition(CharacterIsDead),
                    NotCondition(CharacterIsStunned),
                    NotCondition(OnTotalAttackCooldown),
                    NotCondition(CharacterIsOnVehicleMode),
                    Action(Input_ChangeBehaviorMode)
                ),

                Selector // Actions
                (

                    Sequence // 캐스팅 도중 
                    (
                        Condition(CharacterIsCasting),

                        Sequence
                        (
                            Selector // 이런 액션을 취하면
                            (
                                Condition(MoveKeyDown),
                                Condition(JumpKeyDown),
                                Condition(AttackKeyDown),
                                Condition(RollKeyDown),
                                
                                Condition(CharacterIsDead),
                                Condition(CharacterIsStunned)
                            ),
                            Action(CancelCasting) // 캐스팅 취소
                        )
                    ),

                    Condition(CharacterIsDead),
                    Condition(CharacterIsStunned),
                    Condition(CharacterIsRolling),

                    // 탑승 키 누르면 탑승
                    Sequence
                    (
                        NotCondition(OnToolCooldown), // 도구 사용 쿨타임 중인 경우 점프 불가
                        NotCondition(CharacterIsUnableToMove),
                        NotCondition(CharacterIsJumping),
                        Condition(() => Input.GetKeyDown(Key.rideOnVehicle)),
                        Action(ToggleVehicleState)
                    ),

                    Sequence
                    (
                        Condition(CharacterIsOnVehicleMode),
                        Action(MoveWASD)
                    ), // 탑승하면 얌전히 이동만 가능 =======================================

                    // 도구 사용(무기 공격 포함)
                    Sequence
                    (
                        Condition(CharacterIsEquippedMode),
                        Condition(CharacterIsGrounded),
                        IfAction(AttackKeyDown, UseToolAndPlayAnimation)
                    ),

                    Condition(CharacterIsBinded),
                    // 속박시 아래 행동 불가능 ===============================================

                    // 점프
                    Sequence
                    (
                        NotCondition(OnToolCooldown), // 도구 사용 쿨타임 중인 경우 점프 불가
                        IfAction(JumpKeyDown, Jump),
                        Action(ResetUpperAnimation),
                        Action(PlayIdleAnimation)
                    ),

                    // 구르기
                    Sequence
                    (
                        NotCondition(CharacterIsWitchMode),
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