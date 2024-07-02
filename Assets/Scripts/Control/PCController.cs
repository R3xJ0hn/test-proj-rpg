using UnityEngine;

namespace RPG.Controller
{
    public class PCController : EntityController
    {
        private float clicked = 0;
        private float clickTime = 0;
        private const float ClickDelay = 0.3f;
        private bool prepToAttack = false;

        private Camera mainCamera;
        private GameObject targetEnemy;

        private void Awake()
        {
            mainCamera = Camera.main;
            InitializeComponents();
        }
        private void LateUpdate()
        {
            MovePlayer();
            AttackEnemy();
            SetSprint();
        }

        private void MovePlayer()
        {
            if (Input.GetMouseButton(1))
            {
                if (Physics.Raycast(GetMouseRay(), out RaycastHit hit))
                {
                    MoveTo(hit.point);
                }
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

            Attack(targetEnemy);
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

        private void SetSprint()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Sprint();
            }
           
        }

        public override void CancelAttack()
        {
            prepToAttack = false;
            targetEnemy = null;
            base.CancelAttack();
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