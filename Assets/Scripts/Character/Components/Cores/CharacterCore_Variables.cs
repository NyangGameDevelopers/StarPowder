using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-08 PM 10:18:36
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                              Const Variables
    ***********************************************************************/
    #region .
    public const int AnimationCommon = 0;
    public const int AnimationUpper = 1;
    //public const int AnimationLower = 2;

    #endregion
    /***********************************************************************
    *                              Variables
    ***********************************************************************/

    //[SerializeField]
    /// <summary> WASD 이동 벡터 : 정규화값 </summary>
    private Vector3 _moveDir;
    private Vector3 _worldMoveDir;

    /// <summary> TP 카메라 ~ Rig 초기 거리 </summary>
    private float _tpCamZoomInitialDistance;

    /// <summary> TP 카메라 휠 입력 값 </summary>
    private float _tpCameraWheelInput = 0;

    private bool _isMouseMiddlePressed = false;
    private bool _prevCursorVisibleState = false;

    private float _prevYPos;

    /***********************************************************************
    *                               Current Variables
    ***********************************************************************/
    #region .

    /// <summary> 현재 선택된 카메라 </summary>
    private PersonalCamera _currentCam;

    #endregion

    /***********************************************************************
    *                               Components
    ***********************************************************************/
    #region .
    public Rito.FpsTpsCharacter.PhysicsBasedMovement PbMove { get; private set; }
    public Rigidbody RBody { get; private set; }
    public Animator Anim { get; private set; }

    public FirstPersonCamera FPCam { get; private set; }
    public ThirdPersonCamera TPCam { get; private set; }

    /// <summary> 자식으로 도구를 장착할 오른손 위치 </summary>
    public RightHandMark RightHand { get; private set; }
    /// <summary> 자식으로 도구를 장착할 왼손 위치 </summary>
    public LeftHandMark LeftHand { get; private set; }

    /// <summary> 1인칭 좌우회전 </summary>
    public Transform Walker { get; private set; }
    /// <summary> 1인칭 상하회전 </summary>
    public Transform FpRig { get; private set; }

    /// <summary> 3인칭 좌우회전 </summary>
    public Transform TpRoot { get; private set; }
    /// <summary> 3인칭 상하회전 </summary>
    public Transform TpRig { get; private set; }

    // 캐릭터(워커 아래)
    public Transform Character { get; private set; }

    public ToolBox ToolBox { get; private set; }
    public WeaponBox WeaponBox { get; private set; }

    #endregion
}