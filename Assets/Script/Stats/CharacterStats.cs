using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    strength,
    agility,
    intelegence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    energy,
    experience,
    armor,
    evasion,
    magicRes,
    fireDamage,
    iceDamage,
    lightingDamage
}
public class CharacterStats : MonoBehaviour
{
    //private EntityFX fx;

    [Header("Major stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stat agility;  // 1 point increase evasion by 1% and crit.chance by 1%
    public Stat intelligence; // 1 point increase magic damage by 1 and magic resistance by 3
    public Stat vitality; // 1 point incredase health by 5 points

    [Header("Offensive stats")]
    public Stat damage;
    public Stat critChance;// co hoi chi mang
    public Stat critPower; // suc manh chi mang             // default value 150%

    [Header("Defensive stats")]
    public Stat maxHealth;
    public Stat maxEnergy;
    public Stat maxExperience;
    public Stat armor;// giap cua nhan vat
    public Stat evasion; // evasion tỉ lệ né tránh  => khoang 150%
    public Stat magicResistance;

    // sat thuong phep thuat
    [Header("Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    [Header("Energy")]
    public float energyRegenInterval = 5.0f; // Thời gian giữa các lần hồi năng lượng
    public int energyRegenAmount = 10; // Lượng năng lượng được hồi mỗi lần

    [Header("Experience")]
    public int increaseEXP;
    public int levelUP = 1;

    private Coroutine regenCoroutine;


    public bool isIgnited;  // bi dot chay gau sat thuong theo thoi gian
    public bool isChilled;   // bi lam lanh giao 20% giap
    public bool isShocked;   // bi shock giam do chinh xac 20 %


    [SerializeField] private float ailmentsDuration = 4;// thoi gian FX 
    private float ignitedTimer;// thoi gian bi dot chay
    private float chilledTimer;//thoi gian bi lam lanh
    private float shockedTimer;// thoi gian bị shock


    private float igniteDamageCoodlown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    [SerializeField] private GameObject shockStrikePrefab;
    private int shockDamage;
    public int currentHealth;
    public int currentEnergy;
    public int currentEXP;

    public System.Action onHealthChanged;

    public System.Action onEnergyChanged;

    public System.Action onEXPChanged;
    public bool isDead { get; private set; }
    public bool isInvincible { get; private set; }
    private bool isVulnerable;

    protected virtual void Start()
    {
        // khoi tao suc manh chi mang mac dinh la 150
        critPower.SetDefaultValue(150);
        currentHealth = GetMaxHealthValue();
        currentEnergy = GetMaxEnergyValue();
        currentEXP = GetMaxEXPValue();

       // fx = GetComponent<EntityFX>();
        StartEnergyRegen();
    }


    public void StartEnergyRegen()
    {
        // Kiểm tra xem coroutine có đang chạy không, nếu không thì bắt đầu coroutine mới
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenEnergy());
        }
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        igniteDamageTimer -= Time.deltaTime;


        if (ignitedTimer < 0)
            isIgnited = false;

        if (chilledTimer < 0)
            isChilled = false;

        if (shockedTimer < 0)
            isShocked = false;

        if (isIgnited)
            ApplyIgniteDamage();
    }


    //MakeVulnerableFor gay dam 
    public void MakeVulnerableFor(float _duration) => StartCoroutine(VulnerableCorutine(_duration));

    private IEnumerator VulnerableCorutine(float _duartion)
    {
        isVulnerable = true;

        yield return new WaitForSeconds(_duartion);

        isVulnerable = false;
    }

    // IncreaseStatBy tang chi so 
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        // start corototuine for stat increase
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);

        yield return new WaitForSeconds(_duration);

        _statToModify.RemoveModifier(_modifier);
    }


    public virtual void DoDamage(CharacterStats _targetStats)
    {
        bool criticalStrike = false;


        if (_targetStats.isInvincible)
            return;

        if (TargetCanAvoidAttack(_targetStats))
            return;

       // _targetStats.GetComponent<Entity>().SetupKnockbackDir(transform);
        // totalDamage tong thiet hai
        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            criticalStrike = true;
        }

       // fx.CreateHitFx(_targetStats.transform, criticalStrike);

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);


        // neu vu khi trong trang bij co ki nang lua thi dung ham nay
        DoMagicalDamage(_targetStats);

    }

    // ham gay satv thuong phep thuat
    #region Magical damage and ailemnts

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        // gan sat thuong cho tung loai sat thuong phep thuat
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();


        // tong sat thuong
        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();

        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);
        _targetStats.TakeDamage(totalMagicalDamage);


        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;


        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);

    }

    // AttemptyToApplyAilements ham tong hop  chap nhan sat thuong 
    private void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;
        // neu không thể  ap dung 1 trong 3 sẽ radom co gang ap dung 1 trong 3 
        while (!canApplyIgnite && !canApplyChill && !canApplyShock)
        {
            if (Random.value < .3f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);

                return;
            }

            if (Random.value < .5f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);

                return;
            }

            if (Random.value < .5f && _lightingDamage > 0)
            {
                canApplyShock = true;

                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                return;

            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));

        if (canApplyShock)
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .1f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }


    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {

        // tao bien dam bao doi tuong bi tan cong chua chiu bat khi tac dong nao tu 1 trong 3 tac dong
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;


        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;

     //       fx.IgniteFxFor(ailmentsDuration);
        }

        /*
        if (_chill && canApplyChill)
        {
            chilledTimer = ailmentsDuration;
            isChilled = _chill;

            float slowPercentage = .2f;

            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
     //       fx.ChillFxFor(ailmentsDuration);
        }

        */
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShock(_shock);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike();
            }
        }

    }

    public void ApplyShock(bool _shock)
    {
        if (isShocked)
            return;

        shockedTimer = ailmentsDuration;
        isShocked = _shock;

   //     fx.ShockFxFor(ailmentsDuration);
    }

    private void HitNearestTargetWithShockStrike()
    {
        // _checkTransfrom.position tìm kiếm tất và lưu tất  cả các vào  colliders có trong phạm vi bán kính là 25 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        // closestDistance biến nhận giá trị vô hạn (nhận và lưu trữ khoảng cách gần nhất  từ Player  -> enemy) 
        // Mathf.Infinity;phạm vi cộng vô hạn
        float closestDistance = Mathf.Infinity;
        // closestEnemy lưu trữ quái vật gần nhất
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
            // neu ko có mục tiêu ở gần tự đánh vào chính mk
            if (closestEnemy == null)
                closestEnemy = transform;
        }

