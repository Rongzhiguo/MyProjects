using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;

    public Stat soulsDropAmount;

    [Header("怪物等级")]
    [SerializeField] private int Level;

    [Range(0f, 1f)]
    [SerializeField][Tooltip("攻击加成百分比浮动值")] private float percantageModifier = .4f;
    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100);
        ApplyLeverModifiers();

        base.Start();
        enemy = GetComponent<Enemy>();

        myDropSystem = GetComponent<ItemDrop>();


    }

    /// <summary>
    /// 根据怪物等级进行基础属性值百分比增长
    /// </summary>
    private void ApplyLeverModifiers()
    {
        Modifier(strength);
        Modifier(agility);
        Modifier(intelligence);
        Modifier(vitality);

        Modifier(damage);
        Modifier(critChance);
        Modifier(critPower);

        Modifier(maxHealth);
        Modifier(armor);
        Modifier(evasion);
        Modifier(magicResistance);

        Modifier(fireDamage);
        Modifier(iceDamage);
        Modifier(lightingDamage);

        Modifier(soulsDropAmount);
    }

    private void Modifier(Stat _stat)
    {
        for (int i = 1; i < Level; i++)
        {
            float modifier = _stat.GetValue() * percantageModifier;

            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        //enemy.DamageEffect();
    }

    protected override void Die()
    {
        base.Die();
        enemy.Die();
        myDropSystem.GenerateDrop();

        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        Destroy(gameObject, 4f);
    }
}
