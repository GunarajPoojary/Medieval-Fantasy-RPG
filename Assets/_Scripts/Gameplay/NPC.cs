using GunarajCode.Gameplay;
using UnityEngine;

namespace GunarajCode
{
    public class NPC : MonoBehaviour, IInteractable
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
