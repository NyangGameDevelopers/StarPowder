using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-22 AM 1:35:03
// 작성자 : Rito

/// <summary> 임시 : 장비 목록 관리 및 캐릭터 상호작용 클래스 </summary>
public class Temp_ToolBox : MonoBehaviour
{
    /// <summary> 보관 중인 장비 목록 </summary>
    public List<Tool> ToolList { get => _toolList; private set => _toolList = value; }
    [SerializeField]
    private List<Tool> _toolList; // 인스펙터 확인용

    /// <summary> 현재 선택된 장비 </summary>
    public Tool CurrentTool
    {
        get
        {
            if (Count < 1) return null;
            return ToolList[CurrentIndex];
        }
    }

    public int Count => ToolList.Count;

    /// <summary> 현재 선택된 장비 인덱스 </summary>
    public int CurrentIndex 
    {
        get
        {
            // 인덱스가 벗어난 경우 처음 인덱스 선택
            if (_currentIndex >= Count)
            {
                _currentIndex = 0;
            }
            return _currentIndex;
        }
        set => _currentIndex = value; 
    }
    private int _currentIndex;

    /***********************************************************************
    *                               Methods
    ***********************************************************************/
    #region .
    /// <summary> 현재 장비 착용시켜주기 </summary>
    public void PutOn(LeftHandMark leftHand, RightHandMark rightHand)
    {
        if (Count < 1) return;
        ToolList[CurrentIndex].PutOn(leftHand, rightHand);
    }

    /// <summary> 현재 장비 벗기기 </summary>
    public void TakeOff()
    {
        if (Count < 1) return;
        ToolList[CurrentIndex].TakeOff();
    }

    /// <summary> 현재 장비 벗고 다음 장비 장착 </summary>
    public void SwitchNextTool(LeftHandMark leftHand, RightHandMark rightHand)
    {
        if (Count <= 1) return; // 장비가 하나 이하인 경우 스위칭 불가

        int prev = CurrentIndex;
        CurrentIndex += 1;

        ToolList[prev].TakeOff();
        ToolList[CurrentIndex].PutOn(leftHand, rightHand);
    }

    #endregion
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Awake()
    {
        ToolList = new List<Tool>();

        var tools = GetComponentsInChildren<Tool>();
        if (tools.Length > 0)
            ToolList.AddRange(tools);

        _currentIndex = 0;
    }

    #endregion
}