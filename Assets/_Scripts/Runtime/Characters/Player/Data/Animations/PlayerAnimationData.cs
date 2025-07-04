using UnityEngine;

namespace ProjectEmbersteel.Player.Data.Animations
{
    /// <summary>
    /// Holds animator parameter names and their hashed values used to control player animation states.
    /// Supports both grounded and airborne state groups and provides initialization for hashing.
    /// </summary>
    [System.Serializable]
    public class PlayerAnimationData
    {
        [Header("State Group Parameter Names")]
        [SerializeField] private string _groundedParameterName = "Grounded";
        [SerializeField] private string _airborneParameterName = "Airborne";
        [SerializeField] private string _speedeParameterName = "Speed";

        [Header("Airborne Parameter Names")]
        [SerializeField] private string _fallParameterName = "isFalling";

        [field: SerializeField] public float AnimationBlendSpeed { get; private set; } = 10f;
        [field: SerializeField] public float WalkThreshold { get; private set; } = 2f;
        [field: SerializeField] public float RunThreshold { get; private set; } = 5.335f;

        public int GroundedParameterHash { get; private set; }
        public int AirborneParameterHash { get; private set; }

        public int FallParameterHash { get; private set; }
        public int SpeedParameterHash { get; private set; }
        public float MovementAnimationBlend { get; set; }

        public void Initialize()
        {
            GroundedParameterHash = Animator.StringToHash(_groundedParameterName);
            AirborneParameterHash = Animator.StringToHash(_airborneParameterName);

            FallParameterHash = Animator.StringToHash(_fallParameterName);

            SpeedParameterHash = Animator.StringToHash(_speedeParameterName);
        }
    }
}