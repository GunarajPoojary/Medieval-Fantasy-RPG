using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Editor
{
    public class CopyModifiedPlayerGameobject : EditorWindow
    {
        private GameObject playerObject;
        private GameObject[] childrenToRemove = new GameObject[0];
        private string destinationScenePath;
        private List<string> componentsToRemove = new List<string>();

        [MenuItem("Tools/Modify And Copy Player Gameobject")]
        public static void ShowWindow()
        {
            GetWindow<CopyModifiedPlayerGameobject>("Copy Player Object");
        }

        private void OnGUI()
        {
            GUILayout.Label("Source Scene Player Object", EditorStyles.boldLabel);
            playerObject = (GameObject)EditorGUILayout.ObjectField("Player Object", playerObject, typeof(GameObject), true);

            GUILayout.Label("Children to Remove", EditorStyles.boldLabel);
            int newChildCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Children to Remove", childrenToRemove.Length));
            if (newChildCount != childrenToRemove.Length)
            {
                GameObject[] newChildrenToRemove = new GameObject[newChildCount];
                for (int i = 0; i < Mathf.Min(newChildCount, childrenToRemove.Length); i++)
                {
                    newChildrenToRemove[i] = childrenToRemove[i];
                }
                childrenToRemove = newChildrenToRemove;
            }

            for (int i = 0; i < childrenToRemove.Length; i++)
            {
                childrenToRemove[i] = (GameObject)EditorGUILayout.ObjectField("Child to Remove " + (i + 1), childrenToRemove[i], typeof(GameObject), true);
            }

            GUILayout.Label("Components to Remove", EditorStyles.boldLabel);
            int newComponentCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Components to Remove", componentsToRemove.Count));
            if (newComponentCount != componentsToRemove.Count)
            {
                List<string> newComponentsToRemove = new List<string>(newComponentCount);
                for (int i = 0; i < Mathf.Min(newComponentCount, componentsToRemove.Count); i++)
                {
                    newComponentsToRemove.Add(componentsToRemove[i]);
                }
                componentsToRemove = newComponentsToRemove;
            }

            for (int i = 0; i < componentsToRemove.Count; i++)
            {
                componentsToRemove[i] = EditorGUILayout.TextField("Component to Remove " + (i + 1), componentsToRemove[i]);
            }

            GUILayout.Label("Destination Scene Path", EditorStyles.boldLabel);
            destinationScenePath = EditorGUILayout.TextField("Scene Path", destinationScenePath);

            if (GUILayout.Button("Copy and Modify Player Object"))
            {
                CopyAndModifyPlayerObject();
            }
        }

        private void CopyAndModifyPlayerObject()
        {
            if (playerObject == null)
            {
                Debug.LogError("Player object is not assigned.");
                return;
            }

            if (string.IsNullOrEmpty(destinationScenePath))
            {
                Debug.LogError("Destination scene path is not provided.");
                return;
            }

            // Save the current scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            // Open the destination scene
            Scene destinationScene = EditorSceneManager.OpenScene(destinationScenePath, OpenSceneMode.Additive);

            // Copy the player object
            GameObject copiedPlayerObject = Instantiate(playerObject);

            // Remove specified children
            foreach (GameObject child in childrenToRemove)
            {
                if (child != null)
                {
                    Transform childTransform = copiedPlayerObject.transform.Find(child.name);
                    if (childTransform != null)
                    {
                        DestroyImmediate(childTransform.gameObject);
                    }
                }
            }

            // Remove specified components
            foreach (string componentName in componentsToRemove)
            {
                if (!string.IsNullOrEmpty(componentName))
                {
                    Type componentType = Type.GetType(componentName);
                    if (componentType != null)
                    {
                        Component component = copiedPlayerObject.GetComponent(componentType);
                        if (component != null)
                        {
                            DestroyImmediate(component);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Component type " + componentName + " not found.");
                    }
                }
            }

            // Move the copied player object to the destination scene
            SceneManager.MoveGameObjectToScene(copiedPlayerObject, destinationScene);

            // Save the destination scene
            EditorSceneManager.SaveScene(destinationScene);

            // Close the destination scene
            EditorSceneManager.CloseScene(destinationScene, true);

            Debug.Log("Player object copied and modified successfully.");
        }
    }
}