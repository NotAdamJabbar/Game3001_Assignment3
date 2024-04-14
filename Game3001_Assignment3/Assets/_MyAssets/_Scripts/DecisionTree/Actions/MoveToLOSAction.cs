using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class MoveToLOSAction : ActionNode
{
    public MoveToLOSAction()
    {
        name = "Move To LOS Action";
    }

    public override void Action()
    {
        //Enter functionality for action
        if (Agent.GetComponent<AgentObject>().state != ActionState.MOVE_TO_LOS)
        {
            Debug.Log("Starting " + name);
            AgentObject ao = Agent.GetComponent<AgentObject>();
            ao.state = ActionState.MOVE_TO_LOS;

            //Custom enter actions
            if (AgentScript is CloseCombatEnemy cce)
            {

            }
            else if (AgentScript is RangedCombatEnemy rce)
            {

            }
        }

        //Every frame
        Debug.Log("Preforming " + name);
    }
}
