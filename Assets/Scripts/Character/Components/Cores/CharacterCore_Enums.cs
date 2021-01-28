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
        /// <summary> 빈 손 </summary>
        None,
        /// <summary> 도구 들고 있음 </summary>
        Equip,
        /// <summary> 빗자루 타는 중 </summary>
        Witch,
        /// <summary> 탑승 중 </summary>
        OnVehicle,
        // TODO : 건축
        Build,
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
