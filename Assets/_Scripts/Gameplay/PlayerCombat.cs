using GunarajCode;
using GunarajCode.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private List<DamageDealer> _damageDealer;

    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        EquipmentManager.Instance.OnEquipmentChanged += OnEquipmentChanged;
    }

    private void OnDestroy()
    {
        EquipmentManager.Instance.OnEquipmentChanged -= OnEquipmentChanged;
    }

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

    private void OnEquipmentChanged(EquipmentObject newWeapon, EquipmentObject oldWeapon)
    {
        if (newWeapon is WeaponObject)
        {
            _damageDealer = new List<DamageDealer>();
            _damageDealer = GetComponentsInChildren<DamageDealer>().ToList();
        }
        else
            _damageDealer = null;
    }

    public void StartDealingDamage()
    {
        _damageDealer?.ForEach(X => X.ApplyDamage());
    }

    public void StopDealingDamage()
    {
        _damageDealer?.ForEach(X => X.EndDamage());
    }
}
