using System;
using System.IO;
using UnityEngine;

namespace RPG.Core.SaveLoad
{
    /// <summary>
    /// Handles reading and writing game data to and from a file.
    /// </summary>
    public class FileDataHandler
    {
        private readonly string _encryptionCodeWord = "Word";

        private string _dataDirPath = "";
        private string _dataDirFileName = "";
        private bool _useEncryption = false;

        public FileDataHandler(string dataDirPath, string dataDirFileName, bool useEncryption)
        {
            _dataDirPath = dataDirPath;
            _dataDirFileName = dataDirFileName;
            _useEncryption = useEncryption;
        }

        public GameData Load()
        {
            string fullPath = Path.Combine(_dataDirPath, _dataDirFileName);
            GameData loadData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string dataLoad = File.ReadAllText(fullPath);

                    if (_useEncryption)
                    {
                        dataLoad = EncryptDecrypt(dataLoad);
                    }

                    loadData = JsonUtility.FromJson<GameData>(dataLoad);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
                }
            }
            else
            {
                Debug.LogWarning("Save file not found at: " + fullPath);
            }

            return loadData;
        }

        public void Save(GameData data)
        {
            string fullPath = Path.Combine(_dataDirPath, _dataDirFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);

                if (_useEncryption)
                {
                    dataToStore = EncryptDecrypt(dataToStore);
                }

                File.WriteAllText(fullPath, dataToStore);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
            }
        }

        private string EncryptDecrypt(string data)
        {
            string modifiedData = "";

            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);
            }

            return modifiedData;
        }
    }
}