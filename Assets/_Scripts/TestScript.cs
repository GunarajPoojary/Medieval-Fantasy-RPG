using UnityEngine;

namespace RPG
{
    public class TestScript : MonoBehaviour
    {
        [ColorUsage(true)]
        public Color color;

        public WearableSO wearable;
        public WearableSO secondWearable;
        public PlayerEquipmentHandler equipmentHandler;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Debug.Break();
                equipmentHandler.EquipWearable(wearable);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                equipmentHandler.UnequipWearable((int)wearable.EquipSlot);
            }
        }
    }
}
