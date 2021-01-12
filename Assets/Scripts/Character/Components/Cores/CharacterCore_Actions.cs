using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

// 날짜 : 2021-01-08 PM 9:23:05
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                             Animation Players
    ***********************************************************************/
    #region .

    private void PlayIdleAnimation() => Anim.Play(AnimationName.idle);
    private void PlayWalkAnimation() => Anim.Play(AnimationName.walk);
    private void PlayRunAnimation() => Anim.Play(AnimationName.run);

    #endregion

    /***********************************************************************
    *                              Player Actions
    ***********************************************************************/
    #region .
    /// <summary> 키보드 WASD 이동 </summary>
    private void MoveByKeyboard()
    {
        Vector3 localMoveDir = transform.TransformDirection(_moveDir).normalized;
        RBody.velocity = localMoveDir * Move.moveSpeed * (State.isRunning ? Move.runSpeedMultiplier : 1f);
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

    /// <summary> 마우스를 상하/좌우로 움직여서 카메라 회전 </summary>
    private void Input_RotatePlayerCamera()
    {
        if (State.isCursorVisible && !_isMouseMiddlePressed)
            return;

        float horizontalRotationFactor = 50f;
        float verticalRotationFactor = 50f;

        // 좌우 : 캐릭터 회전
        float hDegree = Input.GetAxisRaw("Mouse X");
        transform.localEulerAngles += 
            Vector3.up 
            * hDegree 
            * _currentCamOption.rotationSpeed 
            * Time.deltaTime 
            * horizontalRotationFactor;

        // 상하 : 카메라 Rig 회전
        float vDegree = -Input.GetAxisRaw("Mouse Y");
        float prevXRot = _currentCam.Rig.localEulerAngles.x;
        float nextXRot = prevXRot
            + vDegree
            * _currentCamOption.rotationSpeed
            * Time.deltaTime
            * verticalRotationFactor;

        if (nextXRot > 180f)
            nextXRot -= 360f;

        if (_currentCamOption.lookUpDegree < nextXRot &&
            _currentCamOption.lookDownDegree > nextXRot)
        {
            _currentCam.Rig.localEulerAngles = Vector3.right * nextXRot;
        }

        // TODO : FP인 경우 머리도 같이 돌려주기
        //if (State.currentView == CameraViewOptions.FirstPerson)
        //{
        //    CameraFP.headTran.localEulerAngles = currentCam.rig.localEulerAngles;
        //}
    }

    /// <summary> TP Cam : 마우스 휠 굴려서 확대/축소 </summary>
    private void Input_CameraZoom()
    {
        if (State.currentView == CameraViewOptions.FirstPerson)
            return;

        _tpCameraWheelInput = Input.GetAxis("Mouse ScrollWheel");
    }

    /// <summary> WASD, LShift 입력으로 이동 벡터, 이동 상태 정의 </summary>
    private void Input_CalculateKeyMoveDir()
    {
        _moveDir = Vector3.zero;

        if (Input.GetKey(Key.moveForward)) _moveDir += Vector3.forward;
        if (Input.GetKey(Key.moveBackward)) _moveDir += Vector3.back;
        if (Input.GetKey(Key.moveLeft)) _moveDir += Vector3.left;
        if (Input.GetKey(Key.moveRight)) _moveDir += Vector3.right;

        bool isRunningKeyDown = Input.GetKey(Key.run);
        bool moving = _moveDir.magnitude > 0.1f;

        State.isMoving = moving;
        State.isWalking = moving && !isRunningKeyDown;
        State.isRunning = moving && isRunningKeyDown;
    }

    #endregion
}