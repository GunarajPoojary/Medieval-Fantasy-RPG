using UnityEngine;

namespace ProjectEmbersteel.Player.Data.Sounds
{
    /// <summary>
    /// Serializable data container for player footstep and landing audio clips.
    /// This class is intended to be used to assign audio clips via the Unity Inspector.
    /// </summary>
    [System.Serializable]
    public class PlayerFootStepAudioData
    {
        public AudioClip[] FootstepAudioClips;
        public AudioClip LandingAudioClip;
    }
}