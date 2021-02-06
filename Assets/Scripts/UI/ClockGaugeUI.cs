using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 날짜 : 2021-02-02 PM 10:38:42
// 작성자 : Rito

/// <summary> 시계방향으로 진행하는 UI </summary>
public class ClockGaugeUI : GaugeUI
{
    [SerializeField]
    protected Image _fillImage;


    /// <summary> 게임 시작 시 호출 : 값 초기화 </summary>
    public override void InitValues(in float currentValue, in float maxValue)
    {
        base.InitValues(currentValue, maxValue);
        _fillImage.fillAmount = CurrentRatio;
    }

    public override void UpdateMaxValue(in float value)
    {
        base.UpdateMaxValue(value);
        _fillImage.fillAmount = CurrentRatio;
    }
    public override void UpdateCurrentValue(in float value)
    {
        base.UpdateCurrentValue(value);
        _fillImage.fillAmount = CurrentRatio;
    }

    protected override void UpdateValueText()
    {
        _valueText.text = $"{CurrentValue:F0}";
    }
}