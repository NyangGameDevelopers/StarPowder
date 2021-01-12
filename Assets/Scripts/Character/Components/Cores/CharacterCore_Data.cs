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
    public KeyOption Key => _keyOption;
    public MoveOptionInfo Move => _moveOption;
    public AnimationNameInfo AnimationName => _animationName;

    public CameraOptionFirstPerson FPCamOption => _firstPersonCameraOption;
    public CameraOptionThirdPerson TPCamOption => _thirdPersonCameraOption;

    /***********************************************************************
    *                               Animations
    ***********************************************************************/
    #region .
    [Serializable]
    public class AnimationNameInfo
    {
        public string idle = "IDLE";
        public string walk = "WALK";
        public string run = "RUN";
    }
    [SerializeField, Tooltip("연결된 애니메이터의 각 애니메이션 이름 정확히 등록")]
    private AnimationNameInfo _animationName = new AnimationNameInfo();

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

        public KeyCode run = KeyCode.LeftShift;

        [Tooltip("마우스 커서 보이기/감추기 토글")]
        public KeyCode showCursorToggle = KeyCode.LeftAlt;

        [Tooltip("1인칭 / 3인칭 카메라 변경 토글")]
        public KeyCode changeViewToggle = KeyCode.Tab;
    }
    [SerializeField]
    private KeyOption _keyOption = new KeyOption();

    #endregion
    /***********************************************************************
    *                               Move Options
    ***********************************************************************/
    #region .
    [Serializable]
    public class MoveOptionInfo
    {
        [Range(1f, 20f), Tooltip("캐릭터 이동속도")]
        public float moveSpeed = 3f;
        [Range(1f, 3f), Tooltip("달리기 이동속도 배수(달리기 이동속도 = 캐릭터 이동속도 X 달리기 배수)")]
        public float runSpeedMultiplier = 1.5f;
    }
    [SerializeField]
    private MoveOptionInfo _moveOption = new MoveOptionInfo();

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
    public class CameraOptionFirstPerson : CameraOption
    {
        ///// <summary> 리깅된 머리 트랜스폼 연결 </summary>
        //public Transform headTran; // 직접 조작 안됨
    }
    [SerializeField]
    private CameraOptionFirstPerson _firstPersonCameraOption = new CameraOptionFirstPerson();

    [Serializable]
    public class CameraOptionThirdPerson : CameraOption
    {
        [Range(0f, 3.5f), Space, Tooltip("줌 확대 최대 거리")]
        public float zoomInDistance = 3f;
        [Range(0f, 5f), Tooltip("줌 축소 최대 거리")]
        public float zoomOutDistance = 3f;
        [Range(1f, 10f), Tooltip("줌 속도")]
        public float zoomSpeed = 5f;
    }
    [SerializeField]
    private CameraOptionThirdPerson _thirdPersonCameraOption = new CameraOptionThirdPerson();

    #endregion
}
