using RPG.Combat;
using RPG.Control.ActionControls;
using RPG.Controls.ActionControls;
using System;
using UnityEngine;

namespace RPG.Controls
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Health))]

    public class EntityController : MonoBehaviour
    {
        public enum ControllerType
        {
            Touch,
            MouseKeyboard,
            AI
        }

        private Mover mover;
        private Health health;
        private Fighter fighter;
        private GameObject _targetEntity;
        private IEntityActionController actionController;

        [SerializeField]
        private ControllerType controllerType;

        [SerializeField, HideInInspector]
        private PatrolPath patrolPath;

        public ControllerType SelectedControllerType
        {
            get => controllerType;
            set => controllerType = value;
        }

        public PatrolPath PatrolPath
        {
            get => patrolPath;
            set => patrolPath = value;
        }

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();

            switch (controllerType)
            {
                case ControllerType.Touch:
                    //actionController = gameObject.AddComponent<TouchActionController>();
                    break;
                case ControllerType.MouseKeyboard:
                    actionController = gameObject.AddComponent<MouseKeyboardActionController>();
                    break;
                case ControllerType.AI:
                    actionController = gameObject.AddComponent<AIActionController>();
                    ((AIActionController)actionController).PatrolPath = patrolPath;
                    break;
            }

            if (actionController != null)
            {
                actionController.ChaseDistance = fighter.AttackRange;
                actionController.AttackEvent += OnAttack;
                actionController.MoveByNormalizedVectorEvent += OnMoveByNormalizedVector;
                actionController.MoveToEvent += OnMoveTo;
                actionController.CancelAttackEvent += OnCancelAttack;
                actionController.SprintEvent += OnSprint;
                actionController.StopMovingEvent += OnStopMoving;
            }
        }

        private void LateUpdate()
        {
            mover.WalkingSpeedIncrease = actionController.AllowedToRun;
        }

        public virtual void OnAttack(GameObject targetEntity)
        {
            _targetEntity = targetEntity;

            if (_targetEntity != null)
            {
                fighter.Attack(_targetEntity);
            }
        }

        private void OnMoveByNormalizedVector(Vector2 vector)
        {
            throw new NotImplementedException();
        }

        public void OnMoveTo(Vector3 position)
        {
            OnCancelAttack();
            mover.MoveTo(position);
        }

        public void OnSprint()
        {
            mover.Sprint();
        }

        public virtual void OnCancelAttack()
        {
            _targetEntity = null;
            fighter.CancellAttack();
        }

        public void OnStopMoving()
        {
            mover.StopMoving();
        }

        public bool IsDead()
        {
            return health.IsDead;
        }

    }

}