using ProjectEmbersteel.Player.Data.States.Airborne;
using ProjectEmbersteel.Player.Data.States.Grounded;
using UnityEngine;

namespace ProjectEmbersteel.Player.Data.ScriptableObjects
{
    /// <summary>
    /// Holds configuration data for the player's state machine, including grounded and airborne states.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStateMachineDataSO", menuName = "Custom/Player/StateMachine/Data")]
    public class PlayerStateMachineConfigSO : DescriptionBaseSO
    {
        [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
        [field: SerializeField] public PlayerAirborneData AirborneData { get; private set; }
    }
}