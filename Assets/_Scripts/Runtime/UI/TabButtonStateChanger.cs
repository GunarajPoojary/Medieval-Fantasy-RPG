using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    /// <summary>
    /// Changes the visual state of tab buttons based on user interactions.
    /// </summary>
    public class TabButtonStateChanger : ITabButtonStateChangeable
    {
        private readonly Color _tabIdleColor;
        private readonly Color _tabHoverColor;
        private readonly Color _tabSelectedColor;

        public TabButtonStateChanger(Color tabIdleColor, Color tabHoverColor, Color tabSelectedColor)
        {
            _tabIdleColor = tabIdleColor;
            _tabHoverColor = tabHoverColor;
            _tabSelectedColor = tabSelectedColor;
        }

        #region ITabButtonStateChanger Methods
        public void ChangeStateOnEnter(TabButton hoveredButton, TabButton selectedTab)
        {
            ResetTabs(hoveredButton.TabGroup);

            if (selectedTab == null || hoveredButton != selectedTab)
            {
                hoveredButton.Background.color = _tabHoverColor;
            }
        }

        public void ChangeStateOnExit(List<TabButton> tabButtons, TabButton selectedTab)
        {
            foreach (var button in tabButtons)
            {
                if (selectedTab != null && button == selectedTab)
                {
                    continue;
                }

                button.Background.color = _tabIdleColor;
            }
        }

        public void ChangeStateOnClick(TabButton clickedButton, TabButton selectedTab, List<GameObject> objectsToSwap)
        {
            if (clickedButton == null || clickedButton.Background == null)
            {
                return;
            }

            ResetTabs(clickedButton.TabGroup);

            clickedButton.Background.color = _tabSelectedColor;

            int index = clickedButton.transform.GetSiblingIndex();

            for (int i = 0; i < objectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    objectsToSwap[i].SetActive(true);
                }
                else
                {
                    objectsToSwap[i].SetActive(false);
                }
            }
        }
        #endregion

        private void ResetTabs(TabGroup tabGroup)
        {
            foreach (var button in tabGroup.TabButtons)
            {
                if (tabGroup.SelectedTab != null && button == tabGroup.SelectedTab)
                {
                    button.Background.color = _tabSelectedColor;
                }
                else
                {
                    button.Background.color = _tabIdleColor;
                }
            }
        }
    }
}