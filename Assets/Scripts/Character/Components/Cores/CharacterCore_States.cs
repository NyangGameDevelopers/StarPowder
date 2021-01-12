using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-08 PM 9:32:38
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /**************************************************************
     *                         Core - States
     **************************************************************/

    [Serializable]
    public class PlayerState
    {
        public bool isDead;

        /// <summary> 캐릭터가 걷거나 뛰고 있는지 여부 </summary>
        public bool isMoving;

        /// <summary> 캐릭터가 걷고 있는지 여부 </summary>
        public bool isWalking;

        /// <summary> 캐릭터가 뛰는고 있는지 여부 </summary>
        public bool isRunning;

        /// <summary> 현재 커서가 보이는지 여부 </summary>
        public bool isCursorVisible;

        /// <summary> 현재 선택된 카메라 뷰 </summary>
        public CameraViewOptions currentView = CameraViewOptions.ThirdPerson;
    }
    [SerializeField]
    public PlayerState _state = new PlayerState();
    public PlayerState State => _state;
}