using UnityEngine;

namespace RPG.Item
{
    public enum ArmorEquipSlot
    {
        Head,
        Chest,
        Arms,
        Belt,
        Legs,
        Feet
    }

    [CreateAssetMenu(fileName = "NewArmor", menuName = "Custom/Items/Armor", order = 2)]
    public class ArmorSO : EquipmentSO
    {
        [SerializeField] private ArmorEquipSlot _equipSlot;

        public ArmorEquipSlot EquipSlot => _equipSlot;
        //public SkinnedMeshRenderer SkinnedMesh;
    }
}