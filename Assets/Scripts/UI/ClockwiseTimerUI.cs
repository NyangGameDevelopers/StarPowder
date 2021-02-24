using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 날짜 : 2021-02-10 PM 10:28:35
// 작성자 : Rito

public class ClockwiseTimerUI : UserInterface
{
    [SerializeField] private Image _fillImage;

    /// <summary> 0 ~ 1 사이로 값 업데이트 </summary>
    public void UpdateValue(in float ratio)
    {
        _fillImage.fillAmount = (1 - ratio);
    }
}