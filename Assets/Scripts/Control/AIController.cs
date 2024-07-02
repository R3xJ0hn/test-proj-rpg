using RPG.Movement;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace RPG.Controller
{
    public class AIController : EntityController
    {
        enum State { Idle, Patrol, Chase, Attack, Suspicion }

        State currentState = State.Idle;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float suspicionDuration = 7f;
        [SerializeField] float wanderRadius = 5f;
        [SerializeField] float lookAroundDuration = 2f;
        [SerializeField] float chaseDistance = 4.5f;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float wayPointDwellTime = 3f;
        [SerializeField] float viewDistance = 10f;
        [SerializeField] float viewAngle = 45f;

        private GameObject player;
        private Vector3 guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float suspicionTime = 0f;
        private bool isWandering = false;
        private bool isLookingAround = false;
        private int currentWayPointIndex = 0;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            guardPosition = transform.position;
            InitializeComponents();
        }

        private void Update()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            AllowedToRun = true;

            if (!IsDead())
                AIStateBehavior();
        }

        void AIStateBehavior()
        {
            switch (currentState)
            {
                case State.Idle:
                    IdleBehavior();
                    break;

                case State.Patrol:
                    PatrolBehavior();
                    break;

                case State.Chase:
                    ChaseBehavior();
                    break;

                case State.Attack:
                    AttackBehavior();
                    break;

                case State.Suspicion:
                    SuspicionBehavior();
                    break;
            }
        }

        private void IdleBehavior()
        {
            if (IsPlayerWithinView())
            {
                currentState = State.Chase;
            }
            else if (patrolPath != null)
            {
                currentState = State.Patrol;
            }
        }

        private void PatrolBehavior()
        {
            AllowedToRun = false;

            if (ArrivedAtWayPoint())
            {
                timeSinceArrivedAtWaypoint = 0;
                currentWayPointIndex = patrolPath.GetNextIndex(currentWayPointIndex);
                StartCoroutine(LookAroundBehavior());
            }

            if (timeSinceArrivedAtWaypoint > wayPointDwellTime)
            {
                MoveTo(GetCurrentWayPoint());
            }

            if (IsPlayerWithinView())
            {
                currentState = State.Chase;
            }
        }

        private void ChaseBehavior()
        {
            if (IsPlayerWithinView())
            {
                timeSinceLastSawPlayer = 0;
                currentState = State.Attack;
            }
            else if (timeSinceLastSawPlayer > suspicionDuration)
            {
                suspicionTime = 0f;
                currentState = State.Suspicion;
            }
            else
            {
                MoveTo(player.transform.position);
            }
        }

        private void AttackBehavior()
        {
            if (IsPlayerWithinView())
            {
                Sprint();
                Attack(player);
            }
            else
            {
                currentState = State.Suspicion;
            }
        }

        private void SuspicionBehavior()
        {
            suspicionTime += Time.deltaTime;
            if (!isWandering && !isLookingAround)
            {
                StartCoroutine(DwellingBehavior());
            }

            if (IsPlayerWithinView())
            {
                StopAllCoroutines();
                isWandering = false;
                isLookingAround = false;
                currentState = State.Chase;
            }
            else if (suspicionTime > suspicionDuration + lookAroundDuration)
            {
                currentState = patrolPath != null ? State.Patrol : State.Idle;
                MoveTo(guardPosition);
            }
        }

        private IEnumerator DwellingBehavior()
        {
            isWandering = true;

            Vector3 wanderDestination = guardPosition + 
                new Vector3(Random.Range(-wanderRadius, wanderRadius), 0, 
                Random.Range(-wanderRadius, wanderRadius));

            MoveTo(wanderDestination);

            StartCoroutine(LookAroundBehavior()); 

            while (Vector3.Distance(transform.position, wanderDestination) > 1f)
            {
                yield return null;
            }

            isWandering = false;
        }

        private IEnumerator LookAroundBehavior()
        {
            isLookingAround = true;
            float lookAroundEndTime = Time.time + lookAroundDuration;
            while (Time.time < lookAroundEndTime)
            {
                transform.Rotate(Vector3.up, 45 * Time.deltaTime);
                yield return null;
            }
            isLookingAround = false;
        }

        private bool IsPlayerWithinView()
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            return (angleToPlayer < viewAngle / 2 && distanceToPlayer < viewDistance) || distanceToPlayer < chaseDistance;
        }

        private bool ArrivedAtWayPoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWayPoint()) < wayPointTolerance;
        }

        private Vector3 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(currentWayPointIndex);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 forward = transform.forward * viewDistance;
            Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
            Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

            Gizmos.DrawLine(transform.position, transform.position + forward);
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

            Handles.color = isWandering ? Color.blue : Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, chaseDistance);
        }
    }
}
