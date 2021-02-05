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
        /// <summary> 평시 </summary>
        Normal,
        /// <summary> 전투모드 </summary>
        Battle,
        /// <summary> 탑승 중 </summary>
        OnVehicle,
        /// <summary> 빗자루 타는 중 </summary>
        Witch,
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
}
