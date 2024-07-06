using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    //private ItemDrop myDropSystem;
    //soulsDropAmount so tin co the roi 
    public Stat soulsDropAmount;

    [Header("Level details")]
    [SerializeField] private int level = 1;


    // percantageModifier co the sua doi 
    [Range(0f, 1f)]
    [SerializeField] private float percantageModifier = .4f;

    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100);
        ApplyLevelModifiers();

        base.Start();

        enemy = GetComponent<Enemy>();
       // myDropSystem = GetComponent<ItemDrop>();
    }

    // ApplyLevelModifiers tao ham chap nhan sua doi khi enemy len lv 
    private void ApplyLevelModifiers()
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

        Modify(soulsDropAmount);
    }

    private void Modify(Stat _stat)
    {
        // duyet mang va tang theo mang 
        for (int i = 1; i < level; i++)
        {
            // lay gia tri tang khi enemy len lv
            float modifier = _stat.GetValue() * percantageModifier;
            //them lai vao chi so cua enemy va lam tron
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        //    IncreaseEXP();
        //    Debug.Log("Increate : " + increaseEXP);
        //   Debug.Log("CurrentEXP : " + currentEXP);
        //   Debug.Log("Level up : " + levelUP);
    }

    protected override void Die()
    {
        base.Die();

        //myDropSystem.GenerateDrop();


        //enemy.Die();

        PlayerManager.instance.currency += soulsDropAmount.GetValue();


        Destroy(gameObject, 5f);
    }
}
