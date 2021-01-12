using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rito.BehaviorTree;

// 날짜 : 2021-01-08 PM 9:22:10
// 작성자 : Rito

public partial class CharacterCore : MonoBehaviour
{
    /***********************************************************************
    *                        Core - Condition Nodes
    ***********************************************************************/
    #region .

    private bool PlayerIsWalking() => State.isWalking;

    private bool PlayerIsRunning() => State.isRunning;

    #endregion
}