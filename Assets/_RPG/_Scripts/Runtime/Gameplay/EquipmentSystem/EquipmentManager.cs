using RPG.Core;
using RPG.Gameplay.Combat;
using RPG.Gameplay.Inventories;
using RPG.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Gameplay.EquipmentSystem
{
    public class EquipmentManager : GenericSingleton<EquipmentManager>, ISaveable
    {
        public WeaponSO EquippedWeaponSO { get; set; }
        public List<GameObject> EquippedWeaponGameobjects { get; private set; }
        public ArmorSO[] EquippedArmorsSO { get; private set; }
        public SkinnedMeshRenderer[] EquippedArmorsMesh { get; private set; }

        public event Action<WeaponSO, WeaponSO> OnWeaponChanged;
        public event Action<ArmorSO, ArmorSO> OnArmorChanged;

        private const string PLAYER_TAG = "Player";
        private SkinnedMeshRenderer _playerHeadMesh;
        private Transform _playerVisualTransform;
        private Animator _playerAnimator;
        private SkinnedMeshRenderer[] _defaultBodyMeshes = new SkinnedMeshRenderer[4];
        private RuntimeAnimatorController _defaultAnimatorController;
        private PlayerWeaponSlots _playerWeaponSlots;

        [SerializeField] private ArmorSO[] _defaultSkinsSO = new ArmorSO[4];
        [SerializeField] private KeyCode _unequipAllKey = KeyCode.U;

        protected override void Awake()
        {
            base.Awake();
            InitializeEquipmentSlots();
        }

        private void OnEnable()
        {
            if (GameManager.Instance.IsNewGame)
            {
                PlayerInitialization(2);
                InitializeDefaultSkins();
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded; // Don't use OnDisable 

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 2)
                PlayerInitialization(1);
            else if (scene.buildIndex == 1)
                PlayerInitialization(2);
            else
                return;

            RemovePlayerDefaultSkin();

            if (EquippedWeaponSO != null)
                EquipWeapon(EquippedWeaponSO);

            if (EquippedArmorsSO != null)
            {
                for (int i = 0; i < EquippedArmorsSO.Length; i++)
                {
                    if (EquippedArmorsSO[i] != null)
                        EquipArmor(EquippedArmorsSO[i]);
                }
            }
        }

        private void PlayerInitialization(int weaponSlotIndex)
        {
            GameObject player = GameObject.FindWithTag(PLAYER_TAG);
            _playerVisualTransform = player.transform.GetChild(0).transform;
            _playerAnimator = _playerVisualTransform.GetComponent<Animator>();
            _playerHeadMesh = _playerVisualTransform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
            _playerWeaponSlots = player.transform.GetChild(weaponSlotIndex).GetComponent<PlayerWeaponSlots>();
        }

        private void RemovePlayerDefaultSkin()
        {
            for (int i = 2; i < _playerVisualTransform.childCount; i++)
                _defaultBodyMeshes[i - 2] = _playerVisualTransform.GetChild(i).GetComponent<SkinnedMeshRenderer>();

            if (_defaultBodyMeshes != null)
            {
                foreach (var mesh in _defaultBodyMeshes)
                    Destroy(mesh.gameObject);
            }
        }

        private void InitializeDefaultSkins()
        {
            for (int i = 2; i < _playerVisualTransform.childCount; i++)
                _defaultBodyMeshes[i - 2] = _playerVisualTransform.GetChild(i).GetComponent<SkinnedMeshRenderer>();

            int iteration = 0;

            for (int i = 0; i < EquippedArmorsSO.Length; i++)
            {
                if (i == 0 || i == 2)
                    continue;

                EquippedArmorsSO[i] = _defaultSkinsSO[iteration];
                EquippedArmorsMesh[i] = _defaultBodyMeshes[iteration];
                iteration++;
            }
        }

        private void Start() => _defaultAnimatorController = _playerAnimator.runtimeAnimatorController;

        private void InitializeEquipmentSlots()
        {
            int numSlots = Enum.GetNames(typeof(ArmorSlot)).Length;
            EquippedArmorsSO = new ArmorSO[numSlots];
            EquippedArmorsMesh = new SkinnedMeshRenderer[numSlots];
        }

        private void Update()
        {
            if (Input.GetKeyDown(_unequipAllKey))
                UnequipAll();
        }

        public void Equip(EquipmentSO item)
        {
            if (item is ArmorSO armor)
                EquipArmor(armor);
            else if (item is WeaponSO weapon)
                EquipWeapon(weapon);
        }

        private void EquipArmor(ArmorSO newItem)
        {
            if (newItem == null)
            {
                Debug.LogError("EquipArmor called with null ArmorObject.");
                return;
            }

            int slotIndex = (int)newItem.ArmorEquipSlot;
            ArmorSO oldItem = UnequipArmor(slotIndex);

            if (EquippedArmorsSO[slotIndex] != null && oldItem != null && !oldItem.IsDefault)
                Inventory.Instance.AddItem(oldItem);

            EquippedArmorsSO[slotIndex] = newItem;
            var newMesh = Instantiate(newItem.SkinnedMesh, _playerVisualTransform);
            newMesh.bones = _playerHeadMesh.bones;
            newMesh.rootBone = _playerHeadMesh.rootBone;
            EquippedArmorsMesh[slotIndex] = newMesh;

            OnArmorChanged?.Invoke(newItem, oldItem);
        }

        private void EquipWeapon(WeaponSO newItem)
        {
            WeaponSO oldItem = UnequipWeapon();

            if (EquippedWeaponSO != null && oldItem != null)
                Inventory.Instance.AddItem(oldItem);

            EquippedWeaponSO = newItem;
            EquipWeaponPrefabs(newItem);
            UpdateAnimatorController(newItem);

            OnWeaponChanged?.Invoke(newItem, oldItem);
        }

        private void EquipWeaponPrefabs(WeaponSO newItem)
        {
            EquippedWeaponGameobjects = new List<GameObject>();

            foreach (var gameObject in newItem.WeaponPrefabs)
            {
                if (_playerWeaponSlots.WeaponToHandTransformMap.TryGetValue(gameObject, out var weaponHandTransform))
                {
                    var newWeapon = Instantiate(gameObject, weaponHandTransform);
                    newWeapon.transform.localPosition = Vector3.zero;
                    newWeapon.transform.localRotation = Quaternion.identity;
                    EquippedWeaponGameobjects.Add(newWeapon);
                }
            }
        }

        private void UpdateAnimatorController(WeaponSO newItem)
        {
            var overrideController = _playerAnimator.runtimeAnimatorController as AnimatorOverrideController;
            _playerAnimator.runtimeAnimatorController = newItem.AnimatorOverride != null ? newItem.AnimatorOverride : _defaultAnimatorController;
        }

        public ArmorSO UnequipArmor(int slotIndex)
        {
            if (EquippedArmorsSO[slotIndex] == null)
                return null;

            var oldItem = EquippedArmorsSO[slotIndex];
            if (EquippedArmorsMesh[slotIndex] != null)
                Destroy(EquippedArmorsMesh[slotIndex].gameObject);

            EquippedArmorsSO[slotIndex] = null;

            //OnArmorChanged?.Invoke(null, oldItem);
            return oldItem;
        }

        public WeaponSO UnequipWeapon()
        {
            if (EquippedWeaponSO == null)
                return null;

            if (EquippedWeaponGameobjects != null)
            {
                foreach (var weapon in EquippedWeaponGameobjects)
                {
                    if (weapon != null)
                        Destroy(weapon);
                }
            }

            var oldItem = EquippedWeaponSO;

            EquippedWeaponSO = null;
            //OnWeaponChanged?.Invoke(null, oldItem);
            return oldItem;
        }

        public void UnequipAll()
        {
            if (EquippedArmorsSO != null)
            {
                for (int i = 0; i < EquippedArmorsSO.Length; i++)
                    UnequipArmor(i);
            }

            UnequipWeapon();

            EquipDefaultSkins();

            _playerAnimator.runtimeAnimatorController = _defaultAnimatorController;
        }

        private void EquipDefaultSkins()
        {
            foreach (var skin in _defaultSkinsSO)
                EquipArmor(skin);
        }

        public void LoadData(GameData data)
        {
            if (ItemDataBase.Instance.GetItemByID(data.CurrentWeaponObjectIDs) is WeaponSO weapon)
                EquippedWeaponSO = weapon;

            for (int i = 0; i < data.CurrentArmorObjectIDs.Length; i++)
            {
                if (ItemDataBase.Instance.GetItemByID(data.CurrentArmorObjectIDs[i]) is ArmorSO armor)
                    EquippedArmorsSO[i] = armor;
            }
        }

        public void SaveData(GameData data)
        {
            // Save equipped weapons
            if (EquippedWeaponSO != null)
                data.CurrentWeaponObjectIDs = EquippedWeaponSO.ID;

            data.CurrentArmorObjectIDs = new string[EquippedArmorsSO.Length];
            for (int i = 0; i < EquippedArmorsSO.Length; i++)
            {
                if (EquippedArmorsSO[i] != null)
                    data.CurrentArmorObjectIDs[i] = EquippedArmorsSO[i].ID;
            }
        }
    }
}
