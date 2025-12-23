using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    private player player;
    private PlayerItemDrop itemDrop;
    protected override void Start()
    {
        base.Start();
        player = GetComponent<player>();
        itemDrop = GetComponent<PlayerItemDrop>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        //player.DamageEffect();
    }

    protected override void Die()
    {
        base.Die();
        player.Die();
        if (PlayerManager.instance.currency > 0)
        {
            int lostCurrency = Random.Range(1, PlayerManager.instance.currency);
            GameManager.instance.lostCurrencyAmount = lostCurrency;
            PlayerManager.instance.currency -= lostCurrency;
        }
        itemDrop.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (_damage > Mathf.RoundToInt(GetMaxHealthValueHP() * .1f))
        {
            player.fx.ScreenSkake(player.fx.shakePwoerDamage);
            Debug.Log("玩家此次受到伤害超过30%");
            player.SetupKnockBackPorer(new Vector2(10, 6));
            //可以播放一些比较短暂痛苦的声音
            //AudioManager.instance.PlaySFX(1);
        }

        //获取身上盔甲装备位上是否穿戴装备
        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);
        if (currentArmor == null)
            return;
        currentArmor.Effect(player.transform);
    }

    public override void OnEvasion()
    {
        player.skill.dodge.CreatMirageOnDodge();
    }

    /// <summary>
    /// 克隆体造成伤害计算方法
    /// </summary>
    public void CloneDoDamage(CharacterStats _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats)) return;

        int totalDamage = damage.GetValue() + strength.GetValue();
        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }

        //需要计算所有伤害总和
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);
        _targetStats.TakeDamage(totalDamage);

        //如果玩家背包有能够照成元素伤害的装备时，则打开魔法伤害的调用  可用if判断
        DoMagicalDamage(_targetStats);
    }
}
