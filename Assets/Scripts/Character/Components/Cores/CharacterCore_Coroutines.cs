using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-09 PM 5:33:12
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                             Action Routines
    ***********************************************************************/
    #region .
    private IEnumerator RollRoutine()
    {
        Vector3 rollDir = _moveDir;
        Vector3 localDir;

        // WASD 입력 없으면 전방으로 구르기
        if (rollDir.magnitude < 0.1f)
            rollDir = Vector3.forward;

#if MOVE2
        if (CurrentIsTPCamera())
        {
            Anim.SetFloat("Roll Z", 1.0f);
            localDir = TPCam.Rig.TransformDirection(rollDir);
            var rot = Quaternion.LookRotation(localDir, Vector3.up).eulerAngles;
            Walker.localEulerAngles = Vector3.up * rot.y;
        }
        else
        {
            Anim.SetFloat("Roll X", rollDir.x);
            Anim.SetFloat("Roll Z", rollDir.z);

            localDir = Walker.TransformDirection(rollDir);
        }
#else
        // 애니메이션 파라미터 설정
        Anim.SetFloat("Roll X", rollDir.x);
        Anim.SetFloat("Roll Z", rollDir.z);

        localDir = Walker.TransformDirection(rollDir);
#endif
        while (State.isRolling)
        {
            float x = Current.rollDuration / Duration.roll;
            //x = Mathf.Sqrt(Mathf.Pow(x, 3f));
            Vector3 next = localDir * Speed.moveSpeed * Move.rollDistance * x;

            RBody.velocity =
                new Vector3(next.x, RBody.velocity.y, next.z);

            yield return null;
        }
        RBody.velocity = new Vector3(0f, RBody.velocity.y, 0f);
    }

    /// <summary> 현재 진행 중인 캐스팅 </summary>
    private Coroutine _castRoutine = null;
    /// <summary> 캐스팅 후 동작 수행 - duration : 캐스팅 소요 시간 </summary>
    private IEnumerator CastActionRoutine(float duration, Action action)
    {
        // 캐스트 시간 넣고
        SetCastDuration(duration);

        // 캐스팅..
        while (Current.castDuration > 0f)
        {
            Current.castDuration -= Time.deltaTime;

            yield return null;
        }

        // 캐스팅 성공 시 동작 수행
        SetCastDuration(0f);
        action();
    }

#endregion
    /***********************************************************************
    *                               Infinite Routines
    ***********************************************************************/
#region .
    private IEnumerator CameraZoomRoutine()
    {
        var wfs = new WaitForSeconds(0.005f);

        Transform tpCamTr = TPCam.transform;
        Transform tpCamRig = TPCam.Rig;

        while (true)
        {
            // Zoom In
            if (_tpCameraWheelInput > 0.01f)
            {
                for (float f = 0f; f < 0.3f;)
                {
                    // Zoom In 도중 Zoom Out 명령 받으면 종료
                    if (_tpCameraWheelInput < -0.01f) break;

                    float deltaTime = Time.deltaTime;
                    float zoom = deltaTime * TPCamOption.zoomSpeed;
                    float currentCamToRigDist = Vector3.Distance(tpCamTr.position, tpCamRig.position);

                    if (_tpCamZoomInitialDistance - currentCamToRigDist < TPCamOption.zoomInDistance)
                    {
                        tpCamTr.Translate(_tpCamToRigDir * zoom, Space.Self);
                    }

                    f += deltaTime;
                    yield return null;
                }

                Debug.Mark(_debugInputActionCall);
            }
            // Zoom Out
            else if (_tpCameraWheelInput < -0.01f)
            {
                for (float f = 0f; f < 0.3f;)
                {
                    // Zoom Out 도중 Zoom In 명령 받으면 종료
                    if (_tpCameraWheelInput > 0.01f) break;

                    float deltaTime = Time.deltaTime;
                    float zoom = deltaTime * TPCamOption.zoomSpeed;
                    float currentCamToRigDist = Vector3.Distance(tpCamTr.position, tpCamRig.position);

                    if (currentCamToRigDist - _tpCamZoomInitialDistance < TPCamOption.zoomOutDistance)
                    {
                        tpCamTr.Translate(-_tpCamToRigDir * zoom, Space.Self);
                    }

                    f += deltaTime;
                    yield return null;
                }

                Debug.Mark(_debugInputActionCall);
            }

            yield return wfs;
        }
    }

#endregion
}