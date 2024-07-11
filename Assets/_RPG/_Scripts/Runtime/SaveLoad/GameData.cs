using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.SaveLoad
{
    [System.Serializable]
    public class GameData
    {
        public SerializedDictionary<Gameplay.Stats.StatType, int> PlayerStats;
        public Vector3 Position;
        public Quaternion Rotation;
        public SerializedDictionary<string, bool> ItemCollected;
        public List<string> ItemIDs;
        public string CurrentWeaponObjectIDs;
        public string[] CurrentArmorObjectIDs;

        public GameData()
        {
            PlayerStats = new();
            Position = new Vector3(3f, 8f, 11f);
            Rotation = Quaternion.identity;
            ItemCollected = new();
            ItemIDs = new List<string>();
            CurrentWeaponObjectIDs = null;
            CurrentArmorObjectIDs = null;
        }
    }
}
