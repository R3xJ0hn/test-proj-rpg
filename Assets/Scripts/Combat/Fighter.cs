using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float weaponRange = 1.5f;
        [SerializeField] float timeBetweenAttacks = 4f;
        [SerializeField] float weaponDamage = 5f;

        Health target;
        float timeSinceLastAttack = 0f;

        public void Attack(RaycastHit targetHit)
        {
            target = targetHit.collider.GetComponent<Health>();
            if (!CanAttack()) return;

            timeSinceLastAttack += Time.deltaTime;
            bool isInRange = Vector3.Distance(transform.position,
                target.transform.position) < weaponRange;

            if (this.target != null && !isInRange)
                GetComponent<Mover>().MoveTo(this.target.transform.position);
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

        //Animation Event triggered by Attack Animation
        void Hit()
        {
            if (target == null) return;

            Health healthComponent = target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }

        public bool CanAttack()
        {
            return target != null && !target.IsDead;
        }

        public void CancellAttack()
        {
            target = null;
            GetComponent<Animator>().SetTrigger("stopAttack");
            GetComponent<Mover>().Stop();
        }

    }

}
