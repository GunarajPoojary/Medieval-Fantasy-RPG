using RPG.Core;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.ScriptableObjects.EventChannels
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying two parameters: an ItemSO and a GameObject.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Double Parameter/Void Return ItemSO And GameObject Parameter Event Channel")]
    public class VoidReturnItemSOAndGameObjectParameterEventChannelSO : GenericVoidReturnDoubleParameterEventChannelSO<ItemSO, GameObject> { }
}
