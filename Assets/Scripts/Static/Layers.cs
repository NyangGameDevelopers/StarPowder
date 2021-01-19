using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-13 PM 8:00:19
// 작성자 : Rito

/// <summary> 레이어, 레이어 마스크 보관 </summary>
public static class Layers
{
    public const int Default = 0;
    public const int GroundLayer = 8;
    public const int CharacterLayer = 9;

    public const int AllMask = -1;
    public const int GroundMask = 1 << GroundLayer;
    public const int CharacterMask = 1 << CharacterLayer;
}