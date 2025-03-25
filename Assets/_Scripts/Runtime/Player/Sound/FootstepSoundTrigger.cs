using UnityEngine;

namespace RPG
{
    public class FootstepSoundTrigger : MonoBehaviour
    {
        private CapsuleCollider _collider;

        [SerializeField] private AudioClip LandingAudioClip;
        [SerializeField] private AudioClip[] FootstepAudioClips;
        [Range(0, 1)][SerializeField] private float FootstepAudioVolume = 0.5f;

        private void Awake()
        {
            _collider = GetComponentInParent<CapsuleCollider>();
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_collider.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_collider.center), FootstepAudioVolume);
            }
        }
    }
}