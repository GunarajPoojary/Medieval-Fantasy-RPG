using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying a single string parameter and returns an ItemSO value.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Non-Void Return Type/Single Parameter/ItemSO Return string Parameter Event Channel")]
    public class ItemSOReturnStringParameterEventChannelSO : GenericNonVoidReturnSingleParameterEventChannelSO<string, ItemSO> { }
}