namespace GunarajCode
{
    /// <summary>
    /// Represents a tab group specifically for managing slots.
    /// </summary>
    public class SlotsTab : TabGroup
    {
        // The index of the currently selected tab
        private int _num;

        /// <summary>
        /// Called when a tab button is clicked.
        /// </summary>
        /// <param name="button">The tab button that was clicked.</param>
        public override void OnTabClick(TabButton button)
        {
            _selectedTab = button;
            ResetTabs();
            button.Background.sprite = TabSelected;
            int index = button.transform.GetSiblingIndex();
            for (int i = 0; i < _objectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    _num = i;
                    _objectsToSwap[i].SetActive(true);
                }
                else
                    _objectsToSwap[i].SetActive(false);
            }
        }

        /// <summary>
        /// Deactivates all buttons in the tab group.
        /// </summary>
        public void DeactivateButtons()
        {
            _selectedTab = null;
            _objectsToSwap[_num].SetActive(false);
        }
    }
}
