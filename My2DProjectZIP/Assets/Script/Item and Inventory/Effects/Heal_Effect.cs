using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "回血效果", menuName = "数据/物品效果/回血")]
public class Heal_Effect : ItemEffect
{
    [SerializeField][Tooltip("回血百分比")][Range(0f, 1f)] private float healPercent;
    public override void ExecuteEffect(Transform _enemyPosition)
    {
        //获取 playerStats对象
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //需要治疗多少血量
        int healAmount = Mathf.RoundToInt(playerStats.GetMaxHealthValueHP() * healPercent);

        //实际治疗值
        playerStats.IncreaseHealthBy(healAmount);
        
    }
}
