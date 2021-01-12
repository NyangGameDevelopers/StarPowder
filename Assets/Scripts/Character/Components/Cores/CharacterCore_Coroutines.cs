using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-09 PM 5:33:12
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                           Core - Coroutines
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
            }

            yield return wfs;
        }
    }

    #endregion
}