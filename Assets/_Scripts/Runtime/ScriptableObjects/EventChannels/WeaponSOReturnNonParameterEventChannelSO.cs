using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event with no parameters and returns a WeaponSO.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Non-Void Return Type/Non-Parameter/WeaponSO Return Non-Parameter Event Channel")]
    public class WeaponSOReturnNonParameterEventChannelSO : GenericNonVoidNonParameterEventChannelSO<WeaponSO> { }
}