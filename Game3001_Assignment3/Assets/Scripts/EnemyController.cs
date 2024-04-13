using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : AgentObject
{
    DecisionTree dt;
    Rigidbody2D rb;
    int patrolIndex;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float movementSpeed, rotationSpeed, pointRadius;
    bool canDecide = true;

    // Start is called before the first frame update
    void Start()
    {
        dt = new DecisionTree(this.gameObject);
        BuildTree();
        rb = GetComponent<Rigidbody2D>();
        dt.RandomNode.StartIdle = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dt.LOSNode.HasLOS != true&&canDecide)
        {
            canDecide = false;
            Invoke("DecideIdle", 5f);
        }

        dt.MakeDecision();

        switch (state)
        {
            case ActionState.PATROL:
                SeekForward();
                break;
            case ActionState.IDLE:
                StartIdle();
                break;
            case ActionState.MOVE_TO_PLAYER:
                
                break;
            default:
                rb.velocity = Vector3.zero;
                break;
        }
    }

    private void DecideIdle()
    {
        canDecide = true;
        dt.RandomNode.StartIdle = Random.Range(0, 2) == 1;
    }

    private void SeekForward() // A seek with rotation to target but only moving along forward vector.
    {
        // Calculate direction to the target.
        Vector2 directionToTarget = (TargetPosition - transform.position).normalized;

        // Calculate the angle to rotate towards the target.
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90.0f; // Note the +90 when converting from Radians.

        // Smoothly rotate towards the target.
        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationAmount);

        // Move along the forward vector using Rigidbody2D.
        rb.velocity = transform.up * movementSpeed;

        // TODO: New for Lab 7a. Continue patrol.
        if (Vector3.Distance(transform.position, TargetPosition) <= pointRadius)
        {
            m_target = GetNextPatrolPoint();
        }
    }

    // TODO: Add for Lab 7a.
    public void StartPatrol()
    {
        m_target = patrolPoints[patrolIndex];
    }

    // TODO: Add for Lab 7a.
    private Transform GetNextPatrolPoint()
    {
        patrolIndex++;
        if (patrolIndex == patrolPoints.Length)
        {
            patrolIndex = 0;
        }

        return patrolPoints[patrolIndex];
    }

    public void StartIdle()
    {
        rb.velocity = Vector2.zero;
    }

    private void BuildTree()
    {
        dt.RandomNode = new RandomCondition();
        dt.treeNodeList.Add(dt.RandomNode);

        TreeNode IdleNode = dt.AddNode(dt.RandomNode, new IdleAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)IdleNode).SetAgent(this.gameObject, typeof(EnemyController));
        dt.treeNodeList.Add(IdleNode);

        dt.LOSNode = new LOSCondition();
        dt.treeNodeList.Add(dt.RandomNode);

        TreeNode patrolNode = dt.AddNode(dt.LOSNode, new PatrolAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)patrolNode).SetAgent(this.gameObject, typeof(EnemyController));
        dt.treeNodeList.Add(patrolNode);

        TreeNode MoveToPlayerNode = dt.AddNode(dt.LOSNode, new MoveToPlayerAction(), TreeNodeType.RIGHT_TREE_NODE);
        ((ActionNode)MoveToPlayerNode).SetAgent(this.gameObject, typeof(EnemyController));
    }
}
