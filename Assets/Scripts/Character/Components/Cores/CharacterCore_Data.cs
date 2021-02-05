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

    public Dictionary<UserAction, KeyCode> Binding => SettingCore.I.Binding.Bindings;
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
        public string bind = "BIND";
        public string stun = "STUN";
        public string die  = "DIE";

        // 평시
        public string idle = "IDLE";
        public string move = "MOVE";
        public string roll = "ROLL";

        // 오른손 도구를 들고 있는 경우
        public string rightHandIdle = "RIGHT_HAND_IDLE";
        public string rightHandMove = "RIGHT_HAND_MOVE";
        public string rightHandRoll = "RIGHT_HAND_ROLL";

        // 양손 도구 들고 있는 경우
        public string doubleHandIdle = "DOUBLE_HAND_IDLE";
        public string doubleHandMove = "DOUBLE_HAND_MOVE";
        public string doubleHandRoll = "DOUBLE_HAND_ROLL";

        // 양손 각각 도구를 들고 있는 경우
        public string twoHandIdle = "TWO_HAND_IDLE";
        public string twoHandMove = "TWO_HAND_MOVE";
        public string twoHandRoll = "TWO_HAND_ROLL";

        // 마녀
        public string witch = "WITCH";

        // 탑승
        public string onVehicle = "ON_VEHICLE";

        // 감정표현
        public string emotion0 = "EMOTION_CLAP";

        //====================== Upper 레이어(상체) 애니메이션 ===================
        public string[] rightHandAttacks = new string[]
            {
                "",
                "RIGHT_HAND_ATTACK_1",
                "RIGHT_HAND_ATTACK_2",
                "RIGHT_HAND_ATTACK_3"
            };

        public string[] doubleHandAttacks = new string[]
            {
                "",
                "DOUBLE_HAND_ATTACK_1",
                "DOUBLE_HAND_ATTACK_2",
                "DOUBLE_HAND_ATTACK_3"
            };

        public string[] twoHandAttacks = new string[]
            {
                "",
                "TWO_HAND_ATTACK_1",
                "TWO_HAND_ATTACK_2",
                "TWO_HAND_ATTACK_3"
            };
    }
    //[SerializeField, Tooltip("연결된 애니메이터의 각 애니메이션 이름 정확히 등록")]
    private AnimationNameSet_ _animationNameSet = new AnimationNameSet_();

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
        [Range(0f, 75f), Tooltip("내려다보기 제한 각도")]
        public float lookDownDegree = 75f;
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
