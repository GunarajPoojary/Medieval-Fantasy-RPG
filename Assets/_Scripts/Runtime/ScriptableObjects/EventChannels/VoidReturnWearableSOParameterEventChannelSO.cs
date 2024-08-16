using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying a single WearableSO parameter.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Single Parameter/Void Return WearableSO Parameter Event Channel")]
    public class VoidReturnWearableSOParameterEventChannelSO : GenericVoidReturnSingleParameterEventChannelSO<WearableSO> { }
}