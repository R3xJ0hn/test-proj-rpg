using RPG.Combat;
using RPG.Controls.ActionControls;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace RPG.Control.ActionControls
{
    public class AIActionController : MonoBehaviour, IEntityActionController
    {
        enum State { Idle, Patrol, Chase, Attack, Suspicion }

        State currentState = State.Idle;
        [SerializeField] float suspicionDuration = 7f;
        [SerializeField] float wanderRadius = 5f;
        [SerializeField] float lookAroundDuration = 2f;
        [SerializeField] float chaseDistance = 4.5f;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float wayPointDwellTime = 3f;
        [SerializeField] float viewDistance = 7f;
        [SerializeField] float viewAngle = 45f;

        public event Action<GameObject> AttackEvent;
        public event Action<Vector2> MoveByNormalizedVectorEvent;
        public event Action<Vector3> MoveToEvent;
        public event Action CancelAttackEvent;
        public event Action SprintEvent;
        public event Action StopMovingEvent;

        private GameObject player;
        private PatrolPath patrolPath;
        private Vector3 guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float suspicionTime = 0f;
        private bool isWandering = false;
        private bool isLookingAround = false;
        private int currentWayPointIndex = 0;
        private bool isDead;

        public PatrolPath PatrolPath { set => patrolPath = value; }
        public float ChaseDistance { set => chaseDistance = value; }
        public bool IsDead { set => isDead = value; }
        public bool AllowedToRun { get; set; }

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            guardPosition = transform.position;
        }

        private void LateUpdate()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;

            if (!isDead)
                AIStateBehavior();
        }
        private void AIStateBehavior()
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
                PatrolBehavior();
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
                OnMoveTo(GetCurrentWayPoint());
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
                AllowedToRun = true;
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
                OnMoveTo(player.transform.position);
            }
        }

        private void AttackBehavior()
        {
            if (IsPlayerWithinView())
            {
                SprintEvent?.Invoke();
                AttackEvent?.Invoke(player);
            }
            else
            {
                currentState = State.Suspicion;
            }
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
                OnMoveTo(guardPosition);
            }
        }

        private IEnumerator DwellingBehavior()
        {
            isWandering = true;

            Vector3 wanderDestination = guardPosition +
                new Vector3(UnityEngine.Random.Range(-wanderRadius, wanderRadius), 0,
                UnityEngine.Random.Range(-wanderRadius, wanderRadius));

            OnMoveTo(wanderDestination);

            StartCoroutine(LookAroundBehavior());

            while (Vector3.Distance(transform.position, wanderDestination) > 1f)
            {
                yield return null;
            }

            isWandering = false;
        }


        private void OnMoveTo(Vector3 targetPos)
        {
            MoveToEvent?.Invoke(targetPos);
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
