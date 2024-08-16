using RPG.Player.Sounds;
using UnityEngine;

namespace RPG.Player.Utils
{
    public class PlayerAnimationEventTrigger : MonoBehaviour
    {
        [SerializeField] private AudioClip _footstepSound;
        [SerializeField] private AudioSource _audioSource;

        private Player _player;
        private IFootstepHandler _footstepHandler;

        private void Awake()
        {
            _player = transform.parent.GetComponent<Player>();
            _footstepHandler = new PlayerFootstepHandler(_audioSource, _footstepSound);
        }

        public void TriggerOnMovementStateAnimationEnterEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            _player.OnMovementStateAnimationEnterEvent();
        }

        public void TriggerOnMovementStateAnimationExitEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            _player.OnMovementStateAnimationExitEvent();
        }

        public void TriggerOnMovementStateAnimationTransitionEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            _player.OnMovementStateAnimationTransitionEvent();
        }

        public void TriggerFootstepEvent() => _footstepHandler?.PlayFootstep();

        private bool IsInAnimationTransition(int layerIndex = 0) => _player.Animator.IsInTransition(layerIndex);
    }
}