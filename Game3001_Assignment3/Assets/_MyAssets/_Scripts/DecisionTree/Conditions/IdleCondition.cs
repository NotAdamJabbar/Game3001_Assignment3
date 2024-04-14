using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleCondition : ConditionNode
{
    public bool IsIdle { get; set; }
    public IdleCondition()
    {
        name = "Idle Condition";
        IsIdle = false;
    }

    public override bool Condition()
    {
        Debug.Log("Checking " + name);
        return IsIdle;
    }
}
