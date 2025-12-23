using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;

    [SerializeField] private Image healthBG;
    [SerializeField]
    [Tooltip("血条移动到最新位置所需要的时间")]
    private float buffTime;
    private float OldSliderValue;

    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackHoleImage;
    [SerializeField] private Image flaskImage;

    //右上角当前灵魂点数量
    [Header("灵魂点")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField][Tooltip("灵魂点增加率")] private float increaseRate = 1500;

    private SkillManager skills;
    // Start is called before the first frame update
    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealhtUI;
        skills = SkillManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSoulsUI();

        //冲刺
        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash_Skill.dashUnlocked)
            SetCooldown(dashImage);

        //格挡
        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldown(parryImage);

        //水晶
        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCooldown(crystalImage);

        //发射短剑
        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword_Skill.sowrdUnlocked)
            SetCooldown(swordImage);

        //黑洞
        if (Input.GetKeyDown(KeyCode.R) && skills.blackhole.blackHoleUnlocked)
            SetCooldown(blackHoleImage);

        //瓶子
        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldown(flaskImage);

        CheckCooldownOf(dashImage, skills.dash_Skill.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword_Skill.cooldown);
        CheckCooldownOf(blackHoleImage, skills.blackhole.cooldown);
        CheckCooldownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    /// <summary>
    /// 动态更新右上角货币数量
    /// </summary>
    private void UpdateSoulsUI()
    {
        if (soulsAmount < PlayerManager.instance.GetCurrentAmount())
            soulsAmount += Time.deltaTime * increaseRate;
        else
            soulsAmount = PlayerManager.instance.GetCurrentAmount();

        //currentSouls.text = PlayerManager.instance.GetCurrentAmount() == 0 ? "0" : PlayerManager.instance.GetCurrentAmount().ToString("#,#");
        currentSouls.text = ((int)soulsAmount) == 0 ? "0" : ((int)soulsAmount).ToString("#,#");
    }

    private void UpdateHealhtUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValueHP();
        slider.value = playerStats.currentHealth;

        var currentHP = slider.value / slider.maxValue;
        if (OldSliderValue != 0)
        {
            if (currentHP < OldSliderValue)
            {
                //需要动态改变healthBG
                StartCoroutine(UI_UpdataHpEffect(currentHP, OldSliderValue));
            }
            else
            {
                OldSliderValue = currentHP;
            }
        }
        else
        {
            OldSliderValue = currentHP;
            healthBG.fillAmount = currentHP;
        }
    }
    IEnumerator UI_UpdataHpEffect(float newHP, float oldHP)
    {
        float elapsdTime = 0;
        while (elapsdTime < buffTime)
        {
            elapsdTime += Time.deltaTime;
            healthBG.fillAmount = Mathf.Lerp(oldHP, newHP, elapsdTime / buffTime);
            yield return null;
        }

        healthBG.fillAmount = slider.value / slider.maxValue;
        OldSliderValue = slider.value / slider.maxValue;
    }

    /// <summary>
    /// 如果技能CD好了，使用后则让其完全灰色 0：白色 1灰色
    /// </summary>
    /// <param name="_image"></param>
    private void SetCooldown(Image _image)
    {
        if (_image.fillAmount <= 0)
        {
            _image.fillAmount = 1;
        }
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}
