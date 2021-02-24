using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 날짜 : 2021-02-02 PM 5:29:20
// 작성자 : Rito

/// <summary> 유저 인터페이스 총 관리 싱글톤 매니저 클래스 </summary>
public class UICore : Rito.SingletonMonoBehavior<UICore>
{
    /***********************************************************************
    *                               UI Properties
    ***********************************************************************/
    #region .

    [field: Header("Left Top Gauges")]
    [field: SerializeField]
    public GaugeUI HpGauge { get; private set; }

    [field: SerializeField]
    public GaugeUI MpGauge { get; private set; }

    [field: SerializeField]
    public ExpGaugeUI ExpGauge { get; private set; }


    [field: Header("Left Top Gauges")]
    [field: SerializeField]
    public ClockwiseTimerUI[] StateTimers { get; private set; }

    [field: Header("Center Pancake")]
    [field: SerializeField]
    public PancakeUI Pancake { get; private set; }

    public TMP_Text _pancakeIndexText;

    #endregion
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .
    protected override void Awake()
    {
        base.Awake();
    }

    #endregion
}