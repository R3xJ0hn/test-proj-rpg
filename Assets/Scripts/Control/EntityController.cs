using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Controller
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Mover))]
    public class EntityController : MonoBehaviour
    {
        private Fighter fighter;
        private Mover mover;
        private GameObject _targetEntity;

        protected void InitializeComponents()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
        }

        public void MoveTo(Vector3 position)
        {
            CancelAttack();
            mover.MoveTo(position);
        }

        public void InAttackRange()
        {

        }

        public virtual void Attack(GameObject targetEntity)
        {
            _targetEntity = targetEntity;

            if (_targetEntity != null)
            {
                fighter.Attack(_targetEntity);
            }
        }

        public virtual void CancelAttack()
        {
            _targetEntity = null;
            fighter.CancellAttack();
        }

    }

}