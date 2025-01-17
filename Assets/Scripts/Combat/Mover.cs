using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Health))]
    public class Mover : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;
        private Health health;

        [SerializeField] private float maxSpeed = 5.66f;
        [SerializeField] private float minSpeed = 1.5f;
        [SerializeField] private float stamina = 100.0f;
        [SerializeField] private float staminaDrainRate = 10f;
        [SerializeField] private float staminaRecoveryRate = 5f;
        [SerializeField] private float runningThreshold = 3f;
        [SerializeField] private float sprintDuration = 3.0f;

        private readonly float speedIncreaseRate = 0.5f;
        private bool isSprinting = false;

        public bool WalkingSpeedIncrease { get; set; } = true;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();

            animator.applyRootMotion = true;
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            agent.stoppingDistance = 0.5f;
            agent.angularSpeed = 360;
            agent.speed = minSpeed;
        }

        private void Update()
        {
            if (health.IsDead)
            {
                agent.enabled = false;
                return;
            }

            UpdateSpeed();
            UpdateStamina();
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination)
        {
            if (health.IsDead) return;
            agent.isStopped = false;
            agent.destination = destination;
        }

        public void StopMoving()
        {
            isSprinting = false;
            if (agent.enabled)
                agent.isStopped = true;
        }

        public void Sprint()
        {
            if (stamina >= 75)
            {
                isSprinting = true;
                StartCoroutine(SprintCoroutine());
            }
        }

        private void UpdateSpeed()
        {
            if (IsMoving() && WalkingSpeedIncrease)
            {
                if (!isSprinting)
                    agent.speed = Mathf.Min(agent.speed + 
                        speedIncreaseRate * Time.deltaTime, runningThreshold);
                else
                    agent.speed = runningThreshold + 
                        ((maxSpeed - runningThreshold) * (stamina / 100));
            }
            else
            {
                isSprinting = false;
                agent.speed = minSpeed;
            }
        }

        private IEnumerator SprintCoroutine()
        {
            agent.speed = maxSpeed;
            yield return new WaitForSeconds(sprintDuration);
            isSprinting = false;
        }

        private void UpdateStamina()
        {
            if (isSprinting)
            {
                if (agent.speed > runningThreshold)
                    stamina -= staminaDrainRate * Time.deltaTime;
            }
            else
            {
                if (IsMoving())
                    stamina += (staminaRecoveryRate * 0.5f) * Time.deltaTime;
                else
                    stamina += staminaRecoveryRate * Time.deltaTime;
            }

            stamina = Mathf.Clamp(stamina, 0, 100);
        }

        private bool IsMoving()
        {
            return agent.velocity.magnitude > 0;
        }

        private void UpdateAnimator()
        {
            animator.SetFloat("forwardSpeed", agent.velocity.magnitude);
        }

    }
}
