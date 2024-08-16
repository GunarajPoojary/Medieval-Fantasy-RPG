using RPG.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Core.SaveLoad
{
    /// <summary>
    /// Manages the saving and loading of game data, and persists it across scenes.
    /// </summary>
    public class DataPersistenceManager : PersistentSingleton<DataPersistenceManager>
    {
        [Header("File storage config")]
        [SerializeField] private string _fileName;
        [SerializeField] private bool _useEncryption;
        [SerializeField] private bool _initializeDataIfNull = false;
        [Space]
        [SerializeField] private VoidReturnBoolParameterEventChannelSO _hasDataChannelSO; // Event channel to signal if data exists.
        [Space]
        [SerializeField] private VoidReturnNonParameterEventChannelSO _newGameChannelSO; // Event channel to start a new game.
        [Space]
        [SerializeField] private VoidReturnNonParameterEventChannelSO _continueGameChannelSO; // Event channel to continue a saved game.

        private FileDataHandler _dataHandler;
        private List<ISaveable> _dataPersistenceObjects = new List<ISaveable>();

        public GameData GameData { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;

            _newGameChannelSO.OnEventRaised += HandleNewGame;
            _continueGameChannelSO.OnEventRaised += HandleContinueGame;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;

            _newGameChannelSO.OnEventRaised -= HandleNewGame;
            _continueGameChannelSO.OnEventRaised -= HandleContinueGame;
        }

        private void OnApplicationQuit() => SaveGame();

        public void NewGame()
        {
            GameData = new GameData();
            SaveGame();
        }

        public void SaveGame()
        {
            if (GameData == null)
            {
                Debug.LogWarning("No data was found. A new game needs to be started before data can be saved.");
                return;
            }

            _dataPersistenceObjects.ForEach(dataPersistenceObj => dataPersistenceObj?.SaveData(GameData));

            _dataHandler.Save(GameData);
        }

        public void LoadGame()
        {
            GameData = _dataHandler.Load();

            if (GameData == null && _initializeDataIfNull)
            {
                NewGame();
            }
            else
            {
                _dataPersistenceObjects.ForEach(dataPersistenceObj => dataPersistenceObj?.LoadData(GameData));
            }

            _hasDataChannelSO.RaiseEvent(GameData != null);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }

        private void HandleContinueGame() => SaveGame();

        private void HandleNewGame() => NewGame();

        private List<ISaveable> FindAllDataPersistenceObjects()
        {
            IEnumerable<ISaveable> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
            return new List<ISaveable>(dataPersistenceObjects);
        }
    }
}