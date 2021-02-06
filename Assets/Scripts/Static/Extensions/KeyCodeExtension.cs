using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-02 PM 4:46:20
// 작성자 : Rito

namespace Extensions
{
    public static class KeyCodeExtension
    {
        public static bool GetKey(this KeyCode key)
            => Input.GetKey(key);
        public static bool KeyDown(this KeyCode key)
            => Input.GetKeyDown(key);
        public static bool KeyUp(this KeyCode key)
            => Input.GetKeyUp(key);
    }
}