using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("克隆数据")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [SerializeField] private float attackMultiplier;
    [Space]
    [Header("克隆者攻击")]
    [SerializeField] private UI_SkillTreeSlot clontAttackUnlockButton;
    [SerializeField][Tooltip("克隆攻击伤害百分比")] private float coloeAttackMultiplier;
    public bool canAttack { get; private set; }

    [Header("克隆体是否能产生攻击特效")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("多重克隆")]
    [SerializeField] private UI_SkillTreeSlot multipleCloneUnlockButton;
    [SerializeField] private float multCloneAttackMultiplier;
    public bool canDuplicateClone { get; private set; }  //克隆体在对怪物照成伤害时是否能触发再次克隆
    [SerializeField][Tooltip("克隆体再次克隆成功的概率（0-100）")] private float chanceToDuplicate;

    [Header("克隆水晶相关参数")]
    [SerializeField] private UI_SkillTreeSlot crystalInseadUnlockButton;
    public bool crystalInseadOfClone { get; private set; }  //是否在使用黑洞技能时候克隆水晶还是克隆自身（false克隆自身  true 克隆水晶）

    protected override void Start()
    {
        base.Start();
        clontAttackUnlockButton.onSkillUnlocked += UnlockCloneAttack;
        aggresiveCloneUnlockButton.onSkillUnlocked += UnlockAggresiveClone;
        multipleCloneUnlockButton.onSkillUnlocked += UnlockMultipleClone;
        crystalInseadUnlockButton.onSkillUnlocked += UnlockCrystalInsead;
    }

    protected override void CheckUnlock()
    {
        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultipleClone();
        UnlockCrystalInsead();
    }

    #region 技能解锁区域
    private void UnlockCloneAttack()
    {
        if (clontAttackUnlockButton.unlocked)
        {
            canAttack = true;
            attackMultiplier = coloeAttackMultiplier;
        }
    }

    private void UnlockAggresiveClone()
    {
        if (aggresiveCloneUnlockButton.unlocked)
        {
            canApplyOnHitEffect = true;
            attackMultiplier = aggresiveCloneAttackMultiplier;
        }
    }

    private void UnlockMultipleClone()
    {
        if (multipleCloneUnlockButton.unlocked)
        {
            canDuplicateClone = true;
            attackMultiplier = multCloneAttackMultiplier;
        }
    }

    private void UnlockCrystalInsead()
    {
        if (crystalInseadUnlockButton.unlocked)
            crystalInseadOfClone = true;
    }

    #endregion


    public void CreatClone(Transform _clonePosition, Vector3 _offset)
    {
        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreatCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform, 25), canDuplicateClone, chanceToDuplicate, player,attackMultiplier);
    }

    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCoroutine(_enemyTransform, new Vector3(0.5f * player.facinDir, .2f)));
    }

    private IEnumerator CloneDelayCoroutine(Transform _enemyTransform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.2f);
        CreatClone(_enemyTransform, _offset);
    }
}
