using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-21 PM 9:46:54
// 작성자 : Rito

/// <summary> 들고 있는 손 타입 </summary>
public enum HandType
{
    /// <summary> 오른손에만 들고 있음 </summary>
    RightHand,

    /// <summary> 양손으로 들고 있음(무기 프랍은 오른손에 위치) </summary>
    DoubleHand,

    /// <summary> 양 손에 각각 하나씩 들고 있음 </summary>
    TwoHand,
}