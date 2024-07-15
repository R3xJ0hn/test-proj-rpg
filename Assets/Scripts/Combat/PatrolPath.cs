using UnityEngine;

namespace RPG.Combat
{
    public class PatrolPath : MonoBehaviour
    {
        private const float waypointGizmosRadius = 0.3f;
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public int GetNextIndex(int index)
        {
            if (index + 1 == transform.childCount) return 0;
            return index + 1;
        }

       public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }

    }
}