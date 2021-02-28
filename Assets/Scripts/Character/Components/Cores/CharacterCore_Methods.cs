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
        PbMove = GetComponent<Rito.CharacterControl.PhysicsBasedMovement>();
        if(PbMove == null)
            PbMove = gameObject.AddComponent<Rito.CharacterControl.PhysicsBasedMovement>();

        // Marks (중요)
        var charRig = GetComponentInChildren<CharacterMark>(); // 활성화된 캐릭터만 찾기
        Character = charRig.transform;
        Anim = Character.GetComponent<Animator>();

        // 손꾸락
        RightHand = charRig.GetComponentInAllChildren<RightHandMark>();
        LeftHand = charRig.GetComponentInAllChildren<LeftHandMark>();

        // 도구박스, 무기박스
        ToolBox = GetComponentInChildren<ToolBox>();
        WeaponBox = GetComponentInChildren<WeaponBox>();

        // 임시 : 첫 번째 무기 착용
        if (WeaponBox.Count > 0)
        {
            Current.battleWeapon = WeaponBox.CurrentWeapon;
        }

        // 캐릭터는 캐릭터 레이어 설정
        SetLayerRecursive(Character, Layers.CharacterLayer);

        // Gets
        RBody = GetComponent<Rigidbody>();
        FPCam = GetComponentInAllChildren<FirstPersonCamera>();
        TPCam = GetComponentInAllChildren<ThirdPersonCamera>();
        FPCam.Init();
        TPCam.Init();

        // Root, Rigs
        FpRig = FPCam.transform.parent;
        Walker = FpRig.transform.parent;
        TpRig = TPCam.transform.parent;
        TpRoot = TpRig.transform.parent;

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
        _tpCamZoomInitialDistance = Vector3.Magnitude(camToRig);

        // 초기 설정들
        SetCameraView(Current.cameraView); // 초기 뷰 설정
        SetCameraAlone(); // 카메라 한개 빼고 전부 비활성화
        SetBehaviorMode(BehaviorMode.Normal);
    }

    #endregion
    /***********************************************************************
    *                            Getter Methods
    ***********************************************************************/
    #region .
    /// <summary> 현재 모드에 따라 알맞은 애니메이션 이름 참조 </summary>
    public string GetAnimation(AnimType type)
    {
        //Debug.Log($"GetAnimation - AnimType : {type}");

        // 죽음 : 공통으로 사망 애니메이션
        if (type.Equals(AnimType.Die)) return AnimationName.die;

        bool idle    = type.Equals(AnimType.Idle);
        bool moving  = type.Equals(AnimType.Move);
        bool rolling = type.Equals(AnimType.Roll);
        bool binded  = type.Equals(AnimType.Bind);
        bool stunned = type.Equals(AnimType.Stun);

        // 상태에 따른 고정 애니메이션
        if (Current.behaviorMode.Equals(BehaviorMode.Witch)) return AnimationName.witch;
        if (Current.behaviorMode.Equals(BehaviorMode.OnVehicle)) return AnimationName.onVehicle;

        // 공통 애니메이션
        if (stunned) return AnimationName.stun;
        if (binded)  return AnimationName.bind;

        // 맨손
        if (Current.toolInHand == null)
        {
            if (idle) return AnimationName.idle;
            if (moving) return AnimationName.move;
            if (rolling) return AnimationName.roll;
        }

        // 도구 존재
        switch (Current.toolInHand.handType)
        {
            case HandType.RightHand when idle:    return AnimationName.rightHandIdle;
            case HandType.RightHand when moving:  return AnimationName.rightHandMove;
            case HandType.RightHand when rolling: return AnimationName.rightHandRoll;

            case HandType.DoubleHand when idle:    return AnimationName.doubleHandIdle;
            case HandType.DoubleHand when moving:  return AnimationName.doubleHandMove;
            case HandType.DoubleHand when rolling: return AnimationName.doubleHandRoll;

            case HandType.TwoHand when idle:    return AnimationName.twoHandIdle;
            case HandType.TwoHand when moving:  return AnimationName.twoHandMove;
            case HandType.TwoHand when rolling: return AnimationName.twoHandRoll;
        }

        return AnimationName.none;
    }

    /// <summary> 현재 손 타입, 공격 인덱스(1, 2)에 따라 공격 애니메이션 이름 가져오기 </summary>
    public string GetAttackAnimation(int attackIndex)
    {
        switch (Current.toolInHand.handType)
        {
            // 오른손 한손무기
            case HandType.RightHand:  return AnimationName.rightHandAttacks[attackIndex];
            case HandType.DoubleHand: return AnimationName.doubleHandAttacks[attackIndex];
            case HandType.TwoHand:    return AnimationName.twoHandAttacks[attackIndex];
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
    private void SetRunningState(bool value) => State.isRunning = value;

    private void SetToolCooldown(in float cooldown) => Current.toolCooldown = cooldown;
    private void SetCastDuration(in float duration) => Current.castDuration = duration;

    /// <summary> 행동모드 변경, 변경에 따른 양손 액티브 상태 조절 </summary>
    private void SetBehaviorMode(BehaviorMode mode)
    {
        BehaviorMode prevMode = Current.behaviorMode;

        switch (mode)
        {
            case BehaviorMode.Normal:
            case BehaviorMode.OnVehicle:
                if (Current.battleWeapon)
                {
                    Current.battleWeapon.TakeOff();
                }
                if (Current.normalTool)
                {
                    Current.normalTool.PutOn(LeftHand, RightHand);
                }

                //if (prevMode.Equals(BehaviorMode.Battle))
                //    PlayUpperAnimation("TAKE_OFF_WEAPON");
                break;

            case BehaviorMode.Battle:
                if (Current.normalTool)
                {
                    Current.normalTool.TakeOff();
                }
                if (Current.battleWeapon)
                {
                    Current.battleWeapon.PutOn(LeftHand, RightHand);
                }

                if (prevMode.Equals(BehaviorMode.Normal))
                    PlayUpperAnimation("PUT_ON_WEAPON");

                Current.changeModeDuration = 0.5f;
                break;
        }

        if (mode.Equals(BehaviorMode.OnVehicle))
        {
            RideOnVehicle();
        }

        if (prevMode.Equals(BehaviorMode.OnVehicle))
        {
            GetOffVehicle();
        }

        Current.behaviorMode = mode;

        // 손 게임오브젝트 활성화 상태 변경
        //RightHand.gameObject.SetActive(
        //    mode.Equals(BehaviorMode.Battle) ||
        //    mode.Equals(BehaviorMode.Witch)
        //);
        //LeftHand.gameObject.SetActive(
        //    mode.Equals(BehaviorMode.Battle)
        //);
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
            Walker.localEulerAngles = Vector3.up * TpRoot.localEulerAngles.y;
            FpRig.localEulerAngles = Vector3.right * TpRig.localEulerAngles.x;
        }
        // FP -> TP
        else
        {
            _currentCam = TPCam;
            TpRoot.localEulerAngles = Vector3.up * Walker.localEulerAngles.y;
            TpRig.localEulerAngles = Vector3.right * FpRig.localEulerAngles.x;
        }

        // 태그 변경 : Camera.main으로 참조하기 위해
        _currentCam.tag = "MainCamera";
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