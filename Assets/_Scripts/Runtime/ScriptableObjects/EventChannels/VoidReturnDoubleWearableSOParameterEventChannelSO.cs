using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying two parameters, both of type WearableSO.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Double Parameter/Void Return Double WearableSO Parameter Event Channel")]
    public class VoidReturnDoubleWearableSOParameterEventChannelSO : GenericVoidReturnDoubleParameterEventChannelSO<WearableSO, WearableSO> { }
}