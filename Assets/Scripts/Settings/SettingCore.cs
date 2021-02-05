using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-02 PM 4:30:40
// 작성자 : Rito

/// <summary> 설정 모음 </summary>
public class SettingCore : Rito.SingletonMonoBehavior<SettingCore>
{
    [field: SerializeField]
    public InputBinding Binding { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        InitBindings();
    }

    /***********************************************************************
    *                               Init Methods
    ***********************************************************************/
    #region .
    private void InitBindings()
    {
        Binding = new InputBinding();
        bool loadSuccess = Binding.LoadFromFile();

        if (!loadSuccess)
        {
            Binding.ResetAll();
            Binding.SaveToFile();
        }
    }

    #endregion
}