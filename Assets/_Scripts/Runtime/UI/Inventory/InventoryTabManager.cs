using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEmbersteel.UI
{
    [Serializable]
    public class InventoryMainTabData
    {
        public InventoryTabButton mainTabButton;
        public GameObject mainTabContent;
        public InventorySubTabData[] subTabs;
    }

    [Serializable]
    public class InventorySubTabData
    {
        public InventoryTabButton subTabButton;
        public GameObject subTabContent;
    }

    public class InventoryTabManager : MonoBehaviour
    {
        [SerializeField] private InventoryMainTabData[] _inventoryTabs;
        private int _currentSelectedMainTabIndex = 0;
        private int _currentSelectedSubTabIndex = 0;
        private int _defaultSubTabIndex = 0;
        private readonly Dictionary<int, int> _lastSelectedSubTabs = new();

        private void Awake() => InitializeTabs();

        private void OnEnable() => SubscribeEvents(true);

        private void OnDisable() => SubscribeEvents(false);

        private void Start() => OnMainTabSelected(_currentSelectedMainTabIndex);

        private void InitializeTabs()
        {
            for (int i = 0; i < _inventoryTabs.Length; i++)
            {
                _inventoryTabs[i].mainTabButton.Initialize(i);
                _lastSelectedSubTabs.Add(i, _defaultSubTabIndex);

                InventorySubTabData[] subTabs = _inventoryTabs[i].subTabs;

                for (int j = 0; j < subTabs.Length; j++)
                    subTabs[j].subTabButton.Initialize(j);
            }
        }

        private void SubscribeEvents(bool subscribe)
        {
            for (int i = 0; i < _inventoryTabs.Length; i++)
            {
                if (subscribe)
                    _inventoryTabs[i].mainTabButton.OnClick += OnMainTabSelected;
                else
                    _inventoryTabs[i].mainTabButton.OnClick -= OnMainTabSelected;

                foreach (InventorySubTabData sub in _inventoryTabs[i].subTabs)
                {
                    if (subscribe)
                        sub.subTabButton.OnClick += OnSubTabSelected;
                    else
                        sub.subTabButton.OnClick -= OnSubTabSelected;
                }
            }
        }

        private void OnMainTabSelected(int index)
        {
            if (_currentSelectedMainTabIndex == index) return;

            _inventoryTabs[_currentSelectedMainTabIndex].mainTabContent.SetActive(false);
            _inventoryTabs[_currentSelectedMainTabIndex].subTabs[_currentSelectedSubTabIndex].subTabContent.SetActive(false);

            _lastSelectedSubTabs[_currentSelectedMainTabIndex] = _currentSelectedSubTabIndex;
            _inventoryTabs[index].mainTabContent.SetActive(true);

            if (_lastSelectedSubTabs.TryGetValue(index, out int subTabIndex))
                _lastSelectedSubTabs[index] = subTabIndex;

            _inventoryTabs[index].subTabs[subTabIndex].subTabContent.SetActive(true);

            _currentSelectedMainTabIndex = index;
            _currentSelectedSubTabIndex = subTabIndex;
        }

        private void OnSubTabSelected(int index)
        {
            if (_currentSelectedSubTabIndex.Equals(index)) return;

            _inventoryTabs[_currentSelectedMainTabIndex].subTabs[_currentSelectedSubTabIndex].subTabContent.SetActive(false);

            _inventoryTabs[_currentSelectedMainTabIndex].subTabs[index].subTabContent.SetActive(true);

            _currentSelectedSubTabIndex = index;
        }
    }
}