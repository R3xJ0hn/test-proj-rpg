using RPG.Controls.ActionControls;
using System;
using UnityEngine;

namespace RPG.Control.ActionControls
{
    public class MouseKeyboardActionController : MonoBehaviour, IEntityActionController
    {
        private float clicked = 0;
        private float clickTime = 0;
        private const float ClickDelay = 0.3f;
        private bool prepToAttack = false;

        private Camera mainCamera;
        private GameObject targetEnemy;
        private float attackRange;
        private bool isDead;


        public event Action<GameObject> AttackEvent;
        public event Action<Vector2> MoveByNormalizedVectorEvent;
        public event Action<Vector3> MoveToEvent;
        public event Action CancelAttackEvent;
        public event Action SprintEvent;
        public event Action StopMovingEvent;

        public float ChaseDistance { set => attackRange = value; }
        public bool IsDead { set => isDead = value; }
        public bool AllowedToRun { get; set; }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            MovePlayer();
            SetSprint();
            AttackEnemy();
        }

        private void MovePlayer()
        {
            if (Input.GetMouseButton(1))
            {
                if (Physics.Raycast(GetMouseRay(), out RaycastHit hit))
                {
                    MoveToEvent?.Invoke(hit.point);
                    CancelAttack();
                }
            }
        }

        private void SetSprint()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                SprintEvent?.Invoke();
            }

        }

        public void AttackEnemy()
        {
            if (SetAnAttack())
            {
                RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        targetEnemy = hit.collider.gameObject;
                        break;
                    }
                }
            }

            if (targetEnemy != null)
                AttackEvent?.Invoke(targetEnemy);
        }


        private bool SetAnAttack()
        {
            if (DoubleClick()) return true;

            if (Input.GetKey(KeyCode.A))
                prepToAttack = true;

            if (prepToAttack && Input.GetMouseButton(0))
                return true;

            if (Input.GetKey(KeyCode.S))
            {
                CancelAttack();
                return false;
            }

            return false;
        }

        public void CancelAttack()
        {
            prepToAttack = false;
            targetEnemy = null;
            CancelAttackEvent?.Invoke();
        }

        private Ray GetMouseRay()
        {
            return mainCamera.ScreenPointToRay(Input.mousePosition);
        }

        private bool DoubleClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                clicked++;
                if (clicked == 1)
                {
                    clickTime = Time.time;
                }
            }

            if (clicked > 1 && Time.time - clickTime < ClickDelay)
            {
                clicked = 0;
                clickTime = 0;
                return true;
            }
            else if (clicked > 2 || Time.time - clickTime > 1)
            {
                clicked = 0;
            }

            return false;
        }


    }
}
