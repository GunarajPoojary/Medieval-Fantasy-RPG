using AYellowpaper.SerializedCollections;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.EquipmentSystem
{
    /// <summary>
    /// Manages the player's default skins and provides methods to set and activate them
    /// </summary>
    public class PlayerDefaultSkins : MonoBehaviour, IDefaultSkinSetter, ISkinsActivator
    {
        [field: SerializeField] public SerializedDictionary<WearableSlot, GameObject> Skins { get; set; }

        // Ensure that the Head and Back slots do not have default skins
        private void OnValidate()
        {
            if (Skins.ContainsKey(WearableSlot.Head) || Skins.ContainsKey(WearableSlot.Back))
            {
                Skins.Remove(WearableSlot.Head);
                Skins.Remove(WearableSlot.Back);
            }
        }

        // Set the default skin for a specific slot based on its index and activate/deactivate it
        public void SetDefaultSkin(int index, bool shouldSetActive)
        {
            if (Skins.TryGetValue((WearableSlot)index, out var skin))
            {
                skin.SetActive(shouldSetActive);
            }
        }

        // Activate all default skins
        public void ActivateSkins()
        {
            foreach (var item in Skins)
            {
                item.Value.SetActive(true);
            }
        }
    }
}