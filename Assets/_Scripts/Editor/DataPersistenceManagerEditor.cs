using UnityEditor;
using UnityEngine;

namespace RPG
{
    [CustomEditor(typeof(DataPersistenceManager))]
    public class DataPersistenceManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var dataPersistenceManager = (DataPersistenceManager)target;
            var gameName = dataPersistenceManager.GameData.Name;

            DrawDefaultInspector();

            if (GUILayout.Button("Save Game"))
            {
                dataPersistenceManager.SaveGame();
            }
            
            if (GUILayout.Button("Load Game"))
            {
                dataPersistenceManager.LoadGame(gameName);
            }
            
            if (GUILayout.Button("Delete Game"))
            {
                dataPersistenceManager.DeleteGame(gameName);
            }
        }
    }
}