using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Player;
    public EnemyLineOfSight LineOfSight;
    public NavMeshTriangulation Triangulation = new NavMeshTriangulation();
    public float UpdateRate = 0.1f;
    private NavMeshAgent Agent;

    [SerializeField]
    private Animator Animator = null;

    public EnemyState DefaultState;
    private EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;
    public float IdleLocationRadius = 4f;
    public float IdleMovespeedMultiplier = 0.5f;
    public Vector3[] Waypoints = new Vector3[4];
    [SerializeField]
    private int WaypointIndex = 0;

    private const string IsWalking = "IsWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private Coroutine FollowCoroutine;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();


        LineOfSight.OnGainSight += HandleGainSight;
        LineOfSight.OnLoseSight += HandleLoseSight;

        OnStateChange += HandleStateChange;
    }

    private void HandleGainSight(Player player)
    {   Debug.Log("Chasing");
        State = EnemyState.Chase;
    }

    private void HandleLoseSight(Player player)
    {
        State = DefaultState;
    }

    private void OnDisable()
    {
        _state = DefaultState; // use _state to avoid triggering OnStateChange when recycling object in the pool
    }

    public void Spawn()
    {
        if (Triangulation.vertices != null)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                NavMeshHit Hit;
                if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out Hit, 2f, Agent.areaMask))
                {
                    Waypoints[i] = Hit.position;
                }
                else
                {
                    Debug.LogError("Unable to find position for navmesh near Triangulation vertex!");
                }
            }
        }
  
        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
    }


    private void Update()
    {
        Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
    }

    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            if (FollowCoroutine != null)
            {
                StopCoroutine(FollowCoroutine);
            }

            if (oldState == EnemyState.Idle)
            {
                Agent.speed /= IdleMovespeedMultiplier;
            }

            switch (newState)
            {
                case EnemyState.Idle:
                    FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;
                case EnemyState.Patrol:
                    FollowCoroutine = StartCoroutine(DoPatrolMotion());
                    break;
                case EnemyState.Chase:
                    Debug.Log("Following");
                    FollowCoroutine = StartCoroutine(FollowTarget());
                    break;
            }
        }
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        Agent.speed *= IdleMovespeedMultiplier;

        while (true)
        {
            if (!Agent.enabled || !Agent.isOnNavMesh)
            {
                yield return Wait;
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
                {
                    Agent.SetDestination(hit.position);
                }
            }

            yield return Wait;
        }
    }

    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        yield return new WaitUntil(() => Agent.enabled && Agent.isOnNavMesh);
        Agent.SetDestination(Waypoints[WaypointIndex]);

        while (true)
        {
            if (Agent.isOnNavMesh && Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                WaypointIndex++;

                if (WaypointIndex >= Waypoints.Length)
                {
                    WaypointIndex = 0;
                }

                Agent.SetDestination(Waypoints[WaypointIndex]);
            }

            yield return Wait;
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        while (true)
        {
            if (Agent.enabled)
            {
                Agent.SetDestination(Player.transform.position);
            }
            yield return Wait;
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < Waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(Waypoints[i], 0.25f);
            if (i + 1 < Waypoints.Length)
            {
                Gizmos.DrawLine(Waypoints[i], Waypoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(Waypoints[i], Waypoints[0]); 
            }
        }
    }
}