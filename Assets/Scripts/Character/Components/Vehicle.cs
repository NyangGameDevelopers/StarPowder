using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-19 PM 9:15:52
// 작성자 : Rito

/// <summary> 캐릭터가 탑승할 것 </summary>
public class Vehicle : MonoBehaviour
{
    /// <summary> 탑승물에 탔을 때 캐릭터의 상대 위치 </summary>
    public Vector3 characterLocalPosition;

    /// <summary> 이동속도 </summary>
    public float speed = 3f;

    /// <summary> 탑승 중 부스트(달리기) 이속 배수
    /// <para/> 달리기 못하게 하려면 1로 세팅 </summary>
    public float runMultiplier = 1f;
}