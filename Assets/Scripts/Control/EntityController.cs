using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Controller
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Health))]

    public class EntityController : MonoBehaviour
    {
        private Mover mover;
        private Health health;
        private Fighter fighter;
        private GameObject _targetEntity;

        public bool AllowedToRun
        {
            get => mover.allowWalkingSpeedIncrease;
            set => mover.allowWalkingSpeedIncrease = value;
        }

        protected void InitializeComponents()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        public void MoveTo(Vector3 position)
        {
            CancelAttack();
            mover.MoveTo(position);
        }

        public void Sprint()
        {
            mover.Sprint();
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

        public bool IsDead() 
        {
            return health.IsDead;
        }

    }

}