using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.SaveLoad
{
    /// <summary>
    /// Stores the game data that will be saved and loaded.
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        // Player's position and rotation in the game world.
        public Vector3 Position;
        public Quaternion Rotation;

        public SerializedDictionary<StatType, int> PlayerStats; // Player stats like health, strength, etc.
        public SerializedDictionary<string, bool> ItemCollected; // Tracks which items have been collected.

        public List<string> ItemIDs; // IDs of Items the player possesses.
        public string CurrentWeaponObjectIDs; // ID of the currently equipped weapon.
        public string[] CurrentArmorObjectIDs; // IDs of the currently equipped armor pieces.

        public GameData()
        {
            Position = new Vector3(3f, 8f, 11f);
            Rotation = Quaternion.identity;

            PlayerStats = new SerializedDictionary<StatType, int>();
            ItemCollected = new SerializedDictionary<string, bool>();

            ItemIDs = new List<string>();
            CurrentWeaponObjectIDs = null;
            CurrentArmorObjectIDs = null;
        }
    }
}