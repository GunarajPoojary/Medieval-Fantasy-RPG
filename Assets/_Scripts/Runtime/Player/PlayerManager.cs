using RPG.Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Player
{
    public class PlayerManager : SimpleSingleton<PlayerManager>
    {
        public GameObject Player { get; private set; }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex != 0)
            {
                Player = GameObject.FindGameObjectWithTag("Player");
            }
        }
    }
}