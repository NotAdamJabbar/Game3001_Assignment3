using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class MoveToPlayerAction : ActionNode
{
    public MoveToPlayerAction()
    {
        name = "Move To Player Action";
    }

    public override void Action()
    {
        //Enter functionality for action
        if (Agent.GetComponent<AgentObject>().state != ActionState.MOVE_TO_PLAYER)
        {
            Debug.Log("Starting " + name);
            AgentObject ao = Agent.GetComponent<AgentObject>();
            ao.state = ActionState.MOVE_TO_PLAYER;

            //Custom enter actions
            if (AgentScript is EnemyController e)
            {

            }
        }

        //Every frame
        Debug.Log("Preforming " + name);
    }
}
