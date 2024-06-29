using System;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
    {
        NavMeshAgent MeshAgent;

        private void Start()
        {
            MeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            UpdateAnimator();
        }
        public void MoveTo(Vector3 destination)
        { 
            MeshAgent.isStopped = false;
            MeshAgent.destination = destination;
        }

        public void Stop()
        {
            MeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = MeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
        }
    }
}