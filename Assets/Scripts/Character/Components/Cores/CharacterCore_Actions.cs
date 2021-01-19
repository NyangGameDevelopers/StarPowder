using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

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
        Anim.Play(GetAnimation(AnimType.None), AnimationUpper);
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
        Decline(ref Current.attackCooldown, deltaTime);
        Decline(ref Current.firstAttackCooldown, deltaTime);

        // 2. Duration
        Decline(ref Current.rollDuration, deltaTime);
        Decline(ref Current.stunDuration, deltaTime);
        Decline(ref Current.bindDuration, deltaTime);
        Decline(ref Current.secondAttackChanceDuration, deltaTime);

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

    /// <summary> 땅으로부터의 거리 체크 </summary>
    private void CheckDistanceFromGround()
    {
        Vector3 ro = transform.position + Vector3.up;
        Vector3 rd = Vector3.down;
        float d = 500f;
        Ray ray = new Ray(ro, rd);

        bool catched = 
            //Physics.Raycast(ray, out var hit, d, Layers.GroundMask);
            Physics.SphereCast(ray, 0.1f, out var hit, d, Layers.GroundMask);

        Current.distFromGround = catched ? (hit.distance - 1f + 0.1f) : 9999f;

        SetGroundState(Current.distFromGround < 0.1f);

        Anim.SetFloat("Move Y", Mathf.Clamp(RBody.velocity.y, -1f, 1f));

        if (State.isGrounded)
        {
            Current.doubleJumped = false;
        }

        Debug.Mark(_debugUpdateActionCall);
    }

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

    /// <summary> 캐릭터 모드 변경 </summary>
    private void ChangeBehaviorToggle()
    {
        switch (State.behaviorMode)
        {
            case BehaviorMode.None: SetBehaviorMode(BehaviorMode.Battle); break;
            case BehaviorMode.Battle:SetBehaviorMode(BehaviorMode.Witch); break;
            case BehaviorMode.Witch:SetBehaviorMode(BehaviorMode.None); break;
        }

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

        SetBehaviorMode(BehaviorMode.OnVehicle);
    }

    /// <summary> 내려오기! </summary>
    private void GetOffVehicle()
    {
        Current.vehicle.gameObject.SetActive(false);
        Character.localPosition = default;
        SetBehaviorMode(BehaviorMode.None);
    }

    /// <summary> 공격 </summary>
    private void AttackAndPlayAnimation()
    {
        //if (!CharacterIsBattleMode()) return;
        Anim.SetFloat("Attack Speed", Speed.attackSpeed);

        if (Current.attackCooldown < 0.01f)
        {
            FirstAttack();
        }
        else if (!Current.secondAttacked
            && Current.firstAttackCooldown < 0.01f
            && Current.secondAttackChanceDuration > 0f)
        {
            SecondAttack();
        }

        Debug.Mark(_debugPlayerActionCall);
    }
    private void FirstAttack()
    {
        // 쿨타임, 지속시간 세팅
        Current.attackCooldown = Cooldown.attack / Speed.attackSpeed;
        Current.firstAttackCooldown = Cooldown.firstAttack / Speed.attackSpeed;
        Current.secondAttackChanceDuration =
            (Cooldown.firstAttack + Duration.secondAttackChance) / Speed.attackSpeed;
        Current.secondAttacked = false;

        Anim.Play(AnimationName.upperBattleAttack0, AnimationUpper);

        Debug.Mark(_debugPlayerActionCall);
    }
    private void SecondAttack()
    {
        Current.secondAttacked = true;
        Anim.Play(AnimationName.upperBattleAttack1, AnimationUpper);

        Debug.Mark(_debugPlayerActionCall);
    }

    /// <summary> 키보드 WASD 이동 </summary>
    private void MoveWASD()
    {
        // WASD 입력이 없는 경우, 이동 불가 상태
        if (_moveDir.magnitude < 0.1f || CharacterIsUnableToMove())
        {
            if (State.isGrounded)
            {
                RBody.velocity = default;
                RBody.useGravity = false;
            }
            else
            {
                RBody.useGravity = true;
            }
            return;
        }

        RBody.useGravity = true;

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

        Vector3 next = _worldMoveDir * moveSpeed * 
            (State.isRunning ? speedMultiplier : 1f);

        RBody.velocity = new Vector3(next.x, RBody.velocity.y, next.z);
        UpdateMoveDirection(_moveDir);

        // 캐릭터 회전
#if MOVE2
        if (CurrentIsTPCamera())
        {
            Vector3 dir = TPCam.Rig.TransformDirection(_moveDir);
            float currentY = Walker.localEulerAngles.y;
            float nextY = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

            if (nextY - currentY > 180f) nextY -= 360f;
            else if (currentY - nextY > 180f) nextY += 360f;

            Walker.eulerAngles = Vector3.up * Mathf.Lerp(currentY, nextY, 0.05f);
        }
#endif

        Debug.Mark(_debugPlayerActionCall);
    }

    /// <summary> 진행 방향으로 구르기 </summary>
    private void RollWASD()
    {
        if (CharacterIsUnableToMove()) return;
        if (OnFirstAttackCooldown()) return;
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
        // 공중에 떠있는 상태
        if (!State.isGrounded)
        {
            if (!Skill.doubleJump) return;

            // 더블 점프 스킬 보유, 더블 점프 아직 안함
            if (!Current.doubleJumped) Current.doubleJumped = true;
            else return;
        }

        // 더블점프 시 도움닫기용 속도 초기화
        if (Current.doubleJumped)
        {
            RBody.velocity = default;
        }
        RBody.AddForce(Vector3.up * Move.jumpForce, ForceMode.VelocityChange);

        Debug.Mark(_debugPlayerActionCall);
    }

    #endregion

    /***********************************************************************
    *                            Input Actions
    ***********************************************************************/
    #region .

    /// <summary> FP, TP 카메라 변경 </summary>
    private void Input_ChangeCamView()
    {
        if (Input.GetKeyDown(Key.changeViewToggle))
        {
            ToggleCameraView();

            Debug.Mark(_debugInputActionCall);
        }
    }

    private void Input_ChangeBehaviorMode()
    {
        if (Input.GetKeyDown(Key.changeBehaviorMode))
        {
            ChangeBehaviorToggle();

            Debug.Mark(_debugInputActionCall);
        }
    }

    /// <summary> 키보드 Alt, 마우스 MB 입력으로 커서 보이기/감추기 </summary>
    private void Input_SetCursorVisibleState()
    {
        // 1. 마우스 중앙버튼 유지하는 동안 커서 감추기
        if (Input.GetMouseButtonDown(2))
        {
            _isMouseMiddlePressed = true;
            _prevCursorVisibleState = State.isCursorVisible;

            if (State.isCursorVisible)
            {
                SetCursorVisibleState(false);
            }

            Debug.Mark(_debugInputActionCall);
        }
        if (Input.GetMouseButtonUp(2))
        {
            _isMouseMiddlePressed = false;

            if (_prevCursorVisibleState)
            {
                SetCursorVisibleState(true);
            }

            Debug.Mark(_debugInputActionCall);
        }

        // 2. Alt 눌러 커서 토글
        if (!_isMouseMiddlePressed && Input.GetKeyDown(Key.showCursorToggle))
        {
            State.isCursorVisible = !State.isCursorVisible;
            SetCursorVisibleState(State.isCursorVisible);

            Debug.Mark(_debugInputActionCall);
        }
    }

    /// <summary> 마우스를 상하/좌우로 움직여서 회전 </summary>
    private void Input_RotatePlayer()
    {
        if (CurrentIsFPCamera()) RotateFP();
        else RotateTP();
    }

    private void RotateFP()
    {
        Transform fpCamRig = FPCam.Rig; // Rig : 상하 회전
        Transform walker = Walker;  // Walker : 좌우 회전

        // ================================================
        // 상하 : 카메라 Rig 회전
        float vDegree = -Input.GetAxisRaw("Mouse Y");
        float xRotPrev = fpCamRig.localEulerAngles.x;
        float xRotNext = xRotPrev
            + vDegree
            * _currentCamOption.rotationSpeed
            * Time.deltaTime * 50f;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // ================================================
        // 좌우 : 워커 회전
        float hDegree = Input.GetAxisRaw("Mouse X");
        float yRotPrev = walker.localEulerAngles.y;
        float yRotAdd =
            hDegree
            * _currentCamOption.rotationSpeed
            * Time.deltaTime * 50f;
        float yRotNext = yRotAdd + yRotPrev;

        // 상하 회전 가능 여부
        bool xRotatable =
            _currentCamOption.lookUpDegree < xRotNext &&
            _currentCamOption.lookDownDegree > xRotNext;

        // 좌우 회전 가능 여부
        bool yRotatable = !CharacterIsUnableToMove();

        // Rig 상하 회전 적용
        fpCamRig.localEulerAngles = Vector3.right * (xRotatable ? xRotNext : xRotPrev);

        // 워커 좌우 회전 적용
        if (yRotatable)
        {
            walker.localEulerAngles = Vector3.up * yRotNext;
        }
    }

    float cur;
    private void RotateTP()
    {
        if (State.isCursorVisible && !_isMouseMiddlePressed)
            return;

        Transform tpCamRig = TPCam.Rig;

        // ================================================
        // 상하 : 카메라 Rig 회전
        float vDegree = -Input.GetAxisRaw("Mouse Y");
        float xRotPrev = tpCamRig.localEulerAngles.x;
        float xRotNext = xRotPrev
            + vDegree
            * _currentCamOption.rotationSpeed
            * Time.deltaTime * 50f;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // ================================================
        // 좌우 : 카메라 Rig 회전
        float hDegree = Input.GetAxisRaw("Mouse X");
#if MOVE2
        float yRotPrev = tpCamRig.localEulerAngles.y;
#else
        float yRotPrev = Walker.localEulerAngles.y;
#endif

        float yRotAdd =
            hDegree
            * _currentCamOption.rotationSpeed
            * Time.deltaTime * 50f;
        float yRotNext = yRotAdd + yRotPrev;

        // 상하 회전 가능 여부 판정
        bool xRotatable =
            _currentCamOption.lookUpDegree < xRotNext &&
            _currentCamOption.lookDownDegree > xRotNext;

        Vector3 nextRot = new Vector3
        (
            xRotatable ? xRotNext : xRotPrev,
#if !MOVE2
            !CharacterIsUnableToMove() ? yRotNext : yRotPrev,
#else
            yRotNext,
#endif
            0f
        );

        // Rig 상하좌우 회전 적용
        tpCamRig.localEulerAngles = nextRot;

#if !MOVE2
        // 워커 좌우 회전 적용
        if(!CharacterIsUnableToMove())
            Walker.localEulerAngles = Vector3.up * yRotNext;
#endif
    }

    /// <summary> TP Cam : 마우스 휠 굴려서 확대/축소 </summary>
    private void Input_CameraZoom()
    {
        if (Current.cameraView == CameraViewOption.FirstPerson)
            return;

        _tpCameraWheelInput = Input.GetAxis("Mouse ScrollWheel");
    }

    /// <summary> WASD, LShift 입력으로 이동 벡터, 이동 상태 정의 </summary>
    private void Input_CalculateMoveDirection()
    {
        _moveDir = Vector3.zero;

        if (Input.GetKey(Key.moveForward)) _moveDir += Vector3.forward;
        if (Input.GetKey(Key.moveBackward)) _moveDir += Vector3.back;
        if (Input.GetKey(Key.moveLeft)) _moveDir += Vector3.left;
        if (Input.GetKey(Key.moveRight)) _moveDir += Vector3.right;

        _moveDir.Normalize();
        Vector3 checkDir;

#if MOVE2
        if (CurrentIsTPCamera())
        {
            _worldMoveDir = TPCam.Rig.TransformDirection(_moveDir);
            checkDir = Walker.forward;
        }
        else
        {
            _worldMoveDir = Walker.TransformDirection(_moveDir);
            checkDir = _worldMoveDir;
        }
#else
        _worldMoveDir = Walker.TransformDirection(_moveDir);
        checkDir = _worldMoveDir;
#endif


        // 벽 매미 현상 방지
        State.isAdjcentToWall =
            CheckAdjecentToWall(checkDir, 0.05f) ||
            CheckAdjecentToWall(checkDir, 0.5f) ||
            CheckAdjecentToWall(checkDir, 0.95f);

        if (State.isAdjcentToWall)
        {
            _worldMoveDir = default;
        }

        bool isRunningKeyDown = Input.GetKey(Key.run);
        bool moving = _moveDir.magnitude > 0.1f && !CharacterIsRolling();

        SetMovingState(moving);
        SetWalkingState(moving && !isRunningKeyDown);
        SetRunningState(moving && isRunningKeyDown);

        // 애니메이션 파라미터 설정
        float mul = State.isWalking ? 0.5f : 1f;
        animSpeedX = Mathf.Lerp(animSpeedX, _moveDir.x * mul, 0.05f);
        animSpeedZ = Mathf.Lerp(animSpeedZ, _moveDir.z * mul, 0.05f);

        Anim.SetFloat("Move X", animSpeedX);
        Anim.SetFloat("Move Z", animSpeedZ);

        if (_moveDir.magnitude > 0.1f)
            Debug.Mark(_debugInputActionCall);
    }
    private float animSpeedX = 0f;
    private float animSpeedZ = 0f;

#endregion
}