using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class RandomCondition : ConditionNode
{
    public bool StartIdle { get; set; }
    public RandomCondition()
    {
        name = "LOS Condition";
        StartIdle = false;
    }

    public override bool Condition()
    {
        Debug.Log("Checking " + name);
        return StartIdle;
    }
}
