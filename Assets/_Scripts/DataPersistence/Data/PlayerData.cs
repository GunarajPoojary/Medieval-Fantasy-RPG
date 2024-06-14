using UnityEngine;

namespace GunarajCode
{
    [System.Serializable]
    public class PlayerData
    {
        public float Health, Defense;
        public Vector3 Position;
        public Quaternion Rotation;

        public PlayerData()
        {
            Health = 0;
            Defense = 0;
        }
    }
}
