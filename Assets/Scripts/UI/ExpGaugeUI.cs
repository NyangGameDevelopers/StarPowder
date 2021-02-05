using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 날짜 : 2021-02-03 AM 12:07:52
// 작성자 : Rito

/// <summary> 경험치 UI </summary>
public class ExpGaugeUI : ClockGaugeUI
{
    [SerializeField]
    protected TMP_Text _levelText;

    public void UpdateLevel(in int level)
    {
        _levelText.text = $"{level}";
    }
    protected override void UpdateValueText()
    {
        _valueText.text = $"{CurrentRatio:00.00%}";
    }
}