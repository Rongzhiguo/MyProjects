using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum EquipmentType
{
    Weapon,//武器
    Armor,//盔甲
    Amulet,//项链（护身符）
    Flask,//燃烧瓶
}

[CreateAssetMenu(fileName = "新装备数据", menuName = "数据/装备")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;

    [Tooltip("装备使用冷却")] public float itemCooldown;
    [Tooltip("装备效果")] public ItemEffect[] itemEffects;

    [Tooltip("力量")] public int strength;  //1点力量增加1点伤害值，并且增加1%命中伤害
    [Tooltip("敏捷")] public int agility;  //1点敏捷增加1点命中率，并且增加1%闪避率
    [Tooltip("智力")] public int intelligence;  //1点智力增加1点魔法伤害，魔法抵抗增加当前智力的3倍
    [Tooltip("体力")] public int vitality;    //1点体力增加5点最大血量

    [Header("攻击统计数据")]
    [Tooltip("初始伤害值")] public int damage;
    [Tooltip("命中率")] public int critChance;
    [Tooltip("命中力量值")] public int critPower;

    [Header("防御统计数据")]
    [Tooltip("血量")] public int Health;
    [Tooltip("护甲")] public int armor;
    [Tooltip("闪避")] public int evasion;
    [Tooltip("魔抗值")] public int magicResistance;

    [Header("元素攻击统计数据")]
    [Tooltip("火焰伤害")] public int fireDamage;
    [Tooltip("寒冰伤害")] public int iceDamage;
    [Tooltip("雷电伤害")] public int lightingDamage;

    [Header("合成途径数据")]
    public List<InventoryItem> craftingMaterials;  //合成所需材料列表

    private int minDescLength; //最小描述行数

    /// <summary>
    /// 装备可执行的效果
    /// </summary>
    public void Effect(Transform _enemyPosition)
    {
        foreach (var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critPower.AddModifier(critPower);

        playerStats.maxHealth.AddModifier(Health);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.lightingDamage.AddModifier(lightingDamage);

        UpDateHealth(playerStats);
    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        playerStats.strength.MoveModifier(strength);
        playerStats.agility.MoveModifier(agility);
        playerStats.intelligence.MoveModifier(intelligence);
        playerStats.vitality.MoveModifier(vitality);

        playerStats.damage.MoveModifier(damage);
        playerStats.critChance.MoveModifier(critChance);
        playerStats.critPower.MoveModifier(critPower);

        playerStats.maxHealth.MoveModifier(Health);
        playerStats.armor.MoveModifier(armor);
        playerStats.evasion.MoveModifier(evasion);
        playerStats.magicResistance.MoveModifier(magicResistance);

        playerStats.fireDamage.MoveModifier(fireDamage);
        playerStats.iceDamage.MoveModifier(iceDamage);
        playerStats.lightingDamage.MoveModifier(lightingDamage);

        UpDateHealth(playerStats);
    }

    /// <summary>
    /// 更新头顶血条方法
    /// </summary>
    /// <param name="playerStats"></param>
    private void UpDateHealth(PlayerStats playerStats)
    {
        if (playerStats.onHealthChanged != null)
        {
            playerStats.onHealthChanged();
        }
    }

    public override string GetDesc()
    {
        sb.Length = 0;
        minDescLength = 0;
        AddItemDesc(strength, "力量");
        AddItemDesc(agility, "敏捷");
        AddItemDesc(intelligence, "智力");
        AddItemDesc(vitality, "体力");

        AddItemDesc(damage, "伤害");
        AddItemDesc(critChance, "命中率");
        AddItemDesc(critPower, "命中力");

        AddItemDesc(Health, "血量");
        AddItemDesc(armor, "护甲");
        AddItemDesc(evasion, "闪避");
        AddItemDesc(magicResistance, "魔抗");

        AddItemDesc(fireDamage, "火焰伤害");
        AddItemDesc(iceDamage, "寒冰伤害");
        AddItemDesc(lightingDamage, "雷电伤害");

        if (minDescLength < 4)
        {
            for (int i = 0; i < 4 - minDescLength; i++)
            {
                sb.AppendLine();
                sb.Append("");
            }
        }

        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (itemEffects[i].effectDesc.Length > 0)
            {
                sb.AppendLine();
                sb.Append("装备效果："+itemEffects[i].effectDesc);
            }
        }
        

        return sb.ToString();
    }

    private void AddItemDesc(int value, string name)
    {
        if (value != 0)
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }
            if (value > 0)
            {
                sb.Append(name + " : " + value);
            }

            minDescLength++;
        }
    }
}
