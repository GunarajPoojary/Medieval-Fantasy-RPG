namespace RPG.UI
{
    /// <summary>
    /// Interface for UI elements that can be toggled active or inactive.
    /// </summary>
    public interface IUIElement
    {
        /// <summary>
        /// Toggles the UI element's active state.
        /// </summary>
        /// <param name="shouldSetActive">If true, activates the UI element; otherwise, deactivates it.</param>
        void ToggleUIs(bool shouldSetActive);
    }
}