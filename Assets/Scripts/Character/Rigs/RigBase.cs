using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-19 PM 7:38:12
// 작성자 : Rito

public abstract class RigBase : MonoBehaviour
{
    /// <summary> Active False인 자식도 다 뒤져서 컴포넌트 찾아오기 </summary>
    public T GetComponentInAllChildren<T>() where T : Component
    {
        List<Transform> _childrenTrList = new List<Transform>();
        Recur_GetAllChildrenTransform(_childrenTrList, transform);

        foreach (var tr in _childrenTrList)
        {
            T found = tr.GetComponent<T>();
            if (found != null)
                return found;
        }
        return null;
    }
    private void Recur_GetAllChildrenTransform(List<Transform> trList, Transform tr)
    {
        trList.Add(tr);
        int childCount = tr.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Recur_GetAllChildrenTransform(trList, tr.GetChild(i));
        }
    }
}