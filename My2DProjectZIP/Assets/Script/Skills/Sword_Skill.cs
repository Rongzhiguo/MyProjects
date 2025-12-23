using System;
using UnityEngine;


public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin,
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce  配置")]
    [SerializeField] private UI_SkillTreeSlot bounceUnlockButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceSpeed;
    [SerializeField] private float bounceGravity;

    [Header("Pierce 配置")]
    [SerializeField] private UI_SkillTreeSlot pierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin 配置")]
    [SerializeField] private UI_SkillTreeSlot spinUnlockButton;
    [SerializeField] private float hitCooldown;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;

    [Header("Regular 投掷剑相关技能配置")]
    [SerializeField] private UI_SkillTreeSlot sowrdUnlockButton;
    public bool sowrdUnlocked { get; private set; }
    [SerializeField] private GameObject sowrdPrefab;
    [SerializeField] private Vector2 luanchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float retrunSpeed;

    [Header("被动技能")]
    [SerializeField][Tooltip("时间静止技能按钮")] private UI_SkillTreeSlot timeStopUnlockButton;
    public bool timeStopUnlocked { get; private set; }

    [SerializeField][Tooltip("弱化技能按钮")] private UI_SkillTreeSlot vulnurableUnlockButton;
    public bool vulnurableUnlocked { get; private set; }

    private Vector2 finalDir;

    [Header("发射弹道轨迹数据")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBeetwenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;
    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();
        CreatDots();

        SetupGravity();


        bounceUnlockButton.onSkillUnlocked += UnlockBounceSword;
        pierceUnlockButton.onSkillUnlocked += UnlockPierceSword;
        spinUnlockButton.onSkillUnlocked += UnlockSpinSword;
        sowrdUnlockButton.onSkillUnlocked += UnlockSowrd;
        timeStopUnlockButton.onSkillUnlocked += UnlockTimeStop;
        vulnurableUnlockButton.onSkillUnlocked += UnlockVulnurable;
    }

    protected override void CheckUnlock()
    {
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
        UnlockSowrd();
        UnlockTimeStop();
        UnlockVulnurable();
    }
    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().normalized.x * luanchForce.x, AimDirection().normalized.y * luanchForce.y);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBeetwenDots);
            }
        }

    }

    public void CreatSword()
    {
        GameObject newSword = Instantiate(sowrdPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        //需要设置剑的类型和相关属性参数并传入其中
        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, spinDuration, maxTravelDistance, hitCooldown);

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, retrunSpeed);
        DotsActive(false);
        player.AssingNewSwrod(newSword);
    }

    #region 解锁区域
    private void UnlockTimeStop()
    {
        if (timeStopUnlockButton.unlocked)
            timeStopUnlocked = true;
    }

    private void UnlockVulnurable()
    {
        if (vulnurableUnlockButton.unlocked)
            vulnurableUnlocked = true;
    }

    private void UnlockSowrd()
    {
        if (sowrdUnlockButton.unlocked)
        {
            sowrdUnlocked = true;
            swordType = SwordType.Regular;
        }
    }

    private void UnlockBounceSword()
    {
        if (bounceUnlockButton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if (pierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (spinUnlockButton.unlocked)
            swordType = SwordType.Spin;
    }

    #endregion

    #region Aim region(瞄准相关方法)
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mousePosition - playerPosition);
    }

    public void DotsActive(bool isActive)
    {
        foreach (var dot in dots)
        {
            dot.SetActive(isActive);
        }
    }

    private void CreatDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position +
            new Vector2(AimDirection().normalized.x * luanchForce.x, AimDirection().normalized.y * luanchForce.y) * t +
            .5f * (Vector2)((Physics2D.gravity * swordGravity) * (t * t));
        return position;
    }
    #endregion
}
