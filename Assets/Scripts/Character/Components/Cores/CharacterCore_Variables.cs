using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-08 PM 10:18:36
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                             Core - Variables
    ***********************************************************************/

    //[SerializeField]
    /// <summary> WASD 이동 벡터 </summary>
    private Vector3 _moveDir;

    /// <summary> TP 카메라 -> Rig 방향 벡터 </summary>
    private Vector3 _tpCamToRigDir;

    /// <summary> TP 카메라 ~ Rig 초기 거리 </summary>
    private float _tpCamZoomInitialDistance;

    /// <summary> TP 카메라 휠 입력 값 </summary>
    private float _tpCameraWheelInput = 0;

    private bool _isMouseMiddlePressed = false;
    private bool _prevCursorVisibleState = false;

    /***********************************************************************
    *                               Current Variables
    ***********************************************************************/
    #region .

    /// <summary> 현재 선택된 카메라 </summary>
    private PersonalCamera _currentCam;
    /// <summary> 현재 선택된 카메라의 옵션 </summary>
    private CameraOption _currentCamOption;

    #endregion

    /***********************************************************************
    *                               Components
    ***********************************************************************/
    #region .
    public Rigidbody RBody { get; private set; }
    public Animator Anim { get; private set; }

    public FirstPersonCamera FPCam { get; private set; }
    public ThirdPersonCamera TPCam { get; private set; }

    #endregion
}