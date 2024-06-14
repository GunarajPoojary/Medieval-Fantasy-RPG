using GunarajCode.ScriptableObjects;
using UnityEngine;

namespace GunarajCode.Stat
{
    public class Player : Character, IDataPersistence
    {
        private void Start()
        {
            EquipmentManager.Instance.OnEquipmentChanged += OnEquipmentChanged;
        }

        private void OnDestroy()
        {
            if (EquipmentManager.Instance != null)
            {
                EquipmentManager.Instance.OnEquipmentChanged -= OnEquipmentChanged;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                TakeDamage(10 * Time.deltaTime);
            }
        }

        private void OnEquipmentChanged(EquipmentObject newItem, EquipmentObject oldItem)
        {
            if (newItem is ArmorObject armorItem && !armorItem.IsDefault)
            {
                foreach (var stat in armorItem.EquipmentStats.StatTypeToValueMap)
                {
                    if (_baseStats.StatTypeToValueMap.ContainsKey(stat.Key))
                    {
                        _baseStats.ChangeStatValue(stat.Key, armorItem.EquipmentStats.GetStatValue(stat.Key));
                    }
                }
            }
            else if (newItem is WeaponObject weaponItem)
            {
                foreach (var stat in weaponItem.EquipmentStats.StatTypeToValueMap)
                {
                    if (_baseStats.StatTypeToValueMap.ContainsKey(stat.Key))
                    {
                        _baseStats.ChangeStatValue(stat.Key, weaponItem.EquipmentStats.GetStatValue(stat.Key));
                    }
                }
            }
        }

        public void LoadData(GameData data)
        {
            if (data.IsNewGame)
            {
                NewGameLoadData(data);
                return;
            }

            Debug.Log(2);
            CurrentHealth = data.Health;
            CurrentDefense = data.Defense;
            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }

        private void NewGameLoadData(GameData data)
        {
            Debug.Log(1);
            CurrentHealth = _maxHealth;
            CurrentDefense = _maxDefense;
            transform.position = data.Position;
            transform.rotation = data.Rotation;
            data.IsNewGame = false;
        }

        public void SaveData(GameData data)
        {
            Debug.Log(3);
            data.Health = CurrentHealth;
            data.Defense = CurrentDefense;
            data.Position = transform.position;
            data.Rotation = transform.rotation;
        }
    }
}
