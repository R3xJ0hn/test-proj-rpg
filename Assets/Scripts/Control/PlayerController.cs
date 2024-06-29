using RPG.Combat;
using RPG.Movement;

namespace RPG.Controller
{
    using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        private float clicked = 0;
        private float clickTime = 0;
        private const float ClickDelay = 0.3f;
        private bool prepToAttack = false;

        private Fighter fighter;
        private Mover mover;
        private Camera mainCamera;
        RaycastHit? targetEnemy;


        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            MovePlayer();
            AttackEnemy();
        }
        private void MovePlayer()
        {
            if (Input.GetMouseButton(1))
            {
                if (Physics.Raycast(GetMouseRay(), out RaycastHit hit))
                {
                    CancelAttack();
                    mover.MoveTo(hit.point);
                }
            }
        }

        private void AttackEnemy()
        {
            if (PerformAttack())
            {
                RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
                foreach (RaycastHit hit in hits)
                    if (hit.collider.CompareTag("Enemy"))
                        targetEnemy = hit;
            }

            if (targetEnemy.HasValue)
            {
                fighter.Attack(targetEnemy.Value);
            }
        }

        private bool PerformAttack()
        {
            if (DoubleClick()) return true;

            if (Input.GetKey(KeyCode.A))
                prepToAttack = true;

            if (prepToAttack && Input.GetMouseButtonDown(0))
                return true;

            if (Input.GetKey(KeyCode.S))
            {
                CancelAttack();
                return false;
            }

            return false;
        }

        private void CancelAttack()
        {
            targetEnemy = null;
            prepToAttack = false;
            fighter.CancellAttack();
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