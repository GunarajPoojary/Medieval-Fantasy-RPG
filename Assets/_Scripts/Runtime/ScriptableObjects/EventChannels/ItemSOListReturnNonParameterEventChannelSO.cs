using RPG.Core;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event with no parameters and returns a list of ItemSO.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Non-Void Return Type/Non-Parameter/ItemSO List Return Non-Parameter Event Channel")]
    public class ItemSOListReturnNonParameterEventChannelSO : GenericNonVoidNonParameterEventChannelSO<List<ItemSO>> { }
}