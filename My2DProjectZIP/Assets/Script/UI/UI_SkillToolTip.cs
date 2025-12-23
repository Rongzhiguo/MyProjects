using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SkillToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI skillDesc;
    [SerializeField] private TextMeshProUGUI skillLockPirce;

    public void ShowToolTip(string _desc, string skillName, bool unlocked, int price)
    {
        skillDesc.text = skillName + "：" + _desc;

        AdjustPosition();

        gameObject.SetActive(true);

        if (unlocked)
        {
            skillLockPirce.gameObject.SetActive(false);
        }
        else
        {
            skillLockPirce.gameObject.SetActive(true);
            skillLockPirce.text = string.Format("解锁价格：{0}金币", price);
        }
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
