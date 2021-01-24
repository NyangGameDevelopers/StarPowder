using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 날짜 : 2021-01-22 AM 3:33:26
// 작성자 : Rito

/// <summary> 빗자루 : 타고 다니기용 </summary>
public class Broom : Tool
{
    public override void Act(out ToolActionResult result)
    {
        result = ToolActionResult.Fail();
    }
}