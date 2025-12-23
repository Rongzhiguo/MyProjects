using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public float cooldown;
    /// <summary>
    /// 技能的CD，大于0则不能使用技能
    /// </summary>
    public float cooldownTimer;

    protected player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
        StartCoroutine(StartAfterAllStart());
    }

    IEnumerator StartAfterAllStart()
    {
        // 等待一帧，这样所有脚本的Start都已经执行完毕
        yield return null;
        // 这里可以执行你想要在所有Start之后执行的代码
        CheckUnlock();
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 检查是否解锁
    /// </summary>
    protected virtual void CheckUnlock()
    {
    
    }

    public virtual bool CanUseSkill()
    {
        if (cooldownTimer < 0)
        {
            // 使用技能
            UseSkill();
            return true;
        }

        //技能正在CD中····
        player.fx.ShowPopUpTextFx("Skill Cooldown!!");
        return false;
    }

    public virtual void UseSkill()
    {
        cooldownTimer = cooldown;
    }

    protected virtual Transform FindClosestEnemy(Transform _checkTransform, float _distance)
    {
        Transform closestEnemy = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_checkTransform.position, _distance);
        float closestDistance = Mathf.Infinity;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                float distanceToEnemy = Vector2.Distance(_checkTransform.position, hit.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }
        }
        return closestEnemy;
    }
}
