using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying a single parameter of type WeaponSO.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Single Parameter/Void Return WeaponSO Parameter Event Channel")]
    public class VoidReturnWeaponSOParameterEventChannelSO : GenericVoidReturnSingleParameterEventChannelSO<WeaponSO> { }
}