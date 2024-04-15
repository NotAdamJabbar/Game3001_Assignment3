using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CloseCombatEnemy : AgentObject
{
    // TODO: Add for Lab 7a.
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float pointRadius;

    [SerializeField] float movementSpeed; // TODO: Uncomment for Lab 7a.
    [SerializeField] float rotationSpeed;
    [SerializeField] float whiskerLength;
    [SerializeField] float whiskerAngle;

    [SerializeField] TMP_Text stateText;

    [SerializeField] float detectRange;
    [SerializeField] float attackRange;
    // [SerializeField] float avoidanceWeight;
    private Rigidbody2D rb;
    private NavigationObject no;
    // Decision Tree. TODO: Add for Lab 7a.
    private DecisionTree dt;
    private int patrolIndex;
    private Transform player;
    private bool attacking = false;
    private bool attackSoundPlayedRecently = false;
    [SerializeField] Transform testTarget; //Planet to seek

    new void Start() // Note the new.
    {
        base.Start(); // Explicitly invoking Start of AgentObject.
        Debug.Log("Starting Close Combat Enemy.");
        rb = GetComponent<Rigidbody2D>();
        no = GetComponent<NavigationObject>();
        // TODO: Add for Lab 7a.
        dt = new DecisionTree(this.gameObject);
        BuildTree();
        patrolIndex = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        stateText.transform.position = new Vector3(transform.position.x, transform.position.y-2f);

        Debug.Log("ATTACKING = " + attacking);
/*        Vector2 direction = (testTarget.position - transform.position).normalized;
        float angleInRadius = Mathf.Atan2(direction.y, direction.x);
        whiskerAngle = angleInRadius * Mathf.Rad2Deg;*/
        bool hit = CastWhisker(whiskerAngle , Color.red) || CastWhisker(-whiskerAngle, Color.blue) || CastWhisker(0,Color.magenta) || CastWhisker(whiskerAngle/2,Color.black)||CastWhisker(whiskerAngle/2,Color.cyan);

        // bool hit = CastWhisker(whiskerAngle, Color.red);
        // transform.Rotate(0f, 0f, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);

        //if (TargetPosition != null)
        //{
        //    // Seek();
        //    SeekForward();
        //    AvoidObstacles();
        //}

        // TODO: Add for Lab 7a. Add seek target for tree temporarily to planet.
        dt.RadiusNode.IsWithinRadius = hit;

        //dt.LOSNode.HasLOS = hit;

        //dt.CloseCombatNode.IsWithinCombatRange =hit;

        dt.MakeDecision();

        switch (state)
        {
            case ActionState.PATROL:
                stateText.text = "Patrolling";
                SeekForward();
                break;
            case ActionState.MOVE_TO_PLAYER:
                stateText.text = "Attacking";
                MoveToPlayer();
                break;
            case ActionState.MOVE_TO_RANGE:
                MoveToPlayer();
                break;
            case ActionState.MOVE_TO_LOS:
                MoveToPlayer();
                break;
            case ActionState.ATTACK:
                stateText.text = "Attacking";
                MoveToPlayer();
                break;
            default:
                rb.velocity = Vector3.zero;
                break;
        }
    }

    public void DoAttack(bool what)
    {
        attacking = what;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (attacking)
            {
                Game.Instance.SOMA.PlaySound("Die");
                SceneManager.LoadScene("LoseScene");
            }

            else
            {
                Game.Instance.SOMA.PlaySound("Hit");
                SceneManager.LoadScene("WinScene");
            }
        }
    }
    //private void AvoidObstacles()
    //{
    //    // Cast whiskers to detect obstacles
    //    bool hitLeft = CastWhisker(whiskerAngle, Color.red);
    //    bool hitRight = CastWhisker(-whiskerAngle, Color.blue);

    //    // Adjust rotation based on detected obstacles
    //    if (hitLeft)
    //    {
    //        // Rotate counterclockwise if the left whisker hit
    //        RotateClockwise();
    //    }
    //    else if (hitRight && !hitLeft)
    //    {
    //        // Rotate clockwise if the right whisker hit
    //        RotateCounterClockwise();
    //    }
    //}

    //private void RotateCounterClockwise()
    //{
    //    // Rotate counterclockwise based on rotationSpeed and a weight.
    //    transform.Rotate(Vector3.forward, rotationSpeed * avoidanceWeight * Time.deltaTime);
    //}

    //private void RotateClockwise()
    //{
    //    // Rotate clockwise based on rotationSpeed.
    //    transform.Rotate(Vector3.forward, -rotationSpeed * avoidanceWeight * Time.deltaTime);
    //}

    private bool CastWhisker(float angle, Color color)
    {
        bool hitResult = false;
        Color rayColor = color;

        // Calculate the direction of the whisker.
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * transform.up;

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
        if (Vector3.Distance(transform.position, TargetPosition) <= pointRadius)
        {
            m_target = GetNextPatrolPoint();
        }
        attacking = false;
    }
    public void StartMoveToPlayer()
    {     
        m_target = player;
        StartCoroutine(PlayAttackSound());
    }
    private IEnumerator PlayAttackSound()
    {
        if (!attackSoundPlayedRecently)
            Game.Instance.SOMA.PlaySound("Attacking");
        attackSoundPlayedRecently = true;
        yield return new WaitForSeconds(1);
        attackSoundPlayedRecently = false;
    }
    private IEnumerator PlayNoAttackSound()
    {
        if (!attackSoundPlayedRecently)
            Game.Instance.SOMA.PlaySound("Patrolling");
        attackSoundPlayedRecently = true;
        yield return new WaitForSeconds(1);
        attackSoundPlayedRecently = false;
    }
    private void MoveToPlayer()
    {
        Vector2 directionToTarget = (player.position - transform.position).normalized;

        // Calculate the angle to rotate towards the target.
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + 90.0f; // Note the +90 when converting from Radians.

        // Smoothly rotate towards the target.
        float angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        float rotationStep = rotationSpeed * Time.deltaTime;
        float rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        transform.Rotate(Vector3.forward, rotationAmount);

        // Move along the forward vector using Rigidbody2D.
        rb.velocity = transform.up * movementSpeed;
        attacking = true;
    }

    // TODO: Add for Lab 7a.
    public void StartPatrol()
    {
        m_target = patrolPoints[patrolIndex];
        StartCoroutine(PlayNoAttackSound());
        DoAttack(false);
    }

    // TODO: Add for Lab 7a.
    private Transform GetNextPatrolPoint()
    {
        patrolIndex++;
        if (patrolIndex >= patrolPoints.Length)
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
        dt.RadiusNode = new RadiusCondition();
        dt.treeNodeList.Add(dt.RadiusNode);

        // Second level.

        // PatrolAction leaf.
        TreeNode patrolNode = dt.AddNode(dt.RadiusNode, new PatrolAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)patrolNode).SetAgent(gameObject, typeof(CloseCombatEnemy));
        dt.treeNodeList.Add(patrolNode);

        // LOSCondition node.
        dt.LOSNode = new LOSCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.RadiusNode, dt.LOSNode, TreeNodeType.RIGHT_TREE_NODE));

        // Third level.

        // MoveToLOSAction leaf.
        TreeNode MoveToLOSNode = dt.AddNode(dt.LOSNode, new MoveToLOSAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)MoveToLOSNode).SetAgent(gameObject, typeof(CloseCombatEnemy));
        dt.treeNodeList.Add(MoveToLOSNode);

        // CloseCombatCondition node.
        dt.CloseCombatNode = new CloseCombatCondition();
        dt.treeNodeList.Add(dt.AddNode(dt.LOSNode, dt.CloseCombatNode, TreeNodeType.RIGHT_TREE_NODE));

        // Fourth level.

        // MoveToPlayerAction leaf.
        TreeNode MoveToPlayerNode = dt.AddNode(dt.CloseCombatNode, new MoveToPlayerAction(), TreeNodeType.LEFT_TREE_NODE);
        ((ActionNode)MoveToPlayerNode).SetAgent(gameObject, typeof(CloseCombatEnemy));
        dt.treeNodeList.Add(MoveToPlayerNode);

        // AttackAction leaf.
        TreeNode AttackNode = dt.AddNode(dt.CloseCombatNode, new AttackAction(), TreeNodeType.RIGHT_TREE_NODE);
        ((ActionNode)AttackNode).SetAgent(gameObject, typeof(CloseCombatEnemy));
        dt.treeNodeList.Add(AttackNode);
    }
}
