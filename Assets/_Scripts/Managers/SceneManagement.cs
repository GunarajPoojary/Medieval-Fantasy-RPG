using UnityEngine;
using UnityEngine.SceneManagement;

namespace GunarajCode
{
    public class SceneManagement : Singleton<SceneManagement>
    {
        //private void OnEnable()
        //{
        //    SceneManager.sceneLoaded += OnSceneLoaded;
        //}

        //private void OnDisable()
        //{
        //    SceneManager.sceneLoaded -= OnSceneLoaded;
        //}

        public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void LoadMainMenu() => SceneManager.LoadScene(0);

        public void LoadGameplay() => SceneManager.LoadScene(1);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                //SavePlayerData();
                if (SceneManager.GetActiveScene().buildIndex == 2)
                {
                    SceneManager.LoadSceneAsync(1);
                }
                else
                    SceneManager.LoadSceneAsync(2);
            }
        }

        //private void SavePlayerData()
        //{
        //    Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //    if (player != null)
        //    {
        //        List<ItemObjectDTO> inventoryItems = new List<ItemObjectDTO>();
        //        foreach (var item in Inventory.Instance.Items)
        //        {
        //            // Add item DTOs to inventory items list
        //            inventoryItems.Add(new ItemObjectDTO { DisplayName = item.DisplayName, Description = item.Description, Type = item.Type });
        //        }

        //        List<ArmorObjectDTO> armorObjects = new List<ArmorObjectDTO>();
        //        foreach (var armor in EquipmentManager.Instance.CurrentArmorObjects)
        //        {
        //            if (armor == null)
        //                continue;

        //            // Add armor DTOs to armor objects list
        //            armorObjects.Add(new ArmorObjectDTO { DisplayName = armor.DisplayName, ArmorEquipSlot = armor.ArmorEquipSlot.ToString(), IsDefault = armor.IsDefault });
        //        }

        //        WeaponObject weapon = EquipmentManager.Instance.CurrentWeaponObject;

        //        WeaponObjectDTO weaponDTO = null;
        //        if (weapon != null)
        //        {
        //            // Add weapon DTO
        //            weaponDTO = new WeaponObjectDTO { DisplayName = weapon.DisplayName, AnimatorOverrideName = weapon.AnimatorOverride.name, WeaponPrefabNames = new string[weapon.WeaponPrefabs.Length] };
        //            for (int i = 0; i < weapon.WeaponPrefabs.Length; i++)
        //            {
        //                weaponDTO.WeaponPrefabNames[i] = weapon.WeaponPrefabs[i].name;
        //            }
        //        }

        //        // Create player data with collected information
        //        SaveData.Instance.playerData = new PlayerData(player, inventoryItems, armorObjects, weaponDTO);
        //        SaveSystem.Save("Save", SaveData.Instance);
        //    }
        //}

        //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        //{
        //    if (scene.buildIndex == 2)
        //    {
        //        Debug.Log(1);
        //        SaveData.Instance = SaveSystem.Load<SaveData>("Save");
        //        if (SaveData.Instance != null && SaveData.Instance.playerData != null)
        //        {
        //            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //            if (player != null)
        //            {
        //                Inventory.Instance.Items.Clear();
        //                foreach (var itemDTO in SaveData.Instance.playerData.InventoryItems)
        //                {
        //                    // Instantiate items from DTOs
        //                    ItemObject item = InstantiateItem(itemDTO);
        //                    if (item != null)
        //                    {
        //                        // Add item to inventory
        //                        Inventory.Instance.Add(item);
        //                    }
        //                }

        //                // Clear current armor objects list
        //                EquipmentManager.Instance.CurrentArmorObjects = new ArmorObject[Enum.GetNames(typeof(ArmorSlot)).Length];

        //                foreach (var armorDTO in SaveData.Instance.playerData.CurrentArmorObjects)
        //                {
        //                    // Instantiate armor objects from DTOs
        //                    ArmorObject armor = InstantiateArmor(armorDTO);
        //                    if (armor != null)
        //                    {
        //                        // Add armor to current armor objects list
        //                        EquipmentManager.Instance.CurrentArmorObjects[(int)armor.ArmorEquipSlot] = armor;
        //                    }
        //                }

        //                // Instantiate weapon object from DTO
        //                WeaponObject weapon = InstantiateWeapon(SaveData.Instance.playerData.CurrentWeaponObject);
        //                if (weapon != null)
        //                {
        //                    // Set current weapon object
        //                    EquipmentManager.Instance.CurrentWeaponObject = weapon;
        //                }
        //            }
        //        }
        //    }
        //}

        //private ItemObject InstantiateItem(ItemObjectDTO itemDTO)
        //{
        //    switch (itemDTO.Type)
        //    {
        //        case ItemType.Weapon:
        //            return ScriptableObject.CreateInstance<WeaponObject>();
        //        case ItemType.Armor:
        //            return ScriptableObject.CreateInstance<ArmorObject>();
        //        default:
        //            return null;
        //    }
        //}

        //private ArmorObject InstantiateArmor(ArmorObjectDTO armorDTO)
        //{
        //    ArmorObject armor = ScriptableObject.CreateInstance<ArmorObject>();
        //    armor.DisplayName = armorDTO.DisplayName;
        //    armor.ArmorEquipSlot = (ArmorSlot)Enum.Parse(typeof(ArmorSlot), armorDTO.ArmorEquipSlot);
        //    armor.IsDefault = armorDTO.IsDefault;
        //    return armor;
        //}

        //private WeaponObject InstantiateWeapon(WeaponObjectDTO weaponDTO)
        //{
        //    if (weaponDTO == null)
        //        return null;

        //    WeaponObject weapon = ScriptableObject.CreateInstance<WeaponObject>();
        //    weapon.DisplayName = weaponDTO.DisplayName;
        //    weapon.AnimatorOverride = Resources.Load<AnimatorOverrideController>(weaponDTO.AnimatorOverrideName);
        //    weapon.WeaponPrefabs = new GameObject[weaponDTO.WeaponPrefabNames.Length];
        //    for (int i = 0; i < weapon.WeaponPrefabs.Length; i++)
        //    {
        //        weapon.WeaponPrefabs[i] = Resources.Load<GameObject>(weaponDTO.WeaponPrefabNames[i]);
        //    }
        //    return weapon;
        //}
    }
}
