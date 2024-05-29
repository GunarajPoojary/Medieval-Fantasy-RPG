using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GunarajCode
{
    /// <summary>
    /// Represents a group of tabs used for selecting different content.
    /// </summary>
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<TabButton> _tabButtons = new List<TabButton>();

        public Sprite TabIdle;
        public Sprite TabHover;
        public Sprite TabSelected;

        public UnityEvent OnTabSelect;

        [SerializeField] protected List<GameObject> _objectsToSwap;

        protected TabButton _selectedTab;

        /// <summary>
        /// Called when the pointer enters a tab button.
        /// </summary>
        /// <param name="button">The tab button entered by the pointer.</param>
        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if (_selectedTab == null || button != _selectedTab)
                button.Background.sprite = TabHover;
        }

        /// <summary>
        /// Called when the pointer exits a tab button.
        /// </summary>
        /// <param name="button">The tab button exited by the pointer.</param>
        public void OnTabExit(TabButton button) => ResetTabs();

        /// <summary>
        /// Called when a tab button is clicked.
        /// </summary>
        /// <param name="button">The tab button that was clicked.</param>
        public virtual void OnTabClick(TabButton button)
        {
            // Sets the clicked button as the selected tab
            _selectedTab = button;
            ResetTabs();
            // Changes the sprite to selected state for the clicked button
            button.Background.sprite = TabSelected;
            // Gets the index of the clicked button
            int index = button.transform.GetSiblingIndex();
            // Activates the corresponding object associated with the clicked tab
            for (int i = 0; i < _objectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    // Invokes the OnTabSelect event
                    OnTabSelect?.Invoke();
                    _objectsToSwap[i].SetActive(true);
                }
                else
                {
                    _objectsToSwap[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// Resets all tab buttons to their default state.
        /// </summary>
        public void ResetTabs()
        {
            foreach (var button in _tabButtons)
            {
                if (_selectedTab != null && button == _selectedTab)
                    continue;

                button.Background.sprite = TabIdle;
            }
        }
    }
}
