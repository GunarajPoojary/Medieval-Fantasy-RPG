using UnityEditor;

namespace RPG
{
    /// <summary>
    /// Custom editor for ItemPickUp.
    /// It also displays the custom editor for the associated item.
    /// </summary>
    [CustomEditor(typeof(ItemPickUp))]
    public class ItemPickUpEditor : Editor
    {
        private Editor _editorInstance;

        private void OnEnable() => _editorInstance = null;

        public override void OnInspectorGUI()
        {
            ItemPickUp itemPickUp = (ItemPickUp)target;

            if (_editorInstance == null)
            {
                _editorInstance = CreateEditor(itemPickUp.Item);
            }

            base.OnInspectorGUI();

            _editorInstance.DrawDefaultInspector();
        }
    }
}