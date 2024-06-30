using UnityEngine;

namespace RPG.Controller
{
    public class AIController : EntityController
    {
        [SerializeField] float chaseDistance = 5f;

        private GameObject player;
        Vector3 guardPosition;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            guardPosition = transform.position;
            InitializeComponents();
        }

        private void Update()
        {
            AttackPlayer();
        }

        public void AttackPlayer()
        {
            if (DisTanceToPlayer() < chaseDistance)
                Attack(player);
            else MoveTo(guardPosition);
        }

        private float DisTanceToPlayer()
        {
            return Vector3.Distance(player.transform.position, transform.position);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

    }


}