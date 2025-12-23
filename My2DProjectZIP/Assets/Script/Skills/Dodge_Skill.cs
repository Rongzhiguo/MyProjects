using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge_Skill : Skill
{
    [Header("闪避")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField][Tooltip("解锁闪避技能后增加多少闪避值")] private int evasionAmount;
    public bool dodgeUnlocked { get; private set; }

    [Header("闪避克隆")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodgButton;
    public bool dodgMirageUnlocked;

    protected override void Start()
    {
        base.Start();
        unlockDodgeButton.onSkillUnlocked += UnlockDodge;
        unlockMirageDodgButton.onSkillUnlocked += UnlockMirageDodge;
    }

    protected override void CheckUnlock()
    {
        UnlockDodge();
        UnlockMirageDodge();
    }
    private void UnlockDodge()
    {
        if (unlockDodgeButton.unlocked)
        { 
            dodgeUnlocked = true;
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
        }
    }

    private void UnlockMirageDodge()
    {
        if (unlockMirageDodgButton.unlocked)
            dodgMirageUnlocked = true;
    }

    /// <summary>
    /// 创建闪避成功后的分身
    /// </summary>
    public void CreatMirageOnDodge()
    {
        if (dodgMirageUnlocked)
            SkillManager.instance.clone_Skill.CreatClone(player.transform, new Vector3(0, 0.2f));
    }
}
