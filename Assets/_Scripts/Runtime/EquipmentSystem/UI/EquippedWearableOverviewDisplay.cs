using UnityEngine;

namespace RPG
{
    public class EquippedWearableOverviewDisplay : EquippedEquipmentOverviewDisplay
    {
        //[SerializeField] private VoidReturnDoubleWearableSOParameterEventChannelSO _wearableChangedEventChannelSO; // Event channel for wearable changes
        //[Space]
        //[SerializeField] private WearableSOArrayReturnNonParameterEventChannelSO _equippedWearableEventChannelSO; // Event channel to get equipped wearables

        private void OnEnable()
        {
            //_wearableChangedEventChannelSO.OnEventRaised += OnWearableChanged;

            // Display the first equipped wearable when enabled
            //DisplayFirstEquippedWearable(_equippedWearableEventChannelSO.RaiseEvent());
        }

        private void OnDisable()
        {
            //_wearableChangedEventChannelSO.OnEventRaised -= OnWearableChanged;
        }

        // Displays the first equipped wearable in the list, or none if all are null
        private void DisplayFirstEquippedWearable(WearableSO[] equippedWearables)
        {
            foreach (WearableSO wearable in equippedWearables)
            {
                if (wearable != null)
                {
                    UpdateOverview(wearable); // Update the overview with the first non-null wearable
                    return;
                }
            }

            UpdateOverview(null); // If all are null, clear the overview
        }

        // Updates the overview when the wearable changes
        private void OnWearableChanged(WearableSO newArmor, WearableSO oldArmor) => UpdateOverview(newArmor);

        // Returns the type text specific to the wearable equipment
        protected override string GetTypeText(EquipmentSO equipment)
        {
            WearableSO wearable = (WearableSO)equipment;
            return wearable.WearableType.ToString();
        }
    }
}