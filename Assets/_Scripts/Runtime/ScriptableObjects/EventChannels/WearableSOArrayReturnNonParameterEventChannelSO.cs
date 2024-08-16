using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event with no parameters and returns an array of WearableSO.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Non-Void Return Type/Non-Parameter/WearableSO Array Return Non-Parameter Event Channel")]
    public class WearableSOArrayReturnNonParameterEventChannelSO : GenericNonVoidNonParameterEventChannelSO<WearableSO[]> { }
}
