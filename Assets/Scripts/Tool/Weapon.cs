using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-20 PM 10:54:41
// 작성자 : Rito

/*
    [공격 메커니즘 - 공격 모션이 세 개인 경우 예시]

    * 쿨타임은 캐릭터에서 계산되며, 현재 쿨타임 > 0 인 경우 공격하지 못함

    [1] 캐릭터에서 공격 수행 : Current.tool.Act(out result); 호출
    [2] 무기에서 ToolActionResult.Success(cooldownFirst, 1) 응답
    [3] 캐릭터가 1번째 공격모션 재생
    [4] 캐릭터에 cooldownFirst(0.3) 쿨타임 카운트 적용
    [5] 무기에 cooldownFirst + chainFirstToSecond (= 0.6) 카운트 적용
    [6] cooldownFirst(0.3) 시간이 지난 이후,

      [7-1] 또는 [7-3]으로 이동

    [7-1] 캐릭터가 chainFirstToSecond(0.3) 시간을 흘려보낸 경우 (연계 공격 기회를 놓침)
    [7-2] 다음 공격 시 다시 첫 번째 공격 수행하게 됨

    [7-3] 캐릭터가 chainFirstToSecond(0.3) 이내에 공격한 경우
          (캐릭터에서 Current.tool.Act(out result); 호출)
    [8] 무기에서 ToolActionResult.Success(cooldownSecond, 1) 응답
    [9] 캐릭터가 2번째 공격모션 재생
    [10] 캐릭터에 cooldownSecond(0.3) 쿨타임 적용
    [11] 무기에 cooldownSecond + chainSecondToThird (= 0.6) 카운트 적용
    [12] cooldownSecond(0.3) 시간이 지난 이후,

      [13-1] 또는 [13-3]으로 이동

    [13-1] 캐릭터가 chainSecondToThird(0.3) 시간을 흘려보낸 경우 (연계 공격 기회를 놓침)
    [13-2] 다음 공격 시 다시 첫 번째 공격 수행하게 됨

    [13-3] 캐릭터가 chainSecondToThird(0.3) 이내에 공격한 경우
           (캐릭터에서 Current.tool.Act(out result); 호출)
    [14] 무기에서 ToolActionResult.Success(cooldownThird, 3) 응답
    [15] 캐릭터가 3번째 공격모션 재생
    [16] 캐릭터에 cooldownThird+cooldownLast(0.9) 쿨타임 적용

*/

/// <summary> 무기 기초 클래스 </summary>
public class Weapon : Tool
{
    [Space]
    [Range(1, 3), Tooltip("공격 모션 개수")]
    public int motionCount = 1;


    /***********************************************************************
    *                               Cooldowns
    ***********************************************************************/
    #region .
    [Space]
    [Header("쿨타입 옵션들"), Tooltip("첫 번째 공격 이후 적용되는 쿨타임")]
    public float cooldownFirst  = 0.3f;
    [Tooltip("두 번째 공격 이후 적용되는 쿨타임")]
    public float cooldownSecond = 0.3f;
    [Tooltip("세 번째 공격 이후 적용되는 쿨타임")]
    public float cooldownThird  = 0.3f;
    [Tooltip("마지막 모션의 공격 이후 추가 적용되는 쿨타임")]
    public float cooldownLast  = 0.6f;

    [Space]
    [Tooltip("첫 번째 공격 이후 두 번째 공격 연계가 허용되는 시간")]
    public float chainFirstToSecond = 0.3f;

    [Tooltip("두 번째 공격 이후 세 번째 공격 연계가 허용되는 시간")]
    public float chainSecondToThird = 0.3f;

    #endregion
    /***********************************************************************
    *                               Current Values (Private)
    ***********************************************************************/
    #region .
    private int currentMotionIndex = 1; // 현재 공격할 모션 인덱스
    private float remainedChance;       // 연계 공격 가능 시간

    #endregion
    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region .

    // 공격 연계 찬스 시간 카운트
    protected virtual void Update()
    {
        if (remainedChance > 0)
        {
            remainedChance -= Time.deltaTime;

            // 연계 공격 찬스를 놓친 경우, 모션 인덱스 1로 초기화
            if (remainedChance <= 0)
            {
                remainedChance = -1f;
                currentMotionIndex = 1;
            }
        }
    }

    #endregion
    /***********************************************************************
    *                               Methods
    ***********************************************************************/
    #region .
    public override void Act(out ToolActionResult result)
    {
        // NOTE : 공격 쿨타임 계산은 도구 사용 자체의 쿨타임 적용을 위해
        // 캐릭터에서 수행

        // 캐릭터에 전달할 값들
        int motionIndex = currentMotionIndex;
        float cooldown = cooldownLast;

        switch (motionIndex)
        {
            // 첫 번째 공격
            case 1:
                // 두 번째 공격 모션 존재 시, 연계 찬스 부여
                if (motionCount > 1)
                {
                    SetChance(cooldownFirst + chainFirstToSecond);
                    currentMotionIndex = 2;
                    cooldown = cooldownFirst;
                }
                // 두 번째 공격 모션이 없을 경우, 1로 유지
                else
                {
                    cooldown = cooldownFirst + cooldownLast;
                }
                break;

            // 두 번째 공격
            case 2:
                // 세 번째 공격 모션 존재 시, 연계 찬스 부여
                if (motionCount > 2)
                {
                    SetChance(cooldownSecond + chainSecondToThird);
                    currentMotionIndex = 3;
                    cooldown = cooldownSecond;
                }
                // 세 번째 공격 모션이 없을 경우, 1로 초기화
                else
                {
                    currentMotionIndex = 1;
                    cooldown = cooldownSecond + cooldownLast;
                }
                break;

            // 세 번째 공격
            case 3:
                // 공격 모션 1로 초기화
                currentMotionIndex = 1;
                cooldown = cooldownThird + cooldownLast;
                break;
        }

        // 캐릭터에 응답
        result = ToolActionResult.Success(cooldown, motionIndex);

        // 공격 수행
        Attack();
    }

    /// <summary> 연계 공격 시간 부여 </summary>
    private void SetChance(in float seconds)
    {
        remainedChance = seconds;
    }

    // TODO : 공격 수행
    private void Attack() { }

    #endregion

}