/*
        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrike_Controller>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
*/
    }

    // dam va thoi gian chiu tac dong
    private void ApplyIgniteDamage()
    {
        if (igniteDamageTimer < 0)
        {
            DecreaseHealthBy(igniteDamage);

            if (currentHealth < 0 && !isDead)
                Die();

            igniteDamageTimer = igniteDamageCoodlown;
        }
    }
    // dam khi bi tac dong 
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;
    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    #endregion  

    // TakeDamagetao ham gay dam
    public virtual void TakeDamage(int _damage)
    {

        //isInvincible khar năng bất tử => khong làm gì 
        if (isInvincible)
            return;

        // giảm máu 
        DecreaseHealthBy(_damage);

    //    GetComponent<Entity>().DamageImpact();
     //   fx.StartCoroutine("FlashFX");

        if (currentHealth < 0 && !isDead)
            Die();
    }

    public virtual void IncreaseEXP()
    {
        increaseEXP += 10;

        if (increaseEXP >= currentEXP)
        {
            increaseEXP = 0;
            levelUP += 1;
        }

        if (onEXPChanged != null)
            onEXPChanged();
    }

    public int GetMaxEXPValue()
    {
        return maxExperience.GetValue();
    }


    #region ENERGY
    public virtual void EnergyConsumption(int _totalEnergy)
    {
        reduceEnergy(_totalEnergy);
    }

    // Giam nang luong 
    public virtual void reduceEnergy(int _energy)
    {
        DecreaseEnergyBy(_energy);

    }
    // dung dem 
    public void StopEnergyRegen()
    {
        // Kiểm tra xem coroutine có đang chạy không, nếu có thì dừng coroutine
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
    }


    // RegenEnergy ham dem thoi gian hoi phuc energy
    private IEnumerator RegenEnergy()
    {
        while (true)
        {
            yield return new WaitForSeconds(energyRegenInterval);
            IncreaseEnergyBy();
        }
    }


    // tao ham hoi phuc lai MP

    public virtual void IncreaseEnergyBy()
    {
        currentEnergy += 10;

        if (currentEnergy > GetMaxEnergyValue())
            currentEnergy = GetMaxEnergyValue();

        if (onEnergyChanged != null)
            onEnergyChanged();
    }
    //DecreaseEnergyBy ham giam noi nang
    protected virtual void DecreaseEnergyBy(int _energy)
    {
        currentEnergy -= _energy;
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
      //      fx.CreatePopUpText("not enought energy ");
            return;
        }
        if (onEnergyChanged != null)
        {
            onEnergyChanged();
        }
    }
    public int GetMaxEnergyValue()
    {
        return maxEnergy.GetValue();
    }

    #endregion


    #region HEALTH
    // tao ham hoi phuc lai hp
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;

        if (currentHealth > GetMaxHealthValue())
            currentHealth = GetMaxHealthValue();

        if (onHealthChanged != null)
            onHealthChanged();
    }



    // DecreaseHealthBy tao ham giam suc khỏe 
    protected virtual void DecreaseHealthBy(int _damage)
    {

        if (isVulnerable)
            _damage = Mathf.RoundToInt(_damage * 1.1f);

        currentHealth -= _damage;

        if (_damage > 0)
  //          fx.CreatePopUpText(_damage.ToString());

        if (onHealthChanged != null)
            onHealthChanged();
    }


    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    #endregion



    protected virtual void Die()
    {
        isDead = true;
    }

    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    // isInvincible ham bat tu 
    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;


    #region Stat calculations
    // CheckTargetArmor giap 
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        // totalDamage = totalDamage- armor(tong so giap )
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();


        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }


    // CheckTargetResistance muc khang cu
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }

    public virtual void OnEvasion()
    {

    }

    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }

        return false;
    }


    // ham tao sat thuong chi mang
    protected bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }


        return false;
    }

    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }



    #endregion
    #region Stats
    public Stat GetStat(StatType _statType)
    {
        if (_statType == StatType.strength) return strength;
        else if (_statType == StatType.agility) return agility;
        else if (_statType == StatType.intelegence) return intelligence;
        else if (_statType == StatType.vitality) return vitality;
        else if (_statType == StatType.damage) return damage;
        else if (_statType == StatType.critChance) return critChance;
        else if (_statType == StatType.critPower) return critPower;
        else if (_statType == StatType.health) return maxHealth;
        else if (_statType == StatType.energy) return maxEnergy;
        else if (_statType == StatType.experience) return maxExperience;
        else if (_statType == StatType.armor) return armor;
        else if (_statType == StatType.evasion) return evasion;
        else if (_statType == StatType.magicRes) return magicResistance;
        else if (_statType == StatType.fireDamage) return fireDamage;
        else if (_statType == StatType.iceDamage) return iceDamage;
        else if (_statType == StatType.lightingDamage) return lightingDamage;

        return null;
    }
    #endregion
}
