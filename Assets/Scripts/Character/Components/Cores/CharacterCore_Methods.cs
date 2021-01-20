using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-08 PM 10:29:19
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *             PUBLIC : Player Action, Interaction Methods
    ***********************************************************************/
    #region .
    /// <summary> 피해 입히기 </summary>
    public void DoDamage(float damage) { }

    /// <summary> 기절시키기 </summary>
    public void DoStun(float duration) => Current.stunDuration = duration;

    /// <summary> 속박시키기 </summary>
    public void DoBind(float duration) => Current.bindDuration = duration;

    #endregion


    /***********************************************************************
    *                            Init Methods
    ***********************************************************************/
    #region .

    private void InitializeComponents()
    {
        // Rigs (중요)
        var charRig = GetComponentInChildren<CharacterMark>(); // 활성화된 캐릭터만 찾기
        Character = charRig.transform;
        Anim = Character.GetComponent<Animator>();

        // 손꾸락
        var rightHand = charRig.GetComponentInAllChildren<RightHandMark>();
        RightHand = rightHand.gameObject;
        var leftHand = charRig.GetComponentInAllChildren<LeftHandMark>();
        LeftHand = leftHand.gameObject;

        var walkerRig = GetComponentInAllChildren<WalkerMark>();
        Walker = walkerRig.transform;

        // 캐릭터는 캐릭터 레이어 설정
        SetLayerRecursive(Character, Layers.CharacterLayer);
        // 무기는 기본 레이어 설정
        SetLayerRecursive(LeftHand.transform, Layers.Default);
        SetLayerRecursive(RightHand.transform, Layers.Default);

        // Gets
        RBody = GetComponent<Rigidbody>();

        FPCam = GetComponentInAllChildren<FirstPersonCamera>();
        TPCam = GetComponentInAllChildren<ThirdPersonCamera>();
        FPCam.Init();
        TPCam.Init();

        Current.vehicle = GetComponentInAllChildren<Vehicle>();
        GetOffVehicle(); // 일단 자동차 비활성화

        // Error Check
        if (RBody == null) Debug.LogError("플레이어 캐릭터에 리지드바디가 존재하지 않습니다.");
        if (Anim == null) Debug.LogError("플레이어 캐릭터에 애니메이터가 존재하지 않습니다.");

        // Init Component Values
        RBody.constraints = RigidbodyConstraints.FreezeRotation;
        Anim.applyRootMotion = false;
    }

    private void InitializeValues()
    {
        // Cursor
        SetCursorVisibleState(false);

        // Camera
        Vector3 camToRig = TPCam.Rig.position - TPCam.transform.position;
        _tpCamToRigDir = TPCam.transform.InverseTransformDirection(camToRig).normalized;
        _tpCamZoomInitialDistance = Vector3.Magnitude(camToRig);

        // 초기 설정들
        SetCameraView(Current.cameraView); // 초기 뷰 설정
        SetCameraAlone(); // 카메라 한개 빼고 전부 비활성화
        SetBehaviorMode(BehaviorMode.None);
    }

    #endregion
    /***********************************************************************
    *                            Getter Methods
    ***********************************************************************/
    #region .
    /// <summary> 현재 모드에 따라 알맞은 애니메이션 이름 참조 </summary>
    public string GetAnimation(AnimType type)
    {
        // 죽음 : 공통으로 사망 애니메이션
        if (type.Equals(AnimType.Die))
            return AnimationName.die;

        bool idle = type.Equals(AnimType.Idle);
        bool moving = type.Equals(AnimType.Move);
        bool rolling = type.Equals(AnimType.Roll);
        bool binded = type.Equals(AnimType.Bind);
        bool stunned = type.Equals(AnimType.Stun);

        bool oneHand = Current.handType.Equals(HandType.OneHand);
        bool twoHand = Current.handType.Equals(HandType.TwoHand);

        switch (State.behaviorMode)
        {
            case BehaviorMode.None when idle:    return AnimationName.idle;
            case BehaviorMode.None when moving:  return AnimationName.move;
            case BehaviorMode.None when rolling: return AnimationName.roll;
            case BehaviorMode.None when binded:  return AnimationName.bind;
            case BehaviorMode.None when stunned: return AnimationName.stun;
             
            case BehaviorMode.Battle when oneHand && idle:    return AnimationName.oneHandIdle;
            case BehaviorMode.Battle when oneHand && moving:  return AnimationName.oneHandMove;
            case BehaviorMode.Battle when oneHand && rolling: return AnimationName.oneHandRoll;
             
            case BehaviorMode.Battle when twoHand && idle:    return AnimationName.twoHandIdle;
            case BehaviorMode.Battle when twoHand && moving:  return AnimationName.twoHandMove;
            case BehaviorMode.Battle when twoHand && rolling: return AnimationName.twoHandRoll;

            case BehaviorMode.Battle when binded:  return AnimationName.bind;
            case BehaviorMode.Battle when stunned: return AnimationName.stun;
             
            // 마녀는 그냥 마녀만
            case BehaviorMode.Witch:    return AnimationName.witch;

            // 탑승도 얌전히 앉아있기만
            case BehaviorMode.OnVehicle: return AnimationName.onVehicle;

            default: return AnimationName.none;
        }
    }

    /// <summary> 현재 손 타입, 공격 인덱스(1, 2)에 따라 공격 애니메이션 이름 가져오기 </summary>
    public string GetAttackAnimation(int attackIndex)
    {
        switch (Current.handType)
        {
            case HandType.OneHand when attackIndex == 1: return AnimationName.oneHandAttack1;
            case HandType.OneHand when attackIndex == 2: return AnimationName.oneHandAttack2;
            case HandType.OneHand when attackIndex == 3: return AnimationName.oneHandAttack3;

            case HandType.TwoHand when attackIndex == 1: return AnimationName.twoHandAttack1;
            case HandType.TwoHand when attackIndex == 2: return AnimationName.twoHandAttack2;
            case HandType.TwoHand when attackIndex == 3: return AnimationName.twoHandAttack3;
        }

        Debug.Log("Methods_GetAttackAnimation : 잘못된 참조");
        return "";
    }

    #endregion

    /***********************************************************************
    *                            Setter, Changer Methods
    ***********************************************************************/
    #region .
    private void SetDeadState(bool value) => State.isDead = value;
    private void SetStunState(bool value) => State.isStunned = value;
    private void SetBindState(bool value) => State.isBinded = value;
    private void SetGroundState(bool value) => State.isGrounded = value;
    private void SetRollState(bool value) => State.isRolling = value;
    private void SetMovingState(bool value) => State.isMoving = value;
    private void SetWalkingState(bool value) => State.isWalking = value;
    private void SetRunningState(bool value) => State.isRunning = value;

    private void SetCastDuration(in float duration) => Current.castDuration = duration;

    /// <summary> 행동모드 변경, 변경에 따른 양손 액티브 상태 조절 </summary>
    private void SetBehaviorMode(BehaviorMode mode)
    {
        State.behaviorMode = mode;

        // 손 게임오브젝트 활성화 상태 변경
        RightHand.SetActive(
            mode.Equals(BehaviorMode.Battle) ||
            mode.Equals(BehaviorMode.Witch)
        );
        LeftHand.SetActive(
            mode.Equals(BehaviorMode.Battle)
        );

        // 사용 중인 도구 종류 갱신
        switch (mode)
        {
            case BehaviorMode.Battle:
                Current.toolType = ToolType.Weapon;
                break;

            default:
                Current.toolType = ToolType.None;
                break;
        }
    }

    private void SetCursorVisibleState(bool value)
    {
        if (value)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SetCameraView(CameraViewOption view, bool inheritRotation = true)
    {
        Current.cameraView = view;
        bool isFP = CurrentIsFPCamera();

        FPCam.Cam.gameObject.SetActive(isFP);
        TPCam.Cam.gameObject.SetActive(!isFP);

        // TP -> FP
        if (isFP)
        {
            _currentCam = FPCam;
            _currentCamOption = FPCamOption;
#if !OLDCAM
            // TP의 회전 인계
            if (inheritRotation)
            {
                Walker.eulerAngles = TPCam.Rig.eulerAngles;
            }
#endif
        }
        // FP -> TP
        else
        {
            _currentCam = TPCam;
            _currentCamOption = TPCamOption;
#if !OLDCAM
            // FP의 회전 인계
            if (inheritRotation)
            {
                TPCam.Rig.eulerAngles = Walker.eulerAngles;
            }

#endif
        }
    }

    /// <summary> 대상 게임오브젝트 및 모든 자식까지 재귀적으로 레이어 설정 </summary>
    private void SetLayerRecursive(Transform target, int layer)
    {
        target.gameObject.layer = layer;

        for (int i = 0; i < target.childCount; i++)
        {
            SetLayerRecursive(target.GetChild(i), layer);
        }
    }

    /// <summary> 현재 활성화된 주요 카메라 외에 모든 카메라 게임오브젝트 비활성화 </summary>
    private void SetCameraAlone()
    {
        var cams = FindObjectsOfType<Camera>();
        foreach (var cam in cams)
        {
            if (cam != _currentCam.Cam)
            {
                cam.gameObject.SetActive(false);
            }
        }
    }

#endregion
    /***********************************************************************
    *                            Toggle, Update Methods
    ***********************************************************************/
#region .
    private void ToggleCameraView()
    {
        SetCameraView(CurrentIsFPCamera() ?
            CameraViewOption.ThirdPerson : CameraViewOption.FirstPerson);
    }

    /// <summary> 현재 캐릭터의 이동방향 설정 </summary>
    private void UpdateMoveDirection(Vector3 moveDir)
    {
        if (Plus(moveDir.z))
        {
            if      (Minus(moveDir.x)) Current.moveDirection = MoveDirection.FrontLeft;
            else if (Plus(moveDir.x))  Current.moveDirection = MoveDirection.FrontRight;
            else Current.moveDirection = MoveDirection.Front;
        }
        else if (Minus(moveDir.z))
        {
            if      (Minus(moveDir.x)) Current.moveDirection = MoveDirection.Backleft;
            else if (Plus(moveDir.x))  Current.moveDirection = MoveDirection.BackRight;
            else Current.moveDirection = MoveDirection.Back;
        }
        else
        {
            if      (Minus(moveDir.x)) Current.moveDirection = MoveDirection.Left;
            else if (Plus(moveDir.x))  Current.moveDirection = MoveDirection.Right;
            else Current.moveDirection = MoveDirection.None;
        }

        bool Plus(in float value) => (value > 0.1f);
        bool Minus(in float value) => (value < -0.1f);
    }

#endregion
    /***********************************************************************
    *                            Finder Methods
    ***********************************************************************/
#region .
    /// <summary> Active False인 자식도 다 뒤져서 컴포넌트 찾아오기 </summary>
    private T GetComponentInAllChildren<T>() where T : Component
    {
        List<Transform> _childrenTrList = new List<Transform>();
        Recur_GetAllChildrenTransform(_childrenTrList, transform);

        foreach (var tr in _childrenTrList)
        {
            T found = tr.GetComponent<T>();
            if (found != null)
                return found;
        }
        return null;
    }
    private void Recur_GetAllChildrenTransform(List<Transform> trList, Transform tr)
    {
        trList.Add(tr);
        int childCount = tr.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Recur_GetAllChildrenTransform(trList, tr.GetChild(i));
        }
    }

