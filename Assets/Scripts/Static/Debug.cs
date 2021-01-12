using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-09 PM 4:47:05
// 작성자 : Rito

// 설명 : Deubg 클래스를 Conditional("UNITY_EDITOR")로 래핑하여
// 빌드 후에는 디버그가 실행되지 않게 함
// 성능 상 많은 이득

/// <summary> 에디터 전용 디버그 래퍼 클래스 </summary>
public static class Debug
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(object msg)
        => UnityEngine.Debug.Log(msg);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(object msg)
        => UnityEngine.Debug.LogWarning(msg);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(object msg)
        => UnityEngine.Debug.LogError(msg);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
        => UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
        => UnityEngine.Debug.DrawRay(start, dir, color, duration);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
        => UnityEngine.Debug.DrawRay(start, dir, color);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir)
        => UnityEngine.Debug.DrawRay(start, dir);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end)
        => UnityEngine.Debug.DrawLine(start, end);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
        => UnityEngine.Debug.DrawLine(start, end, color);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        => UnityEngine.Debug.DrawLine(start, end, color, duration);

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
        => UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    /// <summary>
    /// 메소드 호출 전파 추적용 메소드
    /// </summary>
    public static void Mark(
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
    )
    {
        int begin = sourceFilePath.LastIndexOf(@"\");
        int end = sourceFilePath.LastIndexOf(@".cs");
        string className = sourceFilePath.Substring(begin + 1, end - begin - 1);

        UnityEngine.Debug.Log($"[Mark] {className}.{memberName}, {sourceLineNumber}");
    }
}
