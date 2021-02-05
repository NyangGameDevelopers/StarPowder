using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-28 PM 5:15:37
// 작성자 : Rito

/// <summary> 키 바인딩을 위한 유저 행동 정의 - 키보드 </summary>
public enum UserAction
{
    MouseLeft,
    MouseRight,
    MouseMiddle,

    // WASD
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,

    // Behaviors
    Run,
    Dodge,
    Jump,

    // QuickSlots : Item, Skill, Emotion
    Quick1, // 1
    Quick2, // 2
    Quick3, // 3
    Quick4, // 4
    Quick5, // 5
    Quick6, // 6
    Quick7, // 7
    Quick8, // 8
    Quick9, // 9
    Quick0, // 0
    SubQuick1, // Q
    SubQuick2, // E
    SubQuick3, // R
    SubQuick4, // F

    // Toggles
    Toggle_ChangeBehaviorMode, // Tab
    Toggle_ShowCursor,         // Alt
    Toggle_ChangeCameraView,    // `

    // Switch
    ShowToolSet,  // V

    // UI
    UI_EquipAndStat, // U
    UI_Inventory,    // I
    UI_Skill,        // K
    UI_Emotion,      // F1


    // Temp
    Temp_RideOnVehicle, // G
}