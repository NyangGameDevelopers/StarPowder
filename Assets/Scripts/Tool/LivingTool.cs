using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-02-05 PM 9:25:32
// 작성자 : Rito

public class LivingTool : Tool
{
    public override void Act(out ToolActionResult result)
    {
        result = ToolActionResult.Fail();
    }
}