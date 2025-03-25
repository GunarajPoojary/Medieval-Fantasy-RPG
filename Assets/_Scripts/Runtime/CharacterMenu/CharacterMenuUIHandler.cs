using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Handles the visibility and activation of character menu panels.
    /// </summary>
    public class CharacterMenuUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _mainPanelContainer;
        [SerializeField] private GameObject _weaponSelectionContainer;
        [SerializeField] private GameObject _armorSelectionContainer;

        public void SetPanels(bool mainPanelSetActive, bool weaponSelectionSetActive, bool armorSelectionSetActive)
        {
            _mainPanelContainer.SetActive(mainPanelSetActive);
            _weaponSelectionContainer.SetActive(weaponSelectionSetActive);
            _armorSelectionContainer.SetActive(armorSelectionSetActive);
        }

        public GameObject GetMainPanelContainer() => _mainPanelContainer;
        public GameObject GetWeaponSelectionContainer() => _weaponSelectionContainer;
        public GameObject GetArmorSelectionContainer() => _armorSelectionContainer;
    }
}