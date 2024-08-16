using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying a single int parameter and returns WearableSO value.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Non-Void Return Type/Single Parameter/WearableSO Return int Parameter Event Channel")]
    public class WearableSOReturnIntParameterEventChannelSO : GenericNonVoidReturnSingleParameterEventChannelSO<int, WearableSO> { }
}