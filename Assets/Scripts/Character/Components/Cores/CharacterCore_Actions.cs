using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

using Extensions;

// 날짜 : 2021-01-08 PM 9:23:05
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    // NOTE : 매개변수가 없는 메소드들

    /***********************************************************************
    *                             Animation Players
    ***********************************************************************/
    #region .

    // 1. All Layer
    private void ResetAnimation()
    {
        Anim.Play(GetAnimation(AnimType.None));
        Debug.Mark(_debugPlayAnimationCall);
    }
    private void PlayRollAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Roll));
        Debug.Mark(_debugPlayAnimationCall);
    }
    private void PlayBindAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Bind));
        Debug.Mark(_debugPlayAnimationCall);
    }

    /// <summary> Common : 스턴 / Upper : 애니메이션 없음 </summary>
    private void PlayResetAndStunAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Stun), AnimationCommon);
        Anim.Play(AnimationName.none, AnimationUpper);
        Debug.Mark(_debugPlayAnimationCall);
    }
    /// <summary> Common : 사망 / Upper : 애니메이션 없음 </summary>
    private void PlayResetAndDeathAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Die), AnimationCommon);
        Anim.Play(AnimationName.none, AnimationUpper);
        Debug.Mark(_debugPlayAnimationCall);
    }

    private void PlayIdleAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Idle));
        Debug.Mark(_debugPlayAnimationCall);
    }
    private void PlayMoveAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Move));
        Debug.Mark(_debugPlayAnimationCall);
    }

    // 2. Common Layer
    private void PlayCommonResetAnimation()
    {
        Anim.Play(GetAnimation(AnimType.None), AnimationCommon);
        Debug.Mark(_debugPlayAnimationCall);
    }
    private void PlayCommonIdleAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Idle), AnimationCommon);
        Debug.Mark(_debugPlayAnimationCall);
    }
    private void PlayCommonMoveAnimation()
    {
        Anim.Play(GetAnimation(AnimType.Move), AnimationCommon);
        Debug.Mark(_debugPlayAnimationCall);
    }

    // 3. Upper Layer
    /// <summary> 상체 애니메이션 초기화 </summary>
    private void ResetUpperAnimation()
    {
        Anim.Play(AnimationName.none, AnimationUpper);
        Debug.Mark(_debugPlayAnimationCall);
    }

    /// <summary> 상체 애니메이션 재생 </summary>
    private void PlayUpperAnimation(in string animationName)
    {
        Anim.Play(animationName, AnimationUpper);
        Debug.Mark(_debugPlayAnimationCall);
    }

    #endregion
    /***********************************************************************
    *                               Update Actions
    ***********************************************************************/
    #region .
    /// <summary> 쿨타임 및 지속시간 감소, 지속시간에 따른 특정 상태 변화 적용 </summary>
    private void DeclineCooldownDurationDeltatime()
    {
        float deltaTime = Time.deltaTime;

        // 1. Cooldown
        Decline(ref Current.rollCooldown, deltaTime);
        Decline(ref Current.toolCooldown, deltaTime);

        // 2. Duration
        Decline(ref Current.rollDuration, deltaTime);
        Decline(ref Current.stunDuration, deltaTime);
        Decline(ref Current.bindDuration, deltaTime);
        Decline(ref Current.changeModeDuration, deltaTime);

        // 3. Change States
        SetRollState(Current.rollDuration > 0f);
        SetStunState(Current.stunDuration > 0f);
        SetBindState(Current.bindDuration > 0f);

        void Decline(ref float currentCooldown, in float value)
        {
            if (currentCooldown > 0f)
                currentCooldown -= value;
            if (currentCooldown < 0f)
                currentCooldown = 0f;
        }

        Debug.Mark(_debugUpdateActionCall);
    }

    #endregion
    /***********************************************************************
    *                              Mode Chage Actions
    ***********************************************************************/
    #region .

    private void ChangeToNormalMode() => SetBehaviorMode(BehaviorMode.Normal);
    private void ChangeToBattleMode() => SetBehaviorMode(BehaviorMode.Battle);
    private void ChangeToVehicleMode() => SetBehaviorMode(BehaviorMode.OnVehicle);
    private void ChangeToBuildMode() => SetBehaviorMode(BehaviorMode.Build);


    #endregion
    /***********************************************************************
    *                              Player Actions
    ***********************************************************************/
    #region .
    /// <summary> 죽기 </summary>
    private void Die()
    {
        if (State.isDead) return; // 이미 쥬금

        // 차에 타고 있었으면 내리기
        if(CharacterIsOnVehicleMode())
            GetOffVehicle();

        SetDeadState(true);
        PlayResetAndDeathAnimation();

        Debug.Mark(_debugPlayerActionCall);
    }
    /// <summary> 부활하기 </summary>
    private void Revive()
    {
        if (!State.isDead) return; // 이미 생존

        SetDeadState(false);

        Debug.Mark(_debugPlayerActionCall);
    }

    /// <summary> 캐스팅! </summary>
    private void Cast(in float duration, Action action)
    {
        // 기존에 캐스팅 중이었다면 취소
        if (_castRoutine != null)
        {
            CancelCasting();
        }

        // 캐스팅!
        _castRoutine = StartCoroutine(CastActionRoutine(duration, action));
    }

    /// <summary> 캐스팅 취소 </summary>
    private void CancelCasting()
    {
        StopCoroutine(_castRoutine);
        _castRoutine = null;
        Current.castDuration = 0f;
    }

    /// <summary> 탑승 / 해제 토글 </summary>
    private void ToggleVehicleState()
    {
        if (CharacterIsOnVehicleMode()) GetOffVehicle();
        else RideOnVehicle();
    }

    /// <summary> 현재 등록한 탑승물에 타기! </summary>
    private void RideOnVehicle()
    {
        Current.vehicle.gameObject.SetActive(true);
        Character.localPosition = Current.vehicle.characterLocalPosition;

        // 3인칭 뷰로 변경
        SetCameraView(
            CameraViewOption.ThirdPerson,
            CurrentIsFPCamera() // TP카메라였으면 카메라 회전 인계하지 않음
        );
    }

    /// <summary> 내려오기! </summary>
    private void GetOffVehicle()
    {
        Current.vehicle.gameObject.SetActive(false);
        Character.localPosition = default;
        //SetBehaviorMode(BehaviorMode.Normal);
    }

    /// <summary> 도구 사용하고 애니메이션 재생 </summary>
    private void UseToolAndPlayAnimation()
    {
        if (OnToolCooldown()) return;
        //Anim.SetFloat("Attack Speed", Speed.attackSpeed);

        Current.toolInHand.Act(out ToolActionResult result);

        // 도구 사용 성공 시
        if (result.succeeded)
        {
            string animationName = "";

            switch (Current.toolInHand)
            {
                case Weapon weapon:
                    animationName = GetAttackAnimation(result.motionIndex);
                    break;
            }

            // 애니메이션 재생
            PlayUpperAnimation(animationName);

            // 도구 사용 쿨타임 세팅
            SetToolCooldown(result.coolDown);
        }

        Debug.Mark(_debugPlayerActionCall);
    }

    /// <summary> 키보드 WASD 이동 </summary>
    private void MoveWASD()
    {
        // WASD 입력이 없는 경우, 이동 불가 상태
        if (!State.isMoving || CharacterIsUnableToMove())
        {
            _worldMoveDir = Vector3.zero;
        }
        else
        {
            UpdateMoveDirection(_moveDir);
        }

        float moveSpeed;
        float speedMultiplier;
        if (CharacterIsOnVehicleMode())
        {
            moveSpeed = Current.vehicle.speed;
            speedMultiplier = Current.vehicle.runMultiplier;
        }
        else
        {
            moveSpeed = Speed.moveSpeed;
            speedMultiplier = Speed.runSpeedMultiplier;
        }

        PbMove.SetMovement(_worldMoveDir, State.isRunning);

        Debug.Mark(_debugPlayerActionCall);
    }

    /// <summary> 진행 방향으로 구르기 </summary>
    private void RollWASD()
    {
        if (CharacterIsUnableToMove()) return;
        if (OnToolCooldown()) return;
        if (OnRollingCooldown()) return;

        // 공중 구르기 스킬이 없으면 땅에서만 구르기 가능
        if (!State.isGrounded && !Skill.airTumbling) return;

        Current.rollCooldown = Duration.roll + Cooldown.roll;
        Current.rollDuration = Duration.roll;
        SetRollState(true);

        StartCoroutine(RollRoutine());

        Debug.Mark(_debugPlayerActionCall);
    }

    /// <summary> 점프하기 </summary>
    private void Jump()
    {
        PbMove.SetJump();

        Debug.Mark(_debugPlayerActionCall);
    }

    #endregion

    /***********************************************************************
    *                              Input Actions
    ***********************************************************************/
    #region .

    /// <summary> FP, TP 카메라 변경 </summary>
    private void Input_ChangeCamView()
    {
        if (ChangeCamViewKeyDown())
        {
            ToggleCameraView();

            Debug.Mark(_debugInputActionCall);
        }
    }

    /// <summary> 키보드 Alt, 마우스 MB 입력으로 커서 보이기/감추기 </summary>
    private void Input_SetCursorVisibleState()
    {
        // 1. 마우스 중앙버튼 유지하는 동안 커서 감추기
        if (MouseMiddleKeyDown())
        {
            _isMouseMiddlePressed = true;
            _prevCursorVisibleState = State.isCursorVisible;

            if (State.isCursorVisible)
            {
                SetCursorVisibleState(false);
            }

            Debug.Mark(_debugInputActionCall);
        }
        if (MouseMiddleKeyUp())
        {
            _isMouseMiddlePressed = false;

            if (_prevCursorVisibleState)
            {
                SetCursorVisibleState(true);
            }

            Debug.Mark(_debugInputActionCall);
        }

        // 2. Alt 눌러 커서 토글
        if (!_isMouseMiddlePressed && ShowCursorKeyDown())
        {
            State.isCursorVisible = !State.isCursorVisible;
            SetCursorVisibleState(State.isCursorVisible);

            Debug.Mark(_debugInputActionCall);
        }
    }

    /// <summary> 마우스를 상하/좌우로 움직여서 회전 </summary>
    private void Input_RotatePlayer()
    {
        Transform root, rig;

        // 1인칭
        if (CurrentIsFPCamera())
        {
            root = Walker;
            rig = FpRig;
        }
        // 3인칭
        else
        {
            root = TpRoot;
            rig = TpRig;
            RotateWalker(); // 3인칭일 경우 Walker를 이동방향으로 회전
        }

        if (State.isCursorVisible) return;

        // 회전 ==========================================================
        float deltaCoef = Time.deltaTime * 50f;
        Vector2 rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        // 상하 : Rig 회전
        float xRotPrev = rig.localEulerAngles.x;
        float xRotNext = xRotPrev + rotation.y
            * CamOption.rotationSpeed * deltaCoef;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // 좌우 : Root 회전
        float yRotPrev = root.localEulerAngles.y;
        float yRotNext =
            yRotPrev + rotation.x
            * CamOption.rotationSpeed * deltaCoef;

        // 상하 회전 가능 여부
        bool xRotatable =
            CamOption.lookUpDegree < xRotNext &&
            CamOption.lookDownDegree > xRotNext;

        // Rig 상하 회전 적용
        rig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        // Root 좌우 회전 적용
        root.localEulerAngles = Vector3.up * yRotNext;
    }
    private void RotateWalker()
    {
        if (State.isMoving == false) return;

        Vector3 dir = TpRig.TransformDirection(_moveDir);
        float currentY = Walker.localEulerAngles.y;
        float nextY = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

        if (nextY - currentY > 180f) nextY -= 360f;
        else if (currentY - nextY > 180f) nextY += 360f;

        Walker.eulerAngles = Vector3.up * Mathf.Lerp(currentY, nextY, 0.1f);
    }

    /// <summary> TP Cam : 마우스 휠 굴려서 확대/축소 </summary>
    private void Input_CameraZoom()
    {
        if (Current.cameraView == CameraViewOption.FirstPerson)
            return;

        _tpCameraWheelInput = Input.GetAxis("Mouse ScrollWheel");
    }

    /// <summary> WASD, LShift 입력으로 이동 벡터, 이동 상태 정의 </summary>
    private void Input_CalculatePhysics()
    {
        float h = 0f, v = 0f;

        if (Binding[UserAction.MoveForward].GetKey())  v += 1f;
        if (Binding[UserAction.MoveBackward].GetKey()) v -= 1f;
        if (Binding[UserAction.MoveLeft].GetKey())  h -= 1f;
        if (Binding[UserAction.MoveRight].GetKey()) h += 1f;;

        _moveDir = new Vector3(h, 0f, v);
        _moveDir.Normalize();

        Vector3 animDir; // 애니메이션 적용할 기준 방향벡터

        if (CurrentIsTPCamera())
        {
            _worldMoveDir = TpRoot.TransformDirection(_moveDir);
            animDir = Walker.InverseTransformDirection(_worldMoveDir);
        }
        else
        {
            _worldMoveDir = Walker.TransformDirection(_moveDir);
            animDir = _moveDir;
        }

        State.isMoving = _moveDir.magnitude > 0.1f && !CharacterIsRolling();
        State.isRunning = Binding[UserAction.Run].GetKey();
        State.isGrounded = PbMove.IsGrounded();

        // 부드러운 애니메이션을 위해 러프값 제공
        float goalValue = Mathf.Clamp(RBody.velocity.y, -1f, 1f);

        // 애니메이션 파라미터 설정
        float mul = State.isMoving ? (State.isRunning ? 1f : 0.5f) : 0f;
        animSpeedX = Mathf.Lerp(animSpeedX, animDir.x * mul, 0.05f);
        animSpeedZ = Mathf.Lerp(animSpeedZ, animDir.z * mul, 0.05f);
        animSpeedY = Mathf.Lerp(animSpeedY, goalValue, 0.05f);

        if (State.isGrounded)
        {
            Anim.SetFloat("Move X", animSpeedX);
            Anim.SetFloat("Move Z", animSpeedZ);
        }
        else
        {
            Anim.SetFloat("Move X", 0f);
            Anim.SetFloat("Move Z", 0f);
        }

        Anim.SetFloat("Move Y", animSpeedY);

        if (_moveDir.magnitude > 0.1f)
            Debug.Mark(_debugInputActionCall);
    }
    private float animSpeedX = 0f; // 부드러운 이동을 위해 이전 값 기억 (Lerp)
    private float animSpeedY = 0f;
    private float animSpeedZ = 0f;

    #endregion

    /***********************************************************************
    *                               UI Actions
    ***********************************************************************/
    #region .
    private void ShowPancakeUI()
    {
        State.isUsingPancake = true;
        if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Confined;

        UICore.I.Pancake.Show();
    }

    private void HidePancakeUIAndChangeTool()
    {
        State.isUsingPancake = false;
        if (Cursor.lockState == CursorLockMode.Confined)
            Cursor.lockState = CursorLockMode.Locked;

        int index = UICore.I.Pancake.FadeAndGetIndex();
        Current.selctedPancakeIndex = index;

        if (Current.normalTool)
        {
            Current.normalTool.TakeOff();
        }

        if (index == -1)
        {
            Current.normalTool = null;
            return;
        }
        else
        {
            ToolBox.CurrentIndex = index;
            Current.normalTool = ToolBox.CurrentTool.PutOn(LeftHand, RightHand);
        }
    }

    #endregion
}