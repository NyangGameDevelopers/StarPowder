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
    private void ResetAnimation()
    {
        Anim.Play(AnimationNameSet.none);
    }
    private void PlayIdleAnimation()
    {
        Anim.Play(AnimationNameSet.idle);
        PlayUpperAnimation(AnimationNameSet.upperBattleIdle);
    }
    private void PlayBindAnimation() => Anim.Play(AnimationNameSet.bind);
    private void PlayStunAnimation()
    {
        ResetUpperAnimation();
        Anim.Play(AnimationNameSet.stun);
    }

    private void PlayDeathAnimation()
    {
        ResetUpperAnimation();
        Anim.Play(AnimationNameSet.die);
    }

    private void PlayMoveAnimation()
    {
        PlayUpperAnimation(AnimationNameSet.upperBattleMove);
        Anim.Play(AnimationNameSet.move);
    }

    private void PlayRollAnimation()
    {
        ResetUpperAnimation();
        Anim.Play(AnimationNameSet.roll);
    }

    /// <summary> 상체 애니메이션 초기화 </summary>
    private void ResetUpperAnimation()
    {
        Anim.Play(AnimationNameSet.upperNone, AnimationUpper);
    }

    /// <summary> 전투모드 상체 애니메이션 재생 </summary>
    private void PlayUpperAnimation(in string battleAnimationName)
    {
        if (CharacterIsBattleMode())
        {
            Anim.Play(battleAnimationName, AnimationUpper);
        }
        else
        {
            ResetUpperAnimation();
        }
    }

    /// <summary> 근접 공격 : 상체만 애니메이션 재생 </summary>
    private void PlayAttackAnimation(int index)
    {
        if (index == 0)
        {
            Anim.Play(AnimationNameSet.upperBattleAttack0, AnimationUpper);
        }
        else
        {
            Anim.Play(AnimationNameSet.upperBattleAttack1, AnimationUpper);
        }
    }

    private void PlayUpperIdleAnimation()
    {
        PlayUpperAnimation(AnimationNameSet.upperBattleIdle);
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

        // 2. Duration
        Decline(ref Current.rollDuration, deltaTime);
        Decline(ref Current.stunDuration, deltaTime);
        Decline(ref Current.bindDuration, deltaTime);
        Decline(ref Current.attackMotionDuration, deltaTime);
        Decline(ref Current.continousAttackDuration, deltaTime);

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
    }

    #endregion
    /***********************************************************************
    *                              Player Actions
    ***********************************************************************/
    #region .
    /// <summary> 죽기 </summary>
    private void Die()
    {
        SetDeadState(true);
        PlayDeathAnimation();
    }
    /// <summary> 부활하기 </summary>
    private void Revive()
    {
        SetDeadState(false);
    }

    /// <summary> 캐릭터 모드 변경 </summary>
    private void ChangeBehaviorMode()
    {
        if (CharacterIsPlayingAttackMotion()) return;

        if (State.behaviorMode.Equals(BehaviorMode.None))
        {
            SetBehaviorMode(BehaviorMode.Battle);
        }
        else if (State.behaviorMode.Equals(BehaviorMode.Battle))
        {
            SetBehaviorMode(BehaviorMode.None);
        }
        ResetAnimation(); // 위아래 애니메이션 싱크 맞추기 위해 리셋
    }

    /// <summary> 공격 및 공격 애니메이션 재생 </summary>
    private void Attack()
    {
        if (!CharacterIsBattleMode()) return;
        if (OnAttackCooldown()) return;

        // 2가지 모션 이어서 재생
        if (Current.continousAttackDuration > 0 && Current.attackAnimationIndex == 0)
            Current.attackAnimationIndex = 1;
        else
            Current.attackAnimationIndex = 0;
        PlayAttackAnimation(Current.attackAnimationIndex);

        // 공격속도 적용
        Anim.SetFloat("Attack Speed", Speed.attackSpeed);

        // 쿨타임, 지속시간 세팅
        Current.attackCooldown = Cooldown.attack / Speed.attackSpeed;
        Current.attackMotionDuration = Duration.attack / Speed.attackSpeed;
        Current.continousAttackDuration = (Cooldown.attack + Duration.continousAttack) / Speed.attackSpeed;
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

        Vector3 next = _worldMoveDir * Speed.moveSpeed * (State.isRunning ? Speed.runSpeedMultiplier : 1f);

        RBody.velocity =
            new Vector3(next.x, RBody.velocity.y, next.z);

        UpdateMoveDirection(_moveDir);
    }

    /// <summary> 진행 방향으로 구르기 </summary>
    private void RollWASD()
    {
        if (CharacterIsUnableToMove()) return;
        if (OnAttackCooldown()) return;
        if (OnRollingCooldown()) return;

        // 공중 구르기 스킬이 없으면 땅에서만 구르기 가능
        if (!State.isGrounded && !Skill.airTumbling) return;

        Current.rollCooldown = Duration.roll + Cooldown.roll;
        Current.rollDuration = Duration.roll;
        SetRollState(true);

        StartCoroutine(RollRoutine());
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
            RBody.velocity.Set(0f, 0f, 0f);
        }
        RBody.AddForce(Vector3.up * Move.jumpForce, ForceMode.VelocityChange);
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
        }
    }

    private void Input_ChangeBehaviorMode()
    {
        if (Input.GetKeyDown(Key.changeBehaviorMode))
        {
            ChangeBehaviorMode();
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
        }
        if (Input.GetMouseButtonUp(2))
        {
            _isMouseMiddlePressed = false;

            if (_prevCursorVisibleState)
            {
                SetCursorVisibleState(true);
            }
        }

        // 2. Alt 눌러 커서 토글
        if (!_isMouseMiddlePressed && Input.GetKeyDown(Key.showCursorToggle))
        {
            State.isCursorVisible = !State.isCursorVisible;
            SetCursorVisibleState(State.isCursorVisible);
        }
    }

    float cur=0f;
    /// <summary> 마우스를 상하/좌우로 움직여서 회전 </summary>
    private void Input_RotatePlayer()
    {
        if (State.isCursorVisible && !_isMouseMiddlePressed)
            return;

        const float horizontalRotationFactor = 50f;
        const float verticalRotationFactor = 50f;

        // ================================================
        // 상하 : 카메라 Rig 회전
        float vDegree = -Input.GetAxisRaw("Mouse Y");
        float xRotPrev = _currentCam.Rig.localEulerAngles.x;
        float xRotNext = xRotPrev
            + vDegree
            * _currentCamOption.rotationSpeed
            * Time.deltaTime
            * verticalRotationFactor;

        if (xRotNext > 180f)
            xRotNext -= 360f;

        // ================================================
        // 좌우 : 캐릭터 회전 가능 상태 : 캐릭터 회전
        //        캐릭터 회전 불능 상태 : Rig 회전
        float hDegree = Input.GetAxisRaw("Mouse X");
        float yRotPrev = _currentCam.Rig.localEulerAngles.y;
        float yRotAdd =
            hDegree
            * _currentCamOption.rotationSpeed
            * Time.deltaTime
            * horizontalRotationFactor;
        float yRotNext = yRotAdd + yRotPrev;

        // 상하, 좌우 회전 가능 여부 판정
        bool xRotatable = 
            _currentCamOption.lookUpDegree < xRotNext &&
            _currentCamOption.lookDownDegree > xRotNext;
        bool yRotatable = !CharacterIsDead() && !CharacterIsStunned();// && !CharacterIsBinded();

        Vector3 nextRot = new Vector3
            (
                xRotatable ? xRotNext : xRotPrev,
                !yRotatable ? yRotNext : Mathf.SmoothDamp(yRotPrev, yRotPrev > 180f ? 360f : 0f, ref cur, 0.1f),
                0f
            );

        // Rig 회전 적용
        _currentCam.Rig.localEulerAngles = nextRot;

        // 캐릭터 회전 적용
        if (yRotatable && !CharacterIsRolling())
            transform.localEulerAngles += Vector3.up * yRotAdd;
    }

    /// <summary> TP Cam : 마우스 휠 굴려서 확대/축소 </summary>
    private void Input_CameraZoom()
    {
        if (State.currentView == CameraViewOption.FirstPerson)
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
        _worldMoveDir = transform.TransformDirection(_moveDir);

        bool isRunningKeyDown = Input.GetKey(Key.run);
        bool moving = _moveDir.magnitude > 0.1f;

        SetMovingState(moving);
        SetWalkingState(moving && !isRunningKeyDown);
        SetRunningState(moving && isRunningKeyDown);

        // 애니메이션 파라미터 설정
        float walkingMultiplier = State.isWalking ? 0.5f : 1f;
        Anim.SetFloat("Move X", _moveDir.x * walkingMultiplier);
        Anim.SetFloat("Move Z", _moveDir.z * walkingMultiplier);
    }

    #endregion
}