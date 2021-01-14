using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-08 PM 9:32:38
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /**************************************************************
     *                         Core - States
     **************************************************************/
    [Serializable]
    public class CharacterState
    {
        /// <summary> 일반 / 전투 모드 </summary>
        public BehaviorMode behaviorMode = BehaviorMode.None;

        /// <summary> 죽었니 </summary>
        public bool isDead;

        /// <summary> 기절 </summary>
        public bool isStunned;

        /// <summary> 속박 </summary>
        public bool isBinded;

        /// <summary> 땅에 붙어 있음 </summary>
        public bool isGrounded;

        /// <summary> 구르는 중 </summary>
        public bool isRolling;

        /// <summary> 캐릭터가 걷거나 뛰고 있는지 여부 </summary>
        public bool isMoving;

        /// <summary> 캐릭터가 걷고 있는지 여부 </summary>
        public bool isWalking;

        /// <summary> 캐릭터가 뛰고 있는지 여부 </summary>
        public bool isRunning;

        /// <summary> 캐릭터가 공격 모션 중인지 여부 </summary>
        public bool isAttacking;

        /// <summary> 캐릭터 변신중 </summary>
        public bool isChangingMode;

        /// <summary> 현재 커서가 보이는지 여부 </summary>
        public bool isCursorVisible;

        /// <summary> 현재 캐릭터 이동 방향 </summary>
        public MoveDirection currentMoveDirection = MoveDirection.None;

        /// <summary> 현재 선택된 카메라 뷰 </summary>
        public CameraViewOption currentView = CameraViewOption.ThirdPerson;
    }
    [SerializeField]
    public CharacterState _state = new CharacterState();
    public CharacterState State => _state;

    [Serializable]
    public class CurrentStateValues
    {
        // Cooldowns : 쿨타임
        public float rollCooldown;
        public float attackCooldown;

        // Durations : 진행(지속) 시간
        public float rollDuration;
        public float stunDuration;
        public float bindDuration;
        public float attackMotionDuration;
        public float continousAttackDuration;
        public float changeModeDuration; // 일반 <-> 배틀 모드 전환 중

        /// <summary> 지면으로부터의 거리 </summary>
        public float distFromGround;

        /// <summary> 근접공격 애니메이션 재생할 차례 </summary>
        public int attackAnimationIndex = 0;

        /// <summary> 더블점프를 썼는지 여부 </summary>
        public bool doubleJumped;
    }
    [SerializeField]
    private CurrentStateValues _currentStates = new CurrentStateValues();
    public CurrentStateValues Current => _currentStates;
}