using System;
using System.Collections.Generic;
using RPG.Loot;
using UnityEngine;

namespace RPG
{
    public class PickableItemManager : MonoBehaviour
    {
        public static PickableItemManager Instance { get; private set; }

        private Dictionary<ItemSO, IPickable> _pickables = new();

        private void Awake()
        {
            Instance = this;
        }

        public void OnTryPickingUpItem(IPickable pickable, ItemSO item)
        {
            _pickables.Add(item, pickable);
        }

        public void OnSuccessfulItemPickup(ItemSO item)
        {
            if (_pickables.TryGetValue(item, out IPickable pickable))
            {
                pickable.SetGameObject(false);
                _pickables.Remove(item);
            }
        }
    }
}