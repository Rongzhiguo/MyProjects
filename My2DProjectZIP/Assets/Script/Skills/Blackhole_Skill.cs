using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill : Skill
{
    [Space]
    [SerializeField] private UI_SkillTreeSlot blackHoleUnlockButton;
    public bool blackHoleUnlocked { get; private set; }
    [SerializeField] private int amountOfAttacks = 4;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float blakeholeDuration;
    [Space]
    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinksSpeed;

    Blackhole_Skill_Controller currentBlackhole;

    private void UnlockBlackHole()
    {
        if (blackHoleUnlockButton.unlocked)
            blackHoleUnlocked = true;
    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();

        GameObject newblackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);
        currentBlackhole = newblackHole.GetComponent<Blackhole_Skill_Controller>();
        currentBlackhole.SetupBlackhole(maxSize, growSpeed, shrinksSpeed, amountOfAttacks, attackCooldown, blakeholeDuration);
    }

    protected override void Start()
    {
        base.Start();
        blackHoleUnlockButton.onSkillUnlocked += UnlockBlackHole;
    }

    protected override void CheckUnlock()
    {
        UnlockBlackHole();
    }

    protected override void Update()
    {
        base.Update();
    }

    public bool SkillCompleted()
    {
        if (!currentBlackhole) return false;
        if (currentBlackhole.playerCanExitState)
        {
            currentBlackhole = null;
            return true;
        }
        return false;
    }

    public float GetBlackhloleRadius() => maxSize / 2;
}
