using GunarajCode.ScriptableObjects;
using System.Collections.Generic;

namespace GunarajCode
{
    public class ItemDatabase : Singleton<ItemDatabase>
    {
        public List<ItemObject> Items;  // List of all available items

        private Dictionary<string, ItemObject> _itemLookup;

        protected override void Awake()
        {
            base.Awake();
            _itemLookup = new Dictionary<string, ItemObject>();
            foreach (var item in Items)
            {
                _itemLookup[item.ID] = item;  // Use DisplayName or another unique identifier
            }
        }

        public ItemObject GetItemByID(string id)
        {
            _itemLookup.TryGetValue(id, out var item);
            return item;
        }
    }
}
