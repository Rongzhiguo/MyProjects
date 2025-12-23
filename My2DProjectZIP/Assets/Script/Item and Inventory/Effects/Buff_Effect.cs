using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffStatAmount
{
    [SerializeField][Tooltip("需要增加的属性类型")] private StatType buffTypes;
    [SerializeField][Tooltip("需要增加的属性值")] private int buffAmount;

    public StatType GetStatType() => buffTypes;
    public int GetBuffAmount() => buffAmount;
}

[CreateAssetMenu(fileName = "新BUFF效果", menuName = "数据/物品效果/BUFF效果")]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    //[SerializeField][Tooltip("需要增加的属性类型组")] private StatType[] buffTypes;
    //[SerializeField][Tooltip("需要增加的属性值")] private int buffAmount;
    [SerializeField][Tooltip("需要增加的属性")] private BuffStatAmount[] buffStatAmounts;
    [SerializeField][Tooltip("BUFF持续时间")] private float buffDuration;


    public override void ExecuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();
        for (int i = 0; i < buffStatAmounts.Length; i++)
        {
            stats.IncreaseStatBy(buffStatAmounts[i].GetBuffAmount(), buffDuration, stats.GetStat(buffStatAmounts[i].GetStatType()));
        }
    }

}
