using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectEmbersteel
{
    public class DataPersistenceManager : MonoBehaviour
    {
        public static DataPersistenceManager Instance { get; private set; }
        [field: SerializeField] public GameData GameData { get; set; }

        private IDataService _dataService;

        private string _currentLevelName;

        private void Awake()
        {
            Instance = this;

            _dataService = new FileDataService(new JsonSerializer());
        }

        public void NewGame()
        {
            GameData = new GameData()
            {
                Name = "New Game",
                CurrentLevelName = "Main menu"
            };

            SceneManager.LoadScene(GameData.CurrentLevelName);
        }

        public void SaveGame()
        {
            _dataService.Save(GameData);
        }

        public void LoadGame(string gameName)
        {
            GameData = _dataService.Load(gameName);

            if (String.IsNullOrEmpty(GameData.CurrentLevelName))
            {
                GameData.CurrentLevelName = _currentLevelName;
            }

            SceneManager.LoadScene(GameData.CurrentLevelName);
        }

        public void ReloadGame() => LoadGame(GameData.Name);

        public void DeleteGame(string gameName)
        {
            _dataService.Delete(gameName);
        }
    }
}