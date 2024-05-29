using UnityEngine;

namespace GunarajCode.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Default Item", menuName = "Inventory/Items/Default")]
    public class DefaultObject : ItemObject
    {
        public int AttackBonus;
        public int DefenseBonus;

        public override void Use()
        {
            throw new System.NotImplementedException();
        }
    }
}
