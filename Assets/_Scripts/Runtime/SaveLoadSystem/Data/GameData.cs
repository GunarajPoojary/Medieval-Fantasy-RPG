using System;
// using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectEmbersteel
{
    /// <summary>
    /// Stores the game data that will be saved and loaded.
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        public Guid guid;
        public string Name;
        public string CurrentLevelName;
    }
}