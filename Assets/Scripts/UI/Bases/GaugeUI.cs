using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 날짜 : 2021-02-02 PM 5:46:59
// 작성자 : Rito

/// <summary> 게이지 UI 베이스 클래스 </summary>
public abstract class GaugeUI : UserInterface
{
    [SerializeField]
    protected TMP_Text _valueText;

    public float MaxValue { get; protected set; }
    public float CurrentValue { get; protected set; }

	public float CurrentRatio => CurrentValue / MaxValue;

    /// <summary> 게임 시작 시 호출 : 값 초기화 </summary>
    public virtual void InitValues(in float currentValue, in float maxValue)
    {
        CurrentValue = currentValue;
        MaxValue = maxValue;

        UpdateValueText();
    }

    public virtual void UpdateMaxValue(in float value)
    {
        MaxValue = value;
        UpdateValueText();
    }
    public virtual void UpdateCurrentValue(in float value)
    {
        CurrentValue = value;
        UpdateValueText();
    }

    protected abstract void UpdateValueText();
}