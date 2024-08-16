using RPG.Gameplay.World;
using UnityEngine;

namespace RPG.Gameplay.NPC
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private Animator _animator;

        private int _talkingHash;
        private const string TALKING = "Talking";

        private void Awake() => _talkingHash = Animator.StringToHash(TALKING);

        public void Interact() => _animator.SetBool(_talkingHash, true);

        public Transform GetTransform() => transform;
    }
}