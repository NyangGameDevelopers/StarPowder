using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-20 PM 10:54:41
// 작성자 : Rito

// 무기(임시)
public class Weapon : ToolBase
{
    public string weaponName;

    public Transform RightWeapon { get; private set; }
    public Transform LeftWeapon { get; private set; }

    /// <summary> 오른손만 쓰는 무기인가요? </summary>
    public bool IsOneHanded() => handType == HandType.OneHand;
    /// <summary> 양손에 하나씩 쥐고 있나요 </summary>
    public bool IsTwoHanded() => handType == HandType.TwoHand;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            var left = child.GetComponent<LeftWeaponMark>();
            if (left != null)
                LeftWeapon = left.transform;

            var right = child.GetComponent<RightWeaponMark>();
            if (right != null)
                RightWeapon = right.transform;
        }
    }

    public override void Act()
    {
    }
}