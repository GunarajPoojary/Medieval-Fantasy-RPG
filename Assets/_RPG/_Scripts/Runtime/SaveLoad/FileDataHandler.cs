using System;
using System.IO;
using UnityEngine;

namespace RPG.SaveLoad
{
    public class FileDataHandler
    {
        private string _dataDirPath = "";
        private string _dataDirFileName = "";

        private bool _useEncryption = false;
        private readonly string _encryptionCodeWord = "Word";

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
                    loadData = JsonUtility.FromJson<GameData>(dataLoad);

                    if (_useEncryption)
                        dataLoad = EncryptDecrypt(dataLoad);
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
                    dataToStore = EncryptDecrypt(dataToStore);

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
                modifiedData += (char)(data[i] ^ _encryptionCodeWord[i % _encryptionCodeWord.Length]);

            return modifiedData;
        }
    }
}
