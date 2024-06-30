using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float weaponRange = 1.5f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;

        Transform target;
        float timeSinceLastAttack = 0f;



        public void Attack(RaycastHit target)
        {
            timeSinceLastAttack += Time.deltaTime;
            this.target = target.transform;

            bool isInRange = Vector3.Distance(transform.position, this.target.position) < weaponRange;

            if (this.target != null && !isInRange)
                GetComponent<Mover>().MoveTo(this.target.position);
            else
            {
                GetComponent<Mover>().Stop();
                AttackBehaviour();
            }

        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the Hit Event
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
            
        }

        //Animation Event
        void Hit()
        {
            if (target != null)
            {
                Health healthComponent = target.GetComponent<Health>();
                healthComponent.TakeDamage(weaponDamage);
            }
        }

        public void CancellAttack()
        {
            target = null;
            GetComponent<Mover>().Stop();
        }


    }

}
