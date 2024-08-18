using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    /// <summary>
    /// Manages tab buttons, handling their states and interactions.
    /// </summary>
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private Color _tabIdleColor;

        [SerializeField] private Color _tabHoverColor;

        [SerializeField] private Color _tabSelectedColor;

        // List of GameObjects to swap when a tab is selected.
        [SerializeField] private List<GameObject> _objectsToSwap;

        private ITabButtonStateChangeable _stateChanger;

        [field: SerializeField] public List<TabButton> TabButtons { get; private set; } = new List<TabButton>();

        public TabButton SelectedTab { get; private set; }

        private void Awake()
        {
            _stateChanger = new TabButtonStateChanger(_tabIdleColor, _tabHoverColor, _tabSelectedColor);

            SetDefaultTab();
        }

        private void SetDefaultTab()
        {
            if (TabButtons != null && TabButtons.Count > 0)
            {
                OnTabClick(TabButtons[0]);
            }
        }

        public void OnTabEnter(TabButton button) => _stateChanger.ChangeStateOnEnter(button, SelectedTab);

        public void OnTabExit(TabButton button) => _stateChanger.ChangeStateOnExit(TabButtons, SelectedTab);

        public void OnTabClick(TabButton button)
        {
            SelectedTab = button;
            _stateChanger.ChangeStateOnClick(button, SelectedTab, _objectsToSwap);
        }
    }
}