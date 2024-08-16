using RPG.ScriptableObjects.Items;
using RPG.ScriptableObjects.Stats;
using UnityEditor;
using UnityEngine;

namespace RPG.Editor
{
    /// <summary>
    /// Custom editor for Equipment ScriptableObject.
    /// Allows creating a new BaseStats asset directly from the editor if none is assigned.
    /// </summary>
    [CustomEditor(typeof(EquipmentSO), true)]
    public class BaseStatsAssetCreator : UnityEditor.Editor
    {
        private string _baseStatsName = "EquipmentStats";

        private string _path = "Weapons";

        public override void OnInspectorGUI()
        {
            EquipmentSO equipment = (EquipmentSO)target;

            if (equipment.EquipmentStats == null)
            {
                _baseStatsName = EditorGUILayout.TextField("New Asset Name", _baseStatsName);
                _path = EditorGUILayout.TextField("New Asset Path", _path);

                if (GUILayout.Button("Create New EquipmentStats"))
                {
                    if (string.IsNullOrEmpty(_baseStatsName) || string.IsNullOrEmpty(_path))
                    {
                        Debug.LogWarning("Asset Name or Path cannot be empty.");
                        return;
                    }

                    BaseStats baseStats = CreateInstance<BaseStats>();

                    string path = $"Assets/Game/Resources/Stats/EquipmentStats/{_path}/{_baseStatsName}.asset";

                    AssetDatabase.CreateAsset(baseStats, path);
                    AssetDatabase.SaveAssets();

                    equipment.EquipmentStats = baseStats;
                }
            }

            base.OnInspectorGUI();
        }
    }
}