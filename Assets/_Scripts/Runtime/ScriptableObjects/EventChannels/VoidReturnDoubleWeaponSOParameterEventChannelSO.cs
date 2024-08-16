using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying two parameters, both of type WeaponSO.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Double Parameter/Void Return Double WeaponSO Parameter Event Channel")]
    public class VoidReturnDoubleWeaponSOParameterEventChannelSO : GenericVoidReturnDoubleParameterEventChannelSO<WeaponSO, WeaponSO> { }
}
