using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.UI
{
    public class ButtonSoundPlayer : IButtonSoundPlayer
    {
        private readonly AudioSource _audioSource;
        private readonly AudioClip _hoverSound;
        private readonly AudioClip _selectSound;

        public ButtonSoundPlayer(AudioSource audioSource, AudioClip hoverSound, AudioClip selectSound)
        {
            _audioSource = audioSource;
            _hoverSound = hoverSound;
            _selectSound = selectSound;
        }

        public void OnPointerEnter(BaseEventData data) => PlaySound(_hoverSound);

        public void OnPointerClick(BaseEventData data) => PlaySound(_selectSound);

        private void PlaySound(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }
}