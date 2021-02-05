using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-03 PM 9:48:07
// 작성자 : Rito

public class WeaponBox : MonoBehaviour
{
    /// <summary> 보관 중인 무기 목록 </summary>
    [field: SerializeField]
    public List<Weapon> WeaponList { get; private set; }

    /// <summary> 현재 선택된 장비 </summary>
    public Weapon CurrentWeapon
    {
        get
        {
            if (Count < 1) return null;
            return WeaponList[CurrentIndex];
        }
    }

    public int Count => WeaponList.Count;

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

    #endregion
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    private void Awake()
    {
        WeaponList = new List<Weapon>();

        var weapons = GetComponentsInChildren<Weapon>();
        if (weapons.Length > 0)
            WeaponList.AddRange(weapons);

        _currentIndex = 0;
    }

    #endregion
}