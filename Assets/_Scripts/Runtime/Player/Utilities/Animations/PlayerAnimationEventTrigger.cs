using UnityEngine;

namespace RPG
{
    public class PlayerAnimationEventTrigger : MonoBehaviour
    {
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = transform.parent.GetComponent<PlayerController>();
        }

        public void TriggerOnMovementStateAnimationEnterEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            _playerController.OnMovementStateAnimationEnterEvent();
        }

        public void TriggerOnMovementStateAnimationExitEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            _playerController.OnMovementStateAnimationExitEvent();
        }

        public void TriggerOnMovementStateAnimationTransitionEvent()
        {
            if (IsInAnimationTransition())
            {
                return;
            }

            _playerController.OnMovementStateAnimationTransitionEvent();
        }

        private bool IsInAnimationTransition(int layerIndex = 0) => _playerController.Animator.IsInTransition(layerIndex);
    }
}