#endregion
    /***********************************************************************
    *                            Checker Methods
    ***********************************************************************/
#region .


    /// <summary> 이동 방향 코앞에 벽이 있는지 검사 </summary>
    private bool CheckAdjecentToWall(in Vector3 dir, in float originHeight)
    {
        if (dir.magnitude < 0.1f)
        {
            return false;
        }

        Vector3 ro = transform.position + Vector3.up * originHeight;
        Vector3[] rds = {
            dir,
            Quaternion.Euler(0f, 18f, 0f) * dir,
            Quaternion.Euler(0f, 35f, 0f) * dir,
            Quaternion.Euler(0f, -18f, 0f) * dir,
            Quaternion.Euler(0f, -35f, 0f) * dir
        };
        float d = 0.4f;

        foreach (var rd in rds)
        {
            Ray ray = new Ray(ro, rd);
            bool found = Physics.Raycast(ray, out var hit, d, Layers.GroundMask);

            if (found)
            {
                Vector3 normal = hit.normal;
                float dot = Mathf.Abs(Vector3.Dot(normal, Vector3.up));

                if (dot < 0.05f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion
    /***********************************************************************
    *                            Calculation Methods
    ***********************************************************************/
    #region .
    private bool InRange(in float variable, in float min, in float max)
    {
        return variable >= min && variable <= max;
    }

#endregion
}