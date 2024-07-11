using RPG.Gameplay.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.Gameplay.EquipmentSystem
{
    /// <summary>
    /// Base class for displaying equipped equipment details in the UI.
    /// </summary>
    public abstract class EquippedEquipmentDetail : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI nameText;
        [SerializeField] protected TextMeshProUGUI statsText;
        [SerializeField] protected TextMeshProUGUI typeText;
        [SerializeField] protected GameObject noEquipmentTextObject;
        [SerializeField] protected GameObject equipmentDetailContainer;

        /// <summary>
        /// Updates the UI with the details of the given equipment.
        /// </summary>
        /// <param name="equipment">The equipment to display.</param>
        protected void UpdateDetailUI(EquipmentSO equipment)
        {
            if (equipment != null)
            {
                nameText.text = equipment.name;
                typeText.text = GetTypeText(equipment);
                statsText.text = equipment.EquipmentStats.ToString();
                equipmentDetailContainer.SetActive(true);
                noEquipmentTextObject.SetActive(false);
            }
            else
            {
                equipmentDetailContainer.SetActive(false);
                noEquipmentTextObject.SetActive(true);
            }
        }

        /// <summary>
        /// Gets the type text for the given equipment.
        /// </summary>
        /// <param name="equipment">The equipment to get the type text for.</param>
        /// <returns>The type text.</returns>
        protected abstract string GetTypeText(EquipmentSO equipment);
    }
}
