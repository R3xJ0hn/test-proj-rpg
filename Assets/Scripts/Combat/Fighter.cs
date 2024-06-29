using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float weaponRange = 1.5f;

        Transform targetLocation;

        public void Attack(RaycastHit target)
        {
            targetLocation = target.transform;
            bool isInRange = Vector3.Distance(transform.position, targetLocation.position) < weaponRange;

            if (targetLocation != null && !isInRange)
                GetComponent<Mover>().MoveTo(targetLocation.position);
                
            else
                GetComponent<Mover>().Stop();

        }

        public void CancellAttack()
        {
            targetLocation = null;
            GetComponent<Mover>().Stop();
        }
    }

}
