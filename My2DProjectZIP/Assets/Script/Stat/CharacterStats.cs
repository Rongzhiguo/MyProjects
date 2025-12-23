using System.Collections;
using UnityEngine;

public enum StatType
{
    力量,
    敏捷,
    智力,
    体力,

    血量,
    护甲,
    闪避,
    魔抗值,

    伤害值,
    命中率,
    命中力,

    火焰伤害,
    寒冰伤害,
    雷电伤害,
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("物理攻击统计数据")]
    [Tooltip("力量")] public Stat strength;  //1点力量增加1点伤害值，并且增加1%命中伤害
    [Tooltip("敏捷")] public Stat agility;  //1点敏捷增加1点命中率，并且增加1%闪避率
    [Tooltip("智力")] public Stat intelligence;  //1点智力增加1点魔法伤害，魔法抵抗增加当前智力的3倍
    [Tooltip("体力")] public Stat vitality;    //1点体力增加5点最大血量

    [Header("防御统计数据")]
    [Tooltip("最大血量")] public Stat maxHealth;
    [Tooltip("护甲")] public Stat armor;
    [Tooltip("闪避")] public Stat evasion;
    [Tooltip("魔抗值")] public Stat magicResistance;

    [Header("攻击统计数据")]
    [Tooltip("初始伤害值")] public Stat damage;
    [Tooltip("命中率")] public Stat critChance;
    [Tooltip("命中力量值")] public Stat critPower;

    [Header("元素攻击统计数据")]
    [Tooltip("火焰伤害")] public Stat fireDamage;
    [Tooltip("寒冰伤害")] public Stat iceDamage;
    [Tooltip("雷电伤害")] public Stat lightingDamage;

    [Tooltip("是否被点燃")] public bool isIgnited;  //随着时间推移触发伤害
    [Tooltip("是否被冰冻")] public bool isChilled;  //减少20%护甲值
    [Tooltip("是否被触电")] public bool isShocked;  //减少20%闪避率

    [SerializeField][Tooltip("触发元素持续时间")] private float ailmentsDuration = 4f;
    private float ignitedTimer;  //火焰持续时间
    private float chilledTimer;  //冰冻持续时间
    private float shockedTimer;  //触电持续时间

    private float ignitedDamageCooldown = .3f;  //火焰伤害触发间隔时间
    private float ignitedDamageTimer;
    protected int ignitedDamage;

    [SerializeField] private GameObject shockPrefab;
    //[SerializeField][Tooltip("是否可以产生雷电效果概率（1-100）")] private int shockRandom;
    private int shockDamage; //闪电击中目标的伤害

    public int damageReturn { get; private set; } //最终照成的伤害值

    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool isDead { get; private set; }

    /// <summary>
    /// 是否无敌状态
    /// </summary>
    public bool isInvincible { get; private set; }  

    /// <summary>
    /// 当前血量
    /// </summary>
    public int currentHealth { get; private set; }

    public System.Action onHealthChanged;

    public bool isVulnurable;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = GetMaxHealthValueHP();
        critPower.SetDefaultValue(150);
        fx = GetComponent<EntityFX>();
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;

        ignitedDamageTimer -= Time.deltaTime;

        if (ignitedTimer <= 0)
            isIgnited = false;

        if (chilledTimer <= 0)
            isChilled = false;

        if (shockedTimer <= 0)
            isShocked = false;

        ApplyIgniteDamage();
    }

    public void MakeVulnurableFor(float _duration) => StartCoroutine(VulnurableForCoroutine(_duration));

    private IEnumerator VulnurableForCoroutine(float _duration)
    {
        isVulnurable = true;
        yield return new WaitForSeconds(_duration);
        isVulnurable = false;
    }

    /// <summary>
    /// 增加状态
    /// </summary>
    /// <param name="_modifier">需要增加的值</param>
    /// <param name="_duration">增加这个值的持续时间</param>
    /// <param name="_statToModifier">增加之前的初始值</param>
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModifier)
    {
        StartCoroutine(StatModCoroutine(_modifier, _duration, _statToModifier));
        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    /// <summary>
    /// Mod统计协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator StatModCoroutine(int _modifier, float _duration, Stat _statToModifier)
    {
        _statToModifier.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModifier.MoveModifier(_modifier);
    }


    /// <summary>
    /// 计算物理伤害方法
    /// </summary>
    /// <param name="_targetStats"></param>
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (TargetCanAvoidAttack(_targetStats)) return;
        if (_targetStats.isInvincible) return;

        _targetStats.GetComponent<Entity>().SetuoKonckbackDir(transform);

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        fx.CreatHitFX(_targetStats.transform);

        //需要计算所有伤害总和
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        //_targetStats.TakeDamage(totalDamage);

        //如果玩家背包有能够照成元素伤害的装备时，则打开魔法伤害的调用  可用if判断
        DoMagicalDamage(_targetStats, totalDamage);
    }

    #region 魔法伤害
    /// <summary>
    /// 计算魔法伤害方法
    /// </summary>
    /// <param name="_targetStats"></param>
    public virtual void DoMagicalDamage(CharacterStats _targetStats , int _totalDamage = 0)
    {
        _targetStats.GetComponent<Entity>().SetuoKonckbackDir(transform);
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();
        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        _targetStats.TakeDamage(totalMagicalDamage + _totalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
            return;

        AttemptyToApplyAilements(_targetStats, _fireDamage, _iceDamage, _lightingDamage);
    }

    private void AttemptyToApplyAilements(CharacterStats _targetStats, int _fireDamage, int _iceDamage, int _lightingDamage)
    {
        //元素伤害计算完成后，判断三种元素中哪个元素伤害最高
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lightingDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

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
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_lightingDamage * .2f));


        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;

        //火焰
        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFxFor(ailmentsDuration);
        }

        //冰寒
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;
            fx.ChillFxFor(ailmentsDuration);

            float slowPercentage = .3f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
        }

        //闪电
        if (_shock && canApplyShock)
        {
            if (!isShocked)
            {
                ApplyShoke(_shock);
            }
            else
            {
                if (GetComponent<player>() != null)
                    return;
                HitNearestTargetWithShockStrike();

            }

        }
    }

    public void ApplyShoke(bool _shock)
    {
        if (isShocked)
            return;
        isShocked = _shock;
        shockedTimer = ailmentsDuration;
        fx.ShockFxFor(ailmentsDuration);
    }

    /// <summary>
    /// 已经是雷击状态被攻击时，是否可以再次发射闪电攻击其它敌人
    /// </summary>
    private void HitNearestTargetWithShockStrike()
    {
        //if (Random.Range(0, 100) > shockRandom)
        //    return;

        Transform closestEnemy = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);
        float closestDistance = Mathf.Infinity;
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
        }

        //此处可选择是否启用，如果启用则找不到目标 雷击将劈向当前被攻击目标
        if (closestEnemy == null)
            closestEnemy = transform;

        if (closestEnemy != null)
        {
            GameObject newThunderStike = Instantiate(shockPrefab, transform.position, Quaternion.identity);
            ShockStrike_Controller newThunderStikeScript = newThunderStike.GetComponent<ShockStrike_Controller>();
            newThunderStikeScript.Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    private void ApplyIgniteDamage()
    {
        if (ignitedDamageTimer <= 0 && isIgnited)
        {
            ignitedDamageTimer = ignitedDamageCooldown;
            DecreaseHealthBy(ignitedDamage);
            if (currentHealth <= 0 && !isDead)
            {
                Die();
                isIgnited = false;
            }
        }
    }
    #endregion

    public void SetupIgniteDamage(int _damage) => ignitedDamage = _damage;

    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;

    public virtual void TakeDamage(int _damage)
    {
        if (isInvincible)
            return;
        DecreaseHealthBy(_damage);

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    /// <summary>
    /// 增加当前血量方法
    /// </summary>
    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > GetMaxHealthValueHP())
        {
            currentHealth = GetMaxHealthValueHP();
        }
        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    /// <summary>
    /// 减少当前血量方法
    /// </summary>
    /// <param name="_damage"></param>
    protected virtual void DecreaseHealthBy(int _damage)
    {
        if (_damage == 0)
            return;

        if (isVulnurable)
            _damage = Mathf.RoundToInt(_damage * 1.15f);

        ShowDamageTextPro(_damage);

        currentHealth -= _damage;

        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }

    /// <summary>
    /// 显示本次伤害数值
    /// </summary>
    /// <param name="_damage"></param>
    private void ShowDamageTextPro(int _damage)
    {
        // 计算飘字位置（目标上方）
        Vector3 textPosition = transform.position + Vector3.up * 1.6f;

        // 获取伤害文本
        var damagePool = ObjectPoolManager.instance.GetFormTypePool(PoolType.Damage);
        if (damagePool)
        {
            damagePool.transform.position = textPosition + new Vector3(
                        Random.Range(-0.8f, 0.8f),
                        0,
                        Random.Range(-0.5f, 0.5f)
                    );
            damagePool.GetComponent<DamageText>().Initialize(_damage, isVulnurable, GetDamagerColor());
        }
    }

    private Color GetDamagerColor()
    {
        if (isVulnurable)
            return Color.white;
        if (isIgnited)
            return Color.red;
        if (isChilled)
            return Color.blue;
        if (isShocked)
            return Color.yellow;
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        return color;
    }

    protected virtual void Die()
    {
        isDead = true;
    }

    /// <summary>
    /// 杀死实体方法（用于销毁对象）
    /// </summary>
    public void KillEntity()
    {
        if (!isDead)
            Die();
    }

    /// <summary>
    /// 设置无敌状态
    /// </summary>
    /// <param name="_invincible"></param>
    public void MakeInvincible(bool _invincible) => isInvincible = _invincible;

    /// <summary>
    /// 护甲削减伤害计算
    /// </summary>
    /// <param name="_targetStats">被攻击目标对象</param>
    /// <param name="totalDamage">本次攻击照成的伤害值</param>
    /// <returns></returns>
    protected int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        //冰冻则减少20%护甲
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        else
            totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 1, int.MaxValue);
        return totalDamage;
    }

    /// <summary>
    /// 计算魔法伤害值
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <param name="totalMagicalDamage"></param>
    /// <returns></returns>
    private int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 1, int.MaxValue);
        return totalMagicalDamage;
    }

    /// <summary>
    /// 闪避成功后触发的额外逻辑
    /// </summary>
    public virtual void OnEvasion()
    {

    }

    /// <summary>
    /// 本次攻击是否可以闪避
    /// </summary>
    /// <param name="_targetStats"></param>
    /// <returns></returns>
    protected bool TargetCanAvoidAttack(CharacterStats _targetStats)
    {
        //总闪避
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        //触电则增加20%闪避率
        if (isShocked)
            totalEvasion += 20;
        if (Random.Range(0, 100) < totalEvasion)
        {
            _targetStats.OnEvasion();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 本次能否触发命中伤害加成
    /// </summary>
    /// <returns></returns>
    protected bool CanCrit()
    {
        //总命中率概率
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) < totalCriticalChance)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 命中后计算额外伤害加成
    /// </summary>
    /// <param name="_damage">初始伤害值</param>
    /// <returns></returns>
    protected int CalculateCriticalDamage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        //Debug.Log("伤害加成百分比：" + totalCritPower + "%");

        float critDamage = _damage * totalCritPower;
        //Debug.Log("最终伤害值：" + critDamage);

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealthValueHP() => maxHealth.GetValue() + (vitality.GetValue() * 5);


    public Stat GetStat(StatType _buffTypes)
    {
        #region if 结构判断
        //if (_buffType == StatType.strength)
        //    return stats.strength;
        //else if (_buffType == StatType.agility)
        //    return stats.agility;
        //else if (_buffType == StatType.intelligence)
        //    return stats.intelligence;
        //else if (_buffType == StatType.vitality)
        //    return stats.vitality;
        //else if (_buffType == StatType.maxHealth)
        //    return stats.maxHealth;
        //else if (_buffType == StatType.armor)
        //    return stats.armor;
        //else if (_buffType == StatType.evasion)
        //    return stats.evasion;
        //else if (_buffType == StatType.magicResistance)
        //    return stats.magicResistance;
        //else if (_buffType == StatType.damage)
        //    return stats.damage;
        //else if (_buffType == StatType.critChance)
        //    return stats.critChance;
        //else if (_buffType == StatType.critPower)
        //    return stats.critPower;
        //else if (_buffType == StatType.fireDamage)
        //    return stats.fireDamage;
        //else if (_buffType == StatType.iceDamage)
        //    return stats.iceDamage;
        //else if (_buffType == StatType.lightingDamage)
        //    return stats.lightingDamage;
        #endregion

        switch (_buffTypes)
        {
            case StatType.力量:
                return strength;
            case StatType.敏捷:
                return agility;
            case StatType.智力:
                return intelligence;
            case StatType.体力:
                return vitality;
            case StatType.血量:
                return maxHealth;
            case StatType.护甲:
                return armor;
            case StatType.闪避:
                return evasion;
            case StatType.魔抗值:
                return magicResistance;
            case StatType.伤害值:
                return damage;
            case StatType.命中率:
                return critChance;
            case StatType.命中力:
                return critPower;
            case StatType.火焰伤害:
                return fireDamage;
            case StatType.寒冰伤害:
                return iceDamage;
            case StatType.雷电伤害:
                return lightingDamage;
            default:
                return null;
        }
    }
}
