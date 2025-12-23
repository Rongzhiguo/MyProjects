using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("冲刺")]
    [SerializeField][Tooltip("冲刺解锁按钮")] private UI_SkillTreeSlot dashUnlockButton;
    [Tooltip("冲刺技能是否解锁")] public bool dashUnlocked { get; private set; }

    [Header("开始冲刺克隆")]
    [SerializeField][Tooltip("开始冲刺克隆解锁按钮")] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    [Tooltip("开始冲刺克隆分身是否解锁")] public bool cloneOnDashUnlocked { get; private set; }

    [Header("结束冲刺克隆")]
    [SerializeField][Tooltip("结束冲刺克隆解锁按钮")] private UI_SkillTreeSlot cloneOnArrivalUnlockButton;
    [Tooltip("结束冲刺克隆分身是否解锁")] public bool cloneOnArrivalUnlocked { get; private set; }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    protected override void Start()
    {
        base.Start();

        dashUnlockButton.onSkillUnlocked += UnlockDash;
        cloneOnDashUnlockButton.onSkillUnlocked += UnlockCloneOnDash;
        cloneOnArrivalUnlockButton.onSkillUnlocked += UnlockCloneOnArrivalDash;
    }

    protected override void CheckUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrivalDash();
    }

    private void UnlockDash()
    {
        if (dashUnlockButton.unlocked)
        {
            dashUnlocked = true;
        }
    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
        {
            cloneOnDashUnlocked = true;
        }
    }

    private void UnlockCloneOnArrivalDash()
    {
        if (cloneOnArrivalUnlockButton.unlocked)
        {
            cloneOnArrivalUnlocked = true;
        }
    }

    /// <summary>
    /// 冲刺开始是否可以克隆
    /// </summary>
    public void CloneOnDash()
    {
        if (cloneOnDashUnlocked)
           SkillManager.instance.clone_Skill.CreatClone(player.transform, Vector3.zero);
    }

    /// <summary>
    /// 冲刺结束是否可以克隆
    /// </summary>
    public void CloneOnArrival()
    {
        if (cloneOnArrivalUnlocked)
            SkillManager.instance.clone_Skill.CreatClone(player.transform, Vector3.zero);
    }
}
