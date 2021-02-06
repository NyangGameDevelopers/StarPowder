using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-22 AM 1:35:03
// 작성자 : Rito

/// <summary> 임시 : 장비 목록 관리 및 캐릭터 상호작용 클래스 </summary>
public class ToolBox : MonoBehaviour
{
    /// <summary> 보관 중인 장비 목록 </summary>
    [field: SerializeField]
    public List<Tool> ToolList { get; private set; }

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
        set
        {
            if (_currentIndex >= Count)
            {
                _currentIndex = 0;
            }
            _currentIndex = value;
        }
    }
    private int _currentIndex;

    /***********************************************************************
    *                               Methods
    ***********************************************************************/
    #region .

    #endregion
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Awake()
    {
        ToolList = new List<Tool>();

        var tools = GetComponentsInChildren<Tool>();
        foreach (var tool in tools)
        {
            if (tool.enabled)
                ToolList.Add(tool);
        }

        _currentIndex = 0;
    }

    #endregion
}