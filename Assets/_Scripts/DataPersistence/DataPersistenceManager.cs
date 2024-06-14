using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GunarajCode
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [SerializeField] private bool _initializeDataIfNull = false;

        [Header("File storage config")]
        [SerializeField] private string _fileName;
        [SerializeField] private bool _useEncryption;
        private FileDataHandler _dataHandler;

        private GameData _gameData;
        private List<IDataPersistence> _dataPersistenceObjects = new List<IDataPersistence>();

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

        public void NewGame() => _gameData = new GameData();

        public void SaveGame()
        {
            if (_gameData == null)
            {
                Debug.LogWarning("No data was found. A new game needs to be started before data can be loaded");
                return;
            }

            _dataPersistenceObjects.ForEach(dataPersistenceObj =>
            {
                dataPersistenceObj?.SaveData(_gameData);
            });

            _dataHandler.Save(_gameData);
        }

        private void LoadGame()
        {
            _gameData = _dataHandler.Load();
            if (_gameData == null && _initializeDataIfNull)
            {
                NewGame();
            }

            if (_gameData == null)
            {
                Debug.Log("No data was found. A new game needs to be started before data can be loaded");
                return;
            }

            _dataPersistenceObjects.ForEach(dataPersistenceObj =>
            {
                dataPersistenceObj?.LoadData(_gameData);
            });
        }

        private void OnApplicationQuit() => SaveGame();

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
            return new List<IDataPersistence>(dataPersistenceObjects);
        }

        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        public bool HasGameData() => _gameData != null;
    }
}
