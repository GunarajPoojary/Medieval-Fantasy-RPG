using UnityEngine;

namespace RPG.Player.Sounds
{
    public class PlayerFootstepHandler : IFootstepHandler
    {
        private readonly AudioClip _footstepSound;
        private readonly AudioSource _audioSource;

        public PlayerFootstepHandler(AudioSource audioSource, AudioClip footstepSound)
        {
            _audioSource = audioSource;
            _footstepSound = footstepSound;
        }

        public void PlayFootstep()
        {
            if (_footstepSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_footstepSound);
            }
        }
    }
}
