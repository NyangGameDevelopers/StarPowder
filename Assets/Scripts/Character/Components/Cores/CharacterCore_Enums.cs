using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

// 날짜 : 2021-01-12 AM 1:16:19
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                        Core - Enum Definitions
    ***********************************************************************/
    public enum MouseButton
    {
        Left, Right, Middle
    }

    public enum CameraViewOption
    {
        FirstPerson,
        ThirdPerson
    }

    /// <summary> 캐릭터의 이동 방향 </summary>
    public enum MoveDirection
    {
        None, // 정지
        Front,
        FrontLeft,
        FrontRight,
        Left,
        Right,
        Back,
        Backleft,
        BackRight
    }

    public enum BehaviorMode
    {
        None,
        Battle,
        Witch,     // 마녀에요
        OnVehicle, // 탑승 중
    }

    public enum AnimType
    {
        None,
        Idle,
        Move,
        Roll,
        Bind,
        Stun,
        Die,
        Emotion,
    }

    /// <summary> 장착한 도구 타입 </summary>
    public enum ToolType
    {
        None,
        Weapon,
    }
}

/***********************************************************************
*                               Global Enums
***********************************************************************/
#region .
/// <summary> 들고 있는 손 타입 </summary>
public enum HandType
{
    OneHand, // 오른손에만 들고 있음
    TwoHand  // 양 손에 하나씩 들고 있음
}

#endregion
