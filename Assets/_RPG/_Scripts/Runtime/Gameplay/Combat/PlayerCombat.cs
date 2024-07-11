using RPG.Gameplay.EquipmentSystem;
using RPG.Gameplay.Inventories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Gameplay.Combat
{
    public class PlayerCombat : MonoBehaviour
    {
        private Animator _animator;
        private List<DamageDealer> _damageDealer;

        public float attackRate = 2f;
        private float nextAttackTime = 0f;

        private void Awake() => _animator = GetComponent<Animator>();

        private void Start() => EquipmentManager.Instance.OnWeaponChanged += OnEquipmentChanged;

        private void OnDestroy() => EquipmentManager.Instance.OnWeaponChanged -= OnEquipmentChanged;

        void Update()
        {
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    // Play an attack animation
                    _animator.SetTrigger("Attack");
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        }

        private void OnEquipmentChanged(WeaponSO newWeapon, WeaponSO oldWeapon)
        {
            if (newWeapon is WeaponSO)
            {
                _damageDealer = new List<DamageDealer>();
                _damageDealer = GetComponentsInChildren<DamageDealer>().ToList();
            }
            else
            {
                _damageDealer = null;
            }
        }

        public void StartDealingDamage() => _damageDealer?.ForEach(X => X.ApplyDamage());

        public void StopDealingDamage() => _damageDealer?.ForEach(X => X.EndDamage());
    }
}
