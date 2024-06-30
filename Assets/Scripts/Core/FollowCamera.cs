using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform PlayerTarget;

        void LateUpdate()
        {
            transform.position = PlayerTarget.position;
        }
    }

}