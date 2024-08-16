using RPG.Player.Data.States;
using UnityEngine;

namespace RPG.Player.Data
{
    [CreateAssetMenu(fileName = "Player", menuName = "Custom/Characters/Player")]
    public class PlayerSO : ScriptableObject
    {
        [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
        [field: SerializeField] public PlayerAirborneData AirborneData { get; private set; }
    }
}