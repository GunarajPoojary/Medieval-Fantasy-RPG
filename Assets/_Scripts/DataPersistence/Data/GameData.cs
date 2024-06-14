using System.Collections.Generic;
using UnityEngine;

namespace GunarajCode
{
    [System.Serializable]
    public class GameData
    {
        public float Health, Defense;
        public Vector3 Position;
        public Quaternion Rotation;
        public SerializableDictionary<string, bool> ItemCollected;
        public List<string> ItemIDs;  // List to store item IDs
        public bool IsNewGame;

        // Add maxHealth and maxDefense properties
        public float MaxHealth { get; set; }
        public float MaxDefense { get; set; }

        public GameData()
        {
            IsNewGame = true;
            Health = 0;
            Defense = 0;
            Position = Vector3.up;
            Rotation = Quaternion.identity;
            ItemCollected = new SerializableDictionary<string, bool>();
            ItemIDs = new List<string>();  // Initialize the list
        }
    }
}
