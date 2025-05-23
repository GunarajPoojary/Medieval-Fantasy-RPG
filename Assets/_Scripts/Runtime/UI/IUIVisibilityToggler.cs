﻿namespace RPG
{
    /// <summary>
    /// Interface for UI elements that can be toggled active or inactive.
    /// Used when Inventory UI is Toggled.
    /// </summary>
    public interface IUIVisibilityToggler
    {
        void ToggleUIs(bool shouldSetActive);
    }
}