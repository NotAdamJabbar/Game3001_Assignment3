using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : ActionNode
{
    public IdleAction()
    {
        name = "Idle Action";
    }

    public override void Action()
    {
        //Enter functionality for action
        if (Agent.GetComponent<AgentObject>().state != ActionState.IDLE)
        {
            Debug.Log("Starting " + name);
            AgentObject ao = Agent.GetComponent<AgentObject>();
            ao.state = ActionState.IDLE;

            //Custom enter actions
            if (AgentScript is EnemyController e)
            {
                e.StartIdle();
            }
        }

        //Every frame
        Debug.Log("Preforming " + name);
    }
}
