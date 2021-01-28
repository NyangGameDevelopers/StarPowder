using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-21 AM 1:54:49
// 작성자 : Rito

/// <summary> 캐릭터가 사용할 도구들 베이스 </summary>
public abstract class Tool : MonoBehaviour
{
    /***********************************************************************
    *                               Fields, Properties
    ***********************************************************************/
    #region .

    [Tooltip("도구 아이템 이름")]
    public string toolName;

    /// <summary> 오른손만 사용, 양손 사용, 양손 각각 사용 여부 </summary>
    public HandType handType;

    /// <summary> 오른손에 드는 장비 트랜스폼 </summary>
    public Transform RightHandTool { get => _rightHandTool; protected set => _rightHandTool = value; }

    /// <summary> 왼손에 드는 장비 트랜스폼 </summary>
    public Transform LeftHandTool { get => _leftHandTool; protected set => _leftHandTool = value; }

    // 인스펙터 확인용
    [SerializeField] protected Transform _rightHandTool;
    [SerializeField] protected Transform _leftHandTool;

    #endregion
    /***********************************************************************
    *                               Protected Methods
    ***********************************************************************/
    #region .
    // 왼쪽, 오른쪽 장비 트랜스폼 초기화
    protected virtual void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            var left = child.GetComponent<LeftToolMark>();
            if (left != null)
            {
                LeftHandTool = left.transform;
                LeftHandTool.gameObject.SetActive(false);
            }

            var right = child.GetComponent<RightToolMark>();
            if (right != null)
            {
                RightHandTool = right.transform;
                RightHandTool.gameObject.SetActive(false);
            }
        }
    }

    /// <summary> 로컬 포지션, 로컬 로테이션 초기화 </summary>
    protected void ResetLocalTransform(Transform transform)
    {
        transform.localPosition = default;
        transform.localRotation = default;
    }

    #endregion
    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .

    /// <summary> 장비 손꾸락 타입에 따라 손에 착용시켜주기 </summary>
    public void PutOn(LeftHandMark leftHand, RightHandMark rightHand)
    {
        // 양손 각각 사용의 경우에만 왼손을 씀
        if (handType == HandType.TwoHand)
        {
            LeftHandTool.gameObject.SetActive(true);
            LeftHandTool.SetParent(leftHand.transform);
            ResetLocalTransform(LeftHandTool);
        }

        // 어떤 경우에도 오른손은 항상 사용
        RightHandTool.gameObject.SetActive(true);
        RightHandTool.SetParent(rightHand.transform);
        ResetLocalTransform(RightHandTool);
    }

    /// <summary> 손에서 장비를 벗겨오기 </summary>
    public void TakeOff()
    {
        if (handType == HandType.TwoHand)
        {
            LeftHandTool.gameObject.SetActive(false);
            LeftHandTool.SetParent(transform);
            ResetLocalTransform(LeftHandTool);
        }

        RightHandTool.gameObject.SetActive(false);
        RightHandTool.SetParent(transform);
        ResetLocalTransform(RightHandTool);
    }

    #endregion

    /// <summary> 동작 수행 및 수행 결과 전달 </summary>
    public abstract void Act(out ToolActionResult result);
}
