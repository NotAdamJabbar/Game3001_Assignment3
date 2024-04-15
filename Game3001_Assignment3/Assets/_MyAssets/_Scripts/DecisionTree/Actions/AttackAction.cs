using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fill in for Lab 7a.
public class AttackAction : ActionNode
{
    public AttackAction()
    {
        name = "Attack Action";
    }

    public override void Action()
    {
        //Enter functionality for action
        if(Agent.GetComponent<AgentObject>().state != ActionState.ATTACK)
        {
            Debug.Log("Starting " + name);
            AgentObject ao = Agent.GetComponent<AgentObject>();
            ao.state = ActionState.ATTACK;
            //Game.Instance.SOMA.PlaySound("Attacking");

            //Custom enter actions
            if (AgentScript is CloseCombatEnemy cce)
            {
                cce.DoAttack(true);
            }
            else if(AgentScript is RangedCombatEnemy rce)
            {

            }
        }

        //Every frame
        Debug.Log("Preforming " + name);
    }
}
