using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Mover))]

    public class Fighter : MonoBehaviour
    {
        [SerializeField] float weaponRange = 1.5f;
        [SerializeField] float timeBetweenAttacks = 0.5f;
        [SerializeField] float weaponDamage = 5f;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        public void Attack(GameObject targetSelected)
        {
            target = targetSelected.GetComponent<Health>();

            if (!CanAttack()) return;

            timeSinceLastAttack += Time.deltaTime;
            bool isInRange = Vector3.Distance(transform.position,
                target.transform.position) < weaponRange;

            if (this.target != null && !isInRange)
            {
                // Move closer to the player
                GetComponent<Mover>().MoveTo(this.target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Stop();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                GetComponent<Animator>().ResetTrigger("stopAttack");
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }

        //Animation Event
        //triggered by Attack Animation
        void Hit()
        {
            if (target == null) return;

            Health healthComponent = target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }

        public bool CanAttack()
        {
            bool meIsDead = GetComponent<Health>().IsDead;
            return target != null && !target.IsDead && !meIsDead;
        }

        public void CancellAttack()
        {
            target = null;
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

    }

}
