using System;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]

    public class Mover : MonoBehaviour
    {
        NavMeshAgent agent;
        Animator animator;

        [SerializeField] float maxSpeed = 5.66f;
        [SerializeField] float stamina = 100.0f;
        [SerializeField] float staminaDrainRate = 1f;
        [SerializeField] float staminaRecoveryRate = 10f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;
            animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            agent.stoppingDistance = 0.5f;
            agent.angularSpeed = 360;
        }

        void Update()
        {
            UpdateStamina();
            UpdateSpeed();
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

        private void UpdateStamina()
        {
            if (agent.velocity.magnitude > 0)
                stamina -= staminaDrainRate * Time.deltaTime;
            else
                stamina += staminaRecoveryRate * Time.deltaTime;

            stamina = Mathf.Clamp(stamina, 0, 100);
        }

        private void UpdateSpeed()
        {
            agent.speed = maxSpeed * (stamina / 100);
        }

        private void UpdateAnimator()
        {
            animator.SetFloat("forwardSpeed", agent.velocity.magnitude);
        }
    }
}