using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "冻结敌人效果", menuName = "数据/物品效果/冰冻敌人")]
public class FreezeEnemies_Effect : ItemEffect
{
    [SerializeField][Tooltip("持续时间")] private float duration;
    public override void ExecuteEffect(Transform _transform)
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        //血量大于10% 不会触发冰冻效果
        if (playerStats.currentHealth > playerStats.GetMaxHealthValueHP() * .1f)
            return;

        if (!Inventory.instance.CanUseArmor())
            return;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, 2);
        foreach (var hit in colliders)
        {
            //将怪物定格在此处一定时间
            hit.GetComponent<Enemy>()?.FreezeTimeFor(duration);
        }

        //可在玩家身上播放一个寒冰爆炸效果
    }
}
