using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                         Core - Data Variables
    ***********************************************************************/
    public CooldownInfo Cooldown => _cooldown;
    public DurationInfo Duration => _duration;

    public KeyOption Key => _keyOption;
    public SpeedOption Speed => _speedOption;
    public MoveOption Move => _moveOption;
    public SpecialSkillOption Skill => _skillOption;

    public AnimationNameSet_ AnimationName => _animationNameSet;

    public FirstPersonCameraOption FPCamOption => _firstPersonCameraOption;
    public ThirdPersonCameraOption TPCamOption => _thirdPersonCameraOption;

    /***********************************************************************
    *                               Cooldowns
    ***********************************************************************/
    #region .
    [Serializable]
    public class CooldownInfo
    {
        [Tooltip("구르기 끝난 직후로부터 재사용 쿨타임")]
        public float roll = 0.5f;
        [Tooltip("전체 공격 쿨타임")]
        public float attack = 1.5f;
        [Tooltip("첫번째 공격 이후 두번째 공격을 하기 위한 대기시간")]
        public float firstAttack = 0.3f;
    }
    //[SerializeField, Tooltip("행동 쿨타임들")]
    private CooldownInfo _cooldown = new CooldownInfo();

    #endregion
    /***********************************************************************
    *                               Durations
    ***********************************************************************/
    #region .
    [Serializable]
    public class DurationInfo
    {
        [Tooltip("구르기 및 애니메이션 지속시간")]
        public float roll = 1.0f;
        [Tooltip("두 번째 공격 모션 이어서 재생할 수 있는 허용 시간")]
        public float secondAttackChance = 0.4f;
    }
    //[SerializeField, Tooltip("지속시간(애니메이션 포함)")]
    private DurationInfo _duration = new DurationInfo();

    #endregion
    /***********************************************************************
    *                               Animation Names
    ***********************************************************************/
    #region .
    [Serializable]
    public class AnimationNameSet_
    {
        // 공통
        public string none = "NONE";
        public string roll = "ROLL";
        public string bind = "BIND";
        public string stun = "STUN";
        public string die  = "DIE";

        // 평시
        public string idle = "IDLE";
        public string move = "MOVE";

        // 근접 무기
        public string battleIdle = "BATTLE_IDLE";
        public string battleMove = "BATTLE_MOVE";

        // 마녀
        public string witch = "WITCH";

        // 감정표현
        public string emotion0 = "EMOTION_CLAP";

        //====================== Upper 레이어(상체) 애니메이션 ===================
        public string upperBattleAttack0 = "BATTLE_ATTACK_0";
        public string upperBattleAttack1 = "BATTLE_ATTACK_1";
    }
    //[SerializeField, Tooltip("연결된 애니메이터의 각 애니메이션 이름 정확히 등록")]
    private AnimationNameSet_ _animationNameSet = new AnimationNameSet_();

    #endregion
    /***********************************************************************
    *                               Key Options
    ***********************************************************************/
    #region .
    [Serializable]
    public class KeyOption
    {
        public KeyCode moveForward  = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLeft  = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;

        [Space]
        public KeyCode run = KeyCode.LeftControl;
        public KeyCode roll = KeyCode.LeftShift;
        public KeyCode jump = KeyCode.Space;

        [Space]
        [Tooltip("마우스 커서 보이기/감추기 토글")]
        public KeyCode showCursorToggle = KeyCode.LeftAlt;

        [Tooltip("1인칭 / 3인칭 카메라 변경 토글")]
        public KeyCode changeViewToggle = KeyCode.BackQuote;

        [Space]
        [Tooltip("일반모드 / 전투모드 변경")]
        public KeyCode changeBehaviorMode = KeyCode.Tab;
        public MouseButton attack = MouseButton.Left;
    }
    [SerializeField]
    private KeyOption _keyOption = new KeyOption();

    #endregion
    /***********************************************************************
    *                               Speed Options
    ***********************************************************************/
    #region .
    [Serializable]
    public class SpeedOption
    {
        [Range(1f, 2f), Tooltip("캐릭터 공격속도")]
        public float attackSpeed = 1f;
        [Range(1f, 20f), Tooltip("캐릭터 이동속도")]
        public float moveSpeed = 3f;
        [Range(1f, 3f), Tooltip("달리기 이동속도 배수(달리기 이동속도 = 캐릭터 이동속도 X 달리기 배수)")]
        public float runSpeedMultiplier = 1.5f;

    }
    [SerializeField]
    private SpeedOption _speedOption = new SpeedOption();

    #endregion
    /***********************************************************************
    *                               Move Options
    ***********************************************************************/
    #region .
    [Serializable]
    public class MoveOption
    {
        [Range(1f, 5f), Tooltip("구르기 이동거리")]
        public float rollDistance = 3f;

        [Range(1f, 10f), Tooltip("점프점프")]
        public float jumpForce = 5.5f;
    }
    [SerializeField]
    private MoveOption _moveOption = new MoveOption();

    #endregion
    /***********************************************************************
    *                             Camera Options
    ***********************************************************************/
    #region .

    // 상속용
    public abstract class CameraOption
    {
        [Range(1f, 20f), Space, Tooltip("카메라 상하좌우 회전 속도")]
        public float rotationSpeed = 2f;
        [Range(-90f, 0f), Tooltip("올려다보기 제한 각도")]
        public float lookUpDegree = -60f;
        [Range(0f, 60f), Tooltip("내려다보기 제한 각도")]
        public float lookDownDegree = 45f;
    }

    [Serializable]
    public class FirstPersonCameraOption : CameraOption
    {
        ///// <summary> 리깅된 머리 트랜스폼 연결 </summary>
        //public Transform headTran; // 직접 조작 안됨
    }
    [SerializeField]
    private FirstPersonCameraOption _firstPersonCameraOption = new FirstPersonCameraOption();

    [Serializable]
    public class ThirdPersonCameraOption : CameraOption
    {
        [Range(0f, 3.5f), Space, Tooltip("줌 확대 최대 거리")]
        public float zoomInDistance = 3f;
        [Range(0f, 5f), Tooltip("줌 축소 최대 거리")]
        public float zoomOutDistance = 3f;
        [Range(1f, 10f), Tooltip("줌 속도")]
        public float zoomSpeed = 5f;
    }
    [SerializeField]
    private ThirdPersonCameraOption _thirdPersonCameraOption = new ThirdPersonCameraOption();

    #endregion
    /***********************************************************************
    *                             Special Skills
    ***********************************************************************/
    #region .
    [Serializable]
    public class SpecialSkillOption
    {
        [Tooltip("공중에서 한번 더 점프")]
        public bool doubleJump = false;
        [Tooltip("공중 구르기")]
        public bool airTumbling = false;
    }
    [SerializeField]
    private SpecialSkillOption _skillOption = new SpecialSkillOption();

    #endregion
}
