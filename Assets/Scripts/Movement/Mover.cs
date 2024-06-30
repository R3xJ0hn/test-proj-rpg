using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [SerializeField]

    public class Mover : MonoBehaviour
    {
        NavMeshAgent agent;
        Animator animator;

        [SerializeField] float maxSpeed = 5.66f;
        [SerializeField] float minSpeed = 3.00f;
        [SerializeField] float stamina = 100.0f;
        [SerializeField] float staminaDrainRate = 1f;
        [SerializeField] float staminaRecoveryRate = 10f;
        [SerializeField] float speedIncreaseRate = 0.5f;
        [SerializeField] float staminaDrainSpeedThreshold = 3.0f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            agent.stoppingDistance = 0.5f;
            agent.angularSpeed = 360;
            agent.speed = minSpeed;
        }

        void Update()
        {
            UpdateSpeed();
            UpdateStamina();
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination)
        {
            agent.isStopped = false;
            agent.destination = destination;
        }

        public void Stop()
        {
            agent.isStopped = true;
        }

        private void UpdateSpeed()
        {
            if (agent.velocity.magnitude > 0)
            {
                agent.speed = Mathf.Min(agent.speed + speedIncreaseRate * Time.deltaTime, maxSpeed);
            }
            else
            {
                agent.speed = minSpeed;
            }
        }

        private void UpdateStamina()
        {
            if (agent.velocity.magnitude > 0)
            {
                if (agent.speed > staminaDrainSpeedThreshold)
                    stamina -= staminaDrainRate * Time.deltaTime;
            }
            else
            {
                stamina += staminaRecoveryRate * Time.deltaTime;
            }

            stamina = Mathf.Clamp(stamina, 0, 100);
        }

        private void UpdateAnimator()
        {
            animator.SetFloat("forwardSpeed", agent.velocity.magnitude);
        }
    }
}