using UnityEngine;

namespace RPG.Gameplay.NPC
{
    public class NPC : MonoBehaviour, Gameplay.World.IInteractable
    {
        [SerializeField] private Animator _animator;
        private int _talkingHash;
        private const string TALKING = "Talking";

        private void Awake()
        {
            _talkingHash = Animator.StringToHash(TALKING);
        }

        public Transform InteractableTransform => transform;

        public void Interact()
        {
            _animator.SetBool(_talkingHash, true);
        }
    }
}
