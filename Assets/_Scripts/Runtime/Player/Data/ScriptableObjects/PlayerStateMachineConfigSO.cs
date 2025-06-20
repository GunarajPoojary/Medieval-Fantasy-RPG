using RPG.Player.Data.States.Airborne;
using RPG.Player.Data.States.Grounded;
using UnityEngine;

namespace RPG.Player.Data.ScriptableObjects
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