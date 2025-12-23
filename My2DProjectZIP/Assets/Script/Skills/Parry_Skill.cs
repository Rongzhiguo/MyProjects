using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry_Skill : Skill
{
    [Header("格挡")]
    [Tooltip("格挡技能按钮")][SerializeField] private UI_SkillTreeSlot parryUnlockButton;
    [Tooltip("格挡是否解锁")] public bool parryUnlocked { get; private set; }

    [Header("格挡恢复")]
    [SerializeField][Tooltip("格挡成功恢复血量百分比")][Range(0f, 1f)] private float restoreHealthAmount;
    [Tooltip("格挡恢复是否解锁")] public bool restoreUnlocked { get; private set; }
    [Tooltip("格挡恢复技能按钮")][SerializeField] private UI_SkillTreeSlot restoreUnlockButton;

    [Header("格挡成功创建分身")]
    [Tooltip("格挡成功创建分身技能按钮")][SerializeField] private UI_SkillTreeSlot parryWithMirageUnlockButton;
    [Tooltip("格挡成功创建分身是否解锁")] public bool parryWithMirageUnlocked { get; private set; }

    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.stats.GetMaxHealthValueHP() * restoreHealthAmount);
            player.stats.IncreaseHealthBy(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();
        parryUnlockButton.onSkillUnlocked += UnlockParry;
        restoreUnlockButton.onSkillUnlocked += UnlockRestore;
        parryWithMirageUnlockButton.onSkillUnlocked += UnlockparryWithMirage;
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockRestore();
        UnlockparryWithMirage();
    }

    private void UnlockParry()
    {
        if (parryUnlockButton.unlocked)
            parryUnlocked = true;
    }

    private void UnlockRestore()
    {
        if (restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }

    private void UnlockparryWithMirage()
    {
        if (parryWithMirageUnlockButton.unlocked)
            parryWithMirageUnlocked = true;
    }

    /// <summary>
    /// 格挡成功后是否可以创建分身进行攻击
    /// </summary>
    /// <param name="_enemyTransform"></param>
    public void MakeMirageOnParry(Transform _enemyTransform)
    {
        if (parryWithMirageUnlocked)
            SkillManager.instance.clone_Skill.CreateCloneWithDelay(_enemyTransform);
    }
}
