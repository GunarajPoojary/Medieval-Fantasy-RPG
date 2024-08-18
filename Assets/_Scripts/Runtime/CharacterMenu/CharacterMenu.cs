using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.CharacterMenu
{
    /// <summary>
    /// Manages the character menu, including loading items and switching between different equipment views.
    /// </summary>
    public class CharacterMenu : MonoBehaviour
    {
        [SerializeField] private ItemSOListReturnNonParameterEventChannelSO _getItems;  // Event channel to get a list of items.

        private CharacterMenuUIHandler _characterMenuUIHandler;

        private IEquipmentOverviewDisplayer _weaponOverviewHandler;
        private IEquipmentOverviewDisplayer _wearableOverviewHandler;
        private IEquipmentAdder _weaponAdder;
        private IEquipmentAdder _wearableAdder;

        private void Awake()
        {
            _characterMenuUIHandler = GetComponent<CharacterMenuUIHandler>();

            _weaponOverviewHandler = GetComponent<WeaponOverviewHandler>();
            _weaponAdder = (WeaponOverviewHandler)_weaponOverviewHandler;

            _wearableOverviewHandler = GetComponent<WearablesOverviewHandler>();
            _wearableAdder = (WearablesOverviewHandler)_wearableOverviewHandler;
        }

        private void Start()
        {
            // Add items to their respective UI containers based on type.
            foreach (ItemSO item in _getItems.RaiseEvent())
            {
                if (item is WeaponSO weapon)
                {
                    _weaponAdder.AddEquipment(weapon);
                }
                else if (item is WearableSO wearable)
                {
                    _wearableAdder.AddEquipment(wearable);
                }
            }
        }

        /// <summary>
        /// Switches between different equipment selection screens.
        /// </summary>
        /// <param name="equipmentSelectionScreen">The screen to switch to.</param>
        public void Switch(GameObject equipmentSelectionScreen)
        {
            if (equipmentSelectionScreen == _characterMenuUIHandler.GetWeaponSelectionContainer())
            {
                _characterMenuUIHandler.SetPanels(mainPanelSetActive: false, weaponSelectionSetActive: true, armorSelectionSetActive: false);
                _weaponOverviewHandler.DisplayEquipmentOverview();
            }
            else if (equipmentSelectionScreen == _characterMenuUIHandler.GetArmorSelectionContainer())
            {
                _characterMenuUIHandler.SetPanels(mainPanelSetActive: false, weaponSelectionSetActive: false, armorSelectionSetActive: true);
                _wearableOverviewHandler.DisplayEquipmentOverview();
            }
            else if (equipmentSelectionScreen == _characterMenuUIHandler.GetMainPanelContainer())
            {
                _characterMenuUIHandler.SetPanels(mainPanelSetActive: true, weaponSelectionSetActive: false, armorSelectionSetActive: false);
            }
        }
    }
}