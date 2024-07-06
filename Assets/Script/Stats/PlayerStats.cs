using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    [Header("Level details")]
    [SerializeField] private int leverl = 1;

    //  chi so co the tang
    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier = .4f;

    protected override void Start()
    {
        ApplyLevelModifies();

        base.Start();

        currentEnergy = GetMaxEnergyValue();
        player = GetComponent<Player>();
    }

    public void ApplyLevelModifies()
    {
        Modify(strength);
        Modify(agility);
        Modify(intelligence);
        Modify(vitality);

        Modify(damage);
        Modify(critChance);
        Modify(critPower);

        Modify(maxHealth);
        Modify(armor);
        Modify(evasion);
        Modify(magicResistance);

        Modify(fireDamage);
        Modify(iceDamage);
        Modify(lightingDamage);
    }



    private void Modify(Stat _stat)
    {
        for (int i = 1; i < leverl; i++)
        {
            float modifier = _stat.GetValue() * percantageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }

    protected override void Die()
    {
        base.Die();
      //  player.Die();

      //  GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

    //    GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (isDead)
            return;

        if (_damage > GetMaxHealthValue() * .3f)
        {
          //  player.SetupKnockbackPower(new Vector2(10, 6));
            // chiu sat thuong cao
          //  player.fx.ScreenShake(player.fx.shakeHighDamage);


            int randomSound = Random.Range(34, 35);
        //    AudioManager.instance.PlaySFX(randomSound, null);

        }
        /*
        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
        */
    }

    protected override void DecreaseEnergyBy(int _energy)
    {
        base.DecreaseEnergyBy(_energy);
    }

    public override void OnEvasion()
    {
      //  player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);


        DoMagicalDamage(_targetStats); // remove if you don't want to apply magic hit on primary attack
    }

}
