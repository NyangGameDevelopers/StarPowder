using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-02 PM 9:56:50
// 작성자 : Rito

/// <summary> UI 기본 클래스 </summary>
public abstract class UserInterface : MonoBehaviour
{
    public void ShowUI() => gameObject.SetActive(true);
    public void HideUI() => gameObject.SetActive(false);
}