using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace RPG.Controller
{
    [RequireComponent(typeof(PlayableDirector))]

    public class CinematicController : MonoBehaviour
    {
        private GameObject player;
        private PlayableDirector playableDirector;
        private bool alreadyTriggered = false;
        private EntityController controller;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            playableDirector = GetComponent<PlayableDirector>();
            controller = player.GetComponent<PCController>();

            playableDirector.played += PlaySequence;
            playableDirector.stopped += StopSequence;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyTriggered && other.gameObject.CompareTag("Player"))
            {
                alreadyTriggered = true;
                playableDirector.Play();
            }
        }

        private void PlaySequence(PlayableDirector director)
        {
            controller.CancelAttack();
            controller.StopMoving();
            controller.enabled = false;
        }

        private void StopSequence(PlayableDirector director)
        {
            controller.enabled = true;
        }



    }
}
