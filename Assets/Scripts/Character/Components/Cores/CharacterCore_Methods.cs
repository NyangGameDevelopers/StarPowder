using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-08 PM 10:29:19
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                               Init Methods
    ***********************************************************************/
    #region .

    private void InitializeComponents()
    {
        // Gets
        RBody = GetComponent<Rigidbody>();
        Anim = GetComponentInChildren<Animator>();

        FPCam = GetComponentInAllChildren<FirstPersonCamera>();
        TPCam = GetComponentInAllChildren<ThirdPersonCamera>();
        FPCam.Init();
        TPCam.Init();

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

        SetCameraView(State.currentView); // 초기 뷰 설정
        SetCameraAlone(); // 카메라 한개 빼고 전부 비활성화
    }

    #endregion
    /***********************************************************************
    *                               Setter Methods
    ***********************************************************************/
    #region .

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

    private void SetCameraView(CameraViewOptions view)
    {
        State.currentView = view;
        bool isFP = view == CameraViewOptions.FirstPerson;

        FPCam.Cam.gameObject.SetActive(isFP);
        TPCam.Cam.gameObject.SetActive(!isFP);

        if (isFP)
        {
            _currentCam = FPCam;
            _currentCamOption = FPCamOption;
        }
        else
        {
            _currentCam = TPCam;
            _currentCamOption = TPCamOption;
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
    *                            Toggle Methods
    ***********************************************************************/
    #region .
    private void ToggleCameraView()
    {
        SetCameraView(State.currentView == CameraViewOptions.FirstPerson ?
            CameraViewOptions.ThirdPerson : CameraViewOptions.FirstPerson);
    }

    #endregion
    /***********************************************************************
    *                          Calculation Methods
    ***********************************************************************/
    #region .


    #endregion
    /***********************************************************************
    *                           Finder Methods
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
}