using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ProjectEmbersteel
{
    public class FileDataService : IDataService
    {
        private ISerializer _serializer;
        private string _dataPath;
        private string _fileExtension;

        public FileDataService(ISerializer serializer)
        {
            this._dataPath = Application.persistentDataPath;
            this._fileExtension = "json";
            this._serializer = serializer;
        }

        private string GetPathToFile(string fileName)
        {
            return Path.Combine(_dataPath, string.Concat(fileName, "", _fileExtension));
        }
        
        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);

            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"The file '{data.Name}.{_fileExtension}' already exists and cannot be overwritten.");
            }
            
            File.WriteAllText(fileLocation, _serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (!File.Exists(fileLocation))
            {
                throw new ArgumentException($"No persisted GameData with name '{name}'.");
            }
            
            return _serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public void DeleteAll()
        {
            foreach (var filepath in Directory.GetFiles(_dataPath))
            {
                File.Delete(filepath);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (var path in Directory.EnumerateFiles(_dataPath))
            {
                if (Path.GetExtension(path) == _fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}