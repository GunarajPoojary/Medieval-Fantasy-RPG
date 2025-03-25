using UnityEngine;

namespace RPG
{
    [System.Serializable]
    public class PlayerAnimationData
    {
        [Header("State Group Parameter Names")]
        [SerializeField] private string _groundedParameterName = "Grounded";
        [SerializeField] private string _airborneParameterName = "Airborne";
        [SerializeField] private string _speedeParameterName = "Speed";

        [Header("Grounded Parameter Names")]
        [SerializeField] private string _rollParameterName = "isRolling";
        [SerializeField] private string _hardLandParameterName = "isHardLanding";

        [Header("Airborne Parameter Names")]
        [SerializeField] private string _fallParameterName = "isFalling";

        public int GroundedParameterHash { get; private set; }
        public int AirborneParameterHash { get; private set; }
        
        public int RollParameterHash { get; private set; }
        public int HardLandParameterHash { get; private set; }

        public int FallParameterHash { get; private set; }
        public int SpeedParameterHash { get; private set; }
        public float AnimationBlend { get; set; }

        public void Initialize()
        {
            GroundedParameterHash = Animator.StringToHash(_groundedParameterName);
            AirborneParameterHash = Animator.StringToHash(_airborneParameterName);
            
            RollParameterHash = Animator.StringToHash(_rollParameterName);
            HardLandParameterHash = Animator.StringToHash(_hardLandParameterName);

            FallParameterHash = Animator.StringToHash(_fallParameterName);
            
            SpeedParameterHash = Animator.StringToHash(_speedeParameterName);

        }
    }
}