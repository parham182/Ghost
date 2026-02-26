using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyMove : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] float seeSpeed = 10;
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;
    
    float patrolTimer;
    
    public AudioClip scarysound;
    [SerializeField] AudioSource seesound;
    public NavMeshAgent agent;
    public Transform player;
    
    [Header("FOV Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    bool playerVisible;

    public float chaseAfterLoseTime = 2f;

    float loseTimer;
    bool isChasing;
    
    [Header("Animation Settings")]
    [SerializeField] Animator animator;
    [SerializeField] string idleParam = "Idle";
    [SerializeField] string walkParam = "Walk";
    [SerializeField] string runParam = "Run";
    [SerializeField] float moveThreshold = 0.05f;

    float defSpeed;
    void Start()
    {
        defSpeed = agent.speed;
    }

    void Update()
    {
        CheckFOV();

        if (playerVisible)
        {
            loseTimer = 0f;
            isChasing = true;
            agent.SetDestination(player.position);
            agent.speed = seeSpeed;
        }
        else if (isChasing)
        {
            loseTimer += Time.deltaTime;

            if (loseTimer < chaseAfterLoseTime)
            {
                agent.SetDestination(player.position);
            }
            else 
            {
                isChasing = false;
                agent.speed = defSpeed;
                agent.ResetPath();

                if(seesound != null) seesound.Stop();
            }
        }
        else
        {
            Patrol();
        }

        UpdateAnimationState();
    }


    void CheckFOV()
    {
        playerVisible = false;

        Collider[] targets =
            Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        if (targets.Length == 0) return;

        Transform target = targets[0].transform;
        Vector3 dirToTarget = (target.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
        {
            float dist = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, dirToTarget, dist, obstacleMask))
            {
                if (!playerVisible && scarysound != null)
                {
                    seesound.PlayOneShot(scarysound);
                }

                playerVisible = true;
            }
        }
    }
    Vector3 GetRandomPointOnNavMesh(Vector3 center, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return center;
    }

    void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (agent.hasPath) return;

        if (patrolTimer >= patrolWaitTime)
        {
            Vector3 randomPoint = GetRandomPointOnNavMesh(transform.position, patrolRadius);
            agent.SetDestination(randomPoint);
            patrolTimer = 0f;
        }
    }

    void UpdateAnimationState()
    {
        if (animator == null) return;

        bool isMoving = agent.velocity.sqrMagnitude > moveThreshold * moveThreshold;
        bool isRunning = isChasing && isMoving;
        bool isWalking = !isChasing && isMoving;
        bool isIdle = !isMoving;

        animator.SetBool(idleParam, isIdle);
        animator.SetBool(walkParam, isWalking);
        animator.SetBool(runParam, isRunning);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 left =
            Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 right =
            Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRadius);
    }

}
