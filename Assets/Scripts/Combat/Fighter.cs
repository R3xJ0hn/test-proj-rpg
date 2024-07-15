using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Mover))]

    public class Fighter : MonoBehaviour
    {
        [SerializeField] private float timeBetweenAttacks = 0.5f;
        [SerializeField] private float weaponDamage = 5f;

        private Animator animator;
        private Mover mover;
        private Health targetHealth;
        private Transform targetTransform;
        private float timeSinceLastAttack = Mathf.Infinity;

        public float AttackRange { get; set; } = 4f;
        public float WeaponRange { get; set; } = 1.5f;
        public float WeaponDamage { get; set; } = 1.5f;


        private void Awake()
        {
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
        }

        public void Attack(GameObject targetSelected)
        {
            targetHealth = targetSelected.GetComponent<Health>();
            targetTransform = targetSelected.transform;

            if (!CanAttack()) return;

            timeSinceLastAttack += Time.deltaTime;
            bool isInRange = Vector3.Distance(transform.position,
                targetHealth.transform.position) < WeaponRange;

            if (targetHealth != null && !isInRange)
            {
                // Move closer to the target
                mover.MoveTo(targetTransform.position);
            }
            else
            {
                // When in range
                mover.StopMoving();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(targetTransform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                animator.ResetTrigger("stopAttack");
                animator.SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }

        // Animation Event
        // triggered by Attack Animation
        private void Hit()
        {
            if (targetHealth == null) return;
            targetHealth.TakeDamage(weaponDamage);
        }

        private bool CanAttack()
        {
            bool meIsDead = GetComponent<Health>().IsDead;
            return targetHealth != null && !targetHealth.IsDead && !meIsDead;
        }

        public void CancellAttack()
        {
            targetHealth = null;
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

    }

}
