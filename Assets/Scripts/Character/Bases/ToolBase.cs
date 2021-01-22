using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-21 AM 1:54:49
// 작성자 : Rito

/// <summary> 캐릭터가 사용할 도구들 베이스 </summary>
public abstract class ToolBase : MonoBehaviour
{
    public HandType handType;

    /// <summary> 동작! </summary>
    public abstract void Act();
}
