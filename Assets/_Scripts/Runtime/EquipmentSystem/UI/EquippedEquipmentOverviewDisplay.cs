using RPG.ScriptableObjects.Items;
using TMPro;
using UnityEngine;

namespace RPG.EquipmentSystem
{
    public abstract class EquippedEquipmentOverviewDisplay : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _nameText;
        [SerializeField] protected TextMeshProUGUI _statsText;
        [SerializeField] protected TextMeshProUGUI _typeText;

        [SerializeField] protected GameObject _noEquipmentTextObject; // UI object shown when no equipment is equipped
        [SerializeField] protected GameObject _equipmentOverviewContainer;

        // Updates the overview UI with the given equipment's details
        protected void UpdateOverview(EquipmentSO equipment)
        {
            if (equipment != null)
            {
                _nameText.text = equipment.name;
                _typeText.text = GetTypeText(equipment);
                _statsText.text = equipment.EquipmentStats.ToString();

                _equipmentOverviewContainer.SetActive(true);
                _noEquipmentTextObject.SetActive(false);
            }
            else
            {
                _equipmentOverviewContainer.SetActive(false);
                _noEquipmentTextObject.SetActive(true);
            }
        }

        protected abstract string GetTypeText(EquipmentSO equipment);
    }
}