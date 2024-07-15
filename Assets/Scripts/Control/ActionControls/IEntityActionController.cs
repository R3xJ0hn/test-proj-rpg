using System;
using UnityEngine;

namespace RPG.Controls.ActionControls
{
    public interface IEntityActionController
    {
        float ChaseDistance { set; }
        bool IsDead { set; }
        bool AllowedToRun { get; set; }

        event Action<GameObject> AttackEvent;
        event Action<Vector2> MoveByNormalizedVectorEvent;
        event Action<Vector3> MoveToEvent;
        event Action CancelAttackEvent;
        event Action SprintEvent;
        event Action StopMovingEvent;

    }
}