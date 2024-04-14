using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class RangedCombatEnemy : AgentObject
{
    // TODO: Add for Lab 7a.

    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float pointRadius;

    [SerializeField] float movementSpeed; // TODO: Uncomment for Lab 7a.
    [SerializeField] float rotationSpeed;
    [SerializeField] float whiskerLength;
    [SerializeField] float whiskerAngle;

    [SerializeField] float detectRange;
    // [SerializeField] float avoidanceWeight;
    private Rigidbody2D rb;
    private NavigationObject no;
    // Decision Tree. TODO: Add for Lab 7a.
    private DecisionTree dt;
    private int patrolIndex;
    [SerializeField] Transform testTarget;

    [SerializeField] float health;

    [Header("Torpedo Properties")]
    private bool readyToFire = true;
    [SerializeField] float torpedoCooldown;
    [SerializeField] float torpedoLifespan;
    [SerializeField] GameObject torpedoPrefab;
    [SerializeField] float combatRange;



    new void Start() // Note the new.
    {
        base.Start(); // Explicitly invoking Start of AgentObject.
        Debug.Log("Starting Ranged Combat Enemy.");
        rb = GetComponent<Rigidbody2D>();
        no = GetComponent<NavigationObject>();
        // TODO: Add for Lab 7a.
        dt = new DecisionTree(this.gameObject);
        BuildTree();
        patrolIndex = 0; //only on start, will resume patrol from current patrol point
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        Vector2 direction = (testTarget.position - transform.position).normalized;
        float angleInRadius = Mathf.Atan2(direction.y, direction.x);
        whiskerAngle = angleInRadius * Mathf.Rad2Deg;
        bool hit = CastWhisker(whiskerAngle, Color.red);

        // bool hit = CastWhisker(whiskerAngle, Color.red);
        // transform.Rotate(0f, 0f, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);

        //if (TargetPosition != null)
        //{
        //    // Seek();
        //    SeekForward();
        //    AvoidObstacles();
        //}

        dt.HealthNode.IsHealthy = health > 25f;

        // TODO: Add for Lab 7a. Add seek target for tree temporarily to planet.
        dt.RadiusNode.IsWithinRadius = Vector3.Distance(transform.position, testTarget.position) <= detectRange;
        dt.LOSNode.HasLOS = hit;

        dt.RangedCombatNode.IsWithinCombatRange = Vector3.Distance(transform.position, testTarget.position) <= combatRange;

        // TODO: Update for Lab 7a.
        dt.MakeDecision();

        switch(state)
        {
            case ActionState.PATROL:
                SeekForward();
                break;
            case ActionState.FLEE:
                Flee();
                break;
            case ActionState.MOVE_TO_RANGE:
                MoveToRange();
                break;
            case ActionState.MOVE_TO_LOS:
                MoveToLOS();
                break;
            case ActionState.ATTACK:
                Attack();
                break;
            default:
                rb.velocity = Vector3.zero; 
                break;
        }
    }

    void Flee()
    {

    }
    void MoveToRange()
    {
        SeekForward();
    }
    void MoveToLOS()
    {

    }
    void Attack()
    {
        if (readyToFire)
        {
            FireTorpedo();
        }
    }
    public void SetCombatTarget()
    {
        m_target = testTarget;
    }
    private void FireTorpedo()
    {
        readyToFire = false;
        Game.Instance.SOMA.PlaySound("Torpedo_k");
        Invoke("TorpedoReload", torpedoCooldown);
        GameObject torpedoInst = Instantiate(torpedoPrefab, transform.position, Quaternion.identity);
        torpedoInst.GetComponent<EnemyTorpedo>().LockOnTarget(testTarget);
        Destroy(torpedoInst, torpedoLifespan);
    }
    private void TorpedoReload()
    {
        readyToFire = true;
        Debug.Log("torpedo reloaded");
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    private bool CastWhisker(float angle, Color color)
    {
        bool hitResult = false;
        Color rayColor = color;

        // Calculate the direction of the whisker.
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * Vector2.right;

        if (no.HasLOS(gameObject, "Player", whiskerDirection, whiskerLength))
        {
            // Debug.Log("Obstacle detected!");
            rayColor = Color.green;
            hitResult = true;
        }

        // Debug ray visualization
        Debug.DrawRay(transform.position, whiskerDirection * whiskerLength, rayColor);
        return hitResult;
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
        if(Vector3.Distance(transform.position, TargetPosition)<= pointRadius)
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
        if(patrolIndex == patrolPoints.Length)
        {
            patrolIndex = 0;
        }

        return patrolPoints[patrolIndex];
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "Target")
    //    {
    //        GetComponent<AudioSource>().Play();
    //    }
    //}

    // TODO: Fill in for Lab 7a.
    private void BuildTree()
    {
        // Root condition node.
        dt.HealthNode = new HealthCondition();
        dt.treeNodeList.Add(dt.HealthNode);


        // Second level.


        // flee Action leaf.
        TreeNode fleeNode = dt.AddNode(dt.HealthNode, new FleeAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)fleeNode).SetAgent(this.gameObject, typeof(RangedCombatEnemy));
        dt.treeNodeList.Add(fleeNode);

        /*TreeNode patrolNode = dt.AddNode(dt.RadiusNode, new PatrolAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)patrolNode).Agent = this.gameObject;
        dt.treeNodeList.Add(patrolNode);*/

        // HitCondition node.
        dt.HitNode = new HitCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.HealthNode, dt.HitNode, TreeNodeType.RIGHT_TREE_NODE));
        /*dt.LOSNode = new LOSCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.RadiusNode, dt.LOSNode, TreeNodeType.RIGHT_TREE_NODE));*/

        // Third level.

        // Radius condition.
        dt.RadiusNode = new RadiusCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.HitNode, dt.RadiusNode, TreeNodeType.LEFT_TREE_NODE));

        //TODO: Other LOS to be done later
        //
        //

        // Fourth level.

        // PatrolAction leaf.
        TreeNode patrolNode = dt.AddNode(dt.RadiusNode, new PatrolAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)patrolNode).SetAgent(this.gameObject, typeof(RangedCombatEnemy));
        dt.treeNodeList.Add(patrolNode);

        // LOS condition Node 
        dt.LOSNode = new LOSCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.RadiusNode, dt.LOSNode, TreeNodeType.RIGHT_TREE_NODE));

        //TODO: wait behind cover node

        //TODO: move to cver node

        //Fifth level

        //move to los leaf
        TreeNode movetoLOSNode = dt.AddNode(dt.LOSNode, new MoveToLOSAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)movetoLOSNode).SetAgent(this.gameObject, typeof(RangedCombatEnemy));
        dt.treeNodeList.Add(movetoLOSNode);

        //RangedCOmbatCondition Node
        dt.RangedCombatNode = new RangedCombatCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.LOSNode, dt.RangedCombatNode, TreeNodeType.RIGHT_TREE_NODE));

        //Sixth Level

        //Move to Range action leaf
        TreeNode moveToRangedNode = dt.AddNode(dt.RangedCombatNode, new MoveToRangeAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)moveToRangedNode).SetAgent(this.gameObject, typeof(RangedCombatEnemy));
        dt.treeNodeList.Add(moveToRangedNode);

        //attack action leaf
        TreeNode attackNode = dt.AddNode(dt.RangedCombatNode, new AttackAction(), TreeNodeType.RIGHT_TREE_NODE);
        ((ActionNode)attackNode).SetAgent(this.gameObject, typeof(RangedCombatEnemy));
        dt.treeNodeList.Add(attackNode);
    }
}
