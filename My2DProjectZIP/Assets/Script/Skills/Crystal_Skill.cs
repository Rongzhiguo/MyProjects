using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [Space]
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("水晶克隆自身")]
    [SerializeField][Tooltip("水晶克隆自身解锁按钮")] private UI_SkillTreeSlot unlockCloneInstaedButton;
    public bool cloneInsteadOfCrystal { get; private set; }

    [Header("水晶技能关联")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalBautton;
    public bool crystalUnlocked { get; private set; }

    [Header("水晶爆炸数据")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveButton;
    [SerializeField] private float explosiveCooldown;
    public bool canExplode { get; private set; }

    [Header("水晶移动数据")]
    [SerializeField] private UI_SkillTreeSlot unlockMovingCrystalButton;
    public bool canMoveToEnemy { get; private set; }
    [SerializeField] private float MoveSpeed;

    [Header("水晶可释放的数量数据")]
    [SerializeField] private UI_SkillTreeSlot unlockMultiStackButton;
    public bool canUseMultiStacks { get; private set; }
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float singleMultiCooldown;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    private float multiTimer;
    private bool isCrystalLeftFull = false;

    protected override void Start()
    {
        base.Start();
        RefilCrystal();

        unlockCrystalBautton.onSkillUnlocked += UnlockCrystal;
        unlockCloneInstaedButton.onSkillUnlocked += UnlockCrystalMirage;
        unlockExplosiveButton.onSkillUnlocked += UnlockExplosiveCrystal;
        unlockMovingCrystalButton.onSkillUnlocked += UnlockMovingCrystal;
        unlockMultiStackButton.onSkillUnlocked += UnlockMultiStack;
    }

    protected override void CheckUnlock()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplosiveCrystal();
        UnlockMovingCrystal();
        UnlockMultiStack();
    }

    #region 解锁技能区域
    private void UnlockCrystal()
    {
        if (unlockCrystalBautton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCloneInstaedButton.unlocked)
            cloneInsteadOfCrystal = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveButton.unlocked)
        { 
            canExplode = true;
            cooldown = explosiveCooldown;
        }
    }

    private void UnlockMovingCrystal()
    {
        if (unlockMovingCrystalButton.unlocked)
            canMoveToEnemy = true;
    }

    private void UnlockMultiStack()
    {
        if (unlockMultiStackButton.unlocked)
            canUseMultiStacks = true;
    }
    #endregion

    protected override void Update()
    {
        base.Update();
        SingleMultiCrystal();
    }

    /// <summary>
    /// 这个方法是单个水晶消耗完便开始进入冷却
    /// </summary>
    private void SingleMultiCrystal()
    {
        if (isCrystalLeftFull)
            multiTimer -= Time.deltaTime;

        if (crystalLeft.Count < amountOfStacks && multiTimer <= 0)
        {
            AddCrystal();
            multiTimer = singleMultiCooldown;
        }
        if (crystalLeft.Count >= amountOfStacks)
        {
            isCrystalLeftFull = false;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        if (!isCrystalLeftFull)
            multiTimer = singleMultiCooldown;

        isCrystalLeftFull = true;

        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreatCrystal();
        }
        else
        {
            if (canMoveToEnemy) return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone_Skill.CreatClone(currentCrystal.transform, new Vector3(0, 0.2f));
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>().FinishCrystal();
            }

        }
    }

    public void CreatCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller CrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        CrystalScript.SetupCryStal(crystalDuration, canExplode, canMoveToEnemy, MoveSpeed, FindClosestEnemy(currentCrystal.transform, 25));
    }

    public void CurrentCrystalChooseRandomTarget() => currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy();

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                cooldown = 0;
                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                crystalLeft.Remove(crystalToSpawn);
                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCryStal(crystalDuration, canExplode, canMoveToEnemy, MoveSpeed, FindClosestEnemy(newCrystal.transform, 25));

                //水晶列表为0 需要进入冷却填充水晶  这个方法是所有水晶消耗完再开始进入冷却
                if (crystalLeft.Count <= 0)
                {
                    cooldown = multiStackCooldown;
                    RefilCrystal();
                }
                return true;
            }
        }
        return false;
    }

    private void RefilCrystal()
    {
        for (int i = 0; i < amountOfStacks; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
        isCrystalLeftFull = false;
    }

    private void AddCrystal() => crystalLeft.Add(crystalPrefab);

}
