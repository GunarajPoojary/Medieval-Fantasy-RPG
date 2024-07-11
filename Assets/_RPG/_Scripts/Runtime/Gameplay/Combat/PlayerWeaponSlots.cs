using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Gameplay.Combat
{
    public class PlayerWeaponSlots : MonoBehaviour
    {
        [Serializable]
        public class WeaponHandTransform
        {
            public GameObject WeaponPrefab;
            public Transform HandTransform;
        }

        [SerializeField] private List<WeaponHandTransform> _weaponsHandTransforms = new List<WeaponHandTransform>();
        public Dictionary<GameObject, Transform> WeaponToHandTransformMap = new Dictionary<GameObject, Transform>();

        private void InitializeWeaponHandTransformMap() => _weaponsHandTransforms.ForEach(x => WeaponToHandTransformMap[x.WeaponPrefab] = x.HandTransform);

        private void Awake()
        {
            InitializeWeaponHandTransformMap();
        }
    }
}
