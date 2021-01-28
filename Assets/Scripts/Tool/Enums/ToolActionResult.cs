using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-21 PM 9:19:33
// 작성자 : Rito

/// <summary> 도구 액션의 결과 전달 </summary>
public struct ToolActionResult
{
    /// <summary> 액션이 성공했는지 여부 </summary>
    public bool succeeded;

    /// <summary> 캐릭터에게 적용될 쿨타임 </summary>
    public float coolDown;

    /// <summary> 캐릭터가 재생할 애니메이션 인덱스(기본 : 1) </summary>
    public int motionIndex;


    /// <summary> 실패 시 결과 전달 </summary>
    public static ToolActionResult Fail()
    {
        return new ToolActionResult { succeeded = false };
    }

    /// <summary> 성공 시 결과 전달 </summary>
    public static ToolActionResult Success(in float cooldown)
    {
        return new ToolActionResult
        {
            succeeded = true,
            coolDown = cooldown,
            motionIndex = 1
        };
    }

    /// <summary> 성공 시 결과 전달 </summary>
    public static ToolActionResult Success(in float cooldown, in int motionindex)
    {
        return new ToolActionResult
        {
            succeeded = true,
            coolDown = cooldown,
            motionIndex = motionindex
        };
    }
}