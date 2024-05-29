namespace GunarajCode
{
    public class CharacterScreenItemInfo : ItemInfoUI
    {
        public void Equip()
        {
            _item.Use();
            Inventories.Inventory.Instance.Remove(_item);

            _container.SetActive(false);

            Destroy(_slotPrefab);
        }
    }
}
