using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        Transform PlayerTarget;

        void LateUpdate()
        {
            if (PlayerTarget == null)
                PlayerTarget = GameObject.FindGameObjectWithTag("Player").transform;

            transform.position = PlayerTarget.position;
        }
    }

}