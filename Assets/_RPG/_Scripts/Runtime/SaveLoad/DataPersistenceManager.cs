using RPG.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SaveLoad
{
    public class DataPersistenceManager : MonoBehaviour
    {
        public event Action OnDataLoaded;

        [SerializeField] private bool _initializeDataIfNull = false;

        [Header("File storage config")]
        [SerializeField] private string _fileName;
        [SerializeField] private bool _useEncryption;
        private FileDataHandler _dataHandler;

        private GameData _gameData;
        public GameData GameData { get => _gameData; }
        private List<ISaveable> _dataPersistenceObjects = new List<ISaveable>();

        public static DataPersistenceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

        public void NewGame()
        {
            _gameData = new GameData();
            GameManager.Instance.IsFirstDataLoad = true;
            SaveGame();
        }

        public void SaveGame()
        {
            if (_gameData == null)
            {
                Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
                return;
            }

            _dataPersistenceObjects.ForEach(dataPersistenceObj => dataPersistenceObj?.SaveData(_gameData));
            _dataHandler.Save(_gameData);
        }

        public void LoadGame()
        {
            _gameData = _dataHandler.Load();

            if (_gameData == null && _initializeDataIfNull)
                NewGame();
            else
                _dataPersistenceObjects.ForEach(dataPersistenceObj => dataPersistenceObj?.LoadData(_gameData));

            OnDataLoaded?.Invoke();
        }

        private void OnApplicationQuit() => SaveGame();

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }

        private List<ISaveable> FindAllDataPersistenceObjects()
        {
            IEnumerable<ISaveable> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
            return new List<ISaveable>(dataPersistenceObjects);
        }

        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        public bool HasGameData() => _gameData != null;
    }
}
