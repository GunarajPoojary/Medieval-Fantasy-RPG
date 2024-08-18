using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    /// <summary>
    /// Interface for changing the visual state of tab buttons based on user interactions.
    /// </summary>
    public interface ITabButtonStateChangeable
    {
        void ChangeStateOnEnter(TabButton hoveredButton, TabButton selectedTab);

        void ChangeStateOnExit(List<TabButton> tabButtons, TabButton selectedTab);

        void ChangeStateOnClick(TabButton clickedButton, TabButton selectedTab, List<GameObject> objectsToSwap);
    }
}