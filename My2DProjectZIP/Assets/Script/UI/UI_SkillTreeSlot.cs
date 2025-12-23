using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    private UI ui;
    private Image skillIcon;

    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;

    [TextArea]
    [SerializeField] private string skillDesc;

    [SerializeField] private Color lockedSkillClolr;

    [Tooltip("是否解锁")] public bool unlocked;
    [SerializeField][Tooltip("哪些技能应解锁")] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField][Tooltip("哪些技能应锁定")] private UI_SkillTreeSlot[] shouldBeLocked;

    public Action onSkillUnlocked;  //点击按钮关联事件，让skillManager中响应bool值修改

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(
            () =>
            {
                UnLockSkillSlot();
            }
        );
    }

    private void Start()
    {
        ui = GetComponentInParent<UI>();
        skillIcon = GetComponent<Image>();

        if (unlocked)
            skillIcon.color = Color.white;
        else
            skillIcon.color = lockedSkillClolr;
    }

    public void UnLockSkillSlot()
    {
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unlocked)
            {
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked)
            {
                return;
            }
        }

        if (unlocked)
        {
            Debug.Log("您已激活此技能，无需重复激活！");
            return;
        }

        if (!PlayerManager.instance.HaveEnoughMoney(skillPrice))
        {
            Debug.Log("激活此技能需要" + skillPrice + "金币，您金币不足，无法激活！");
            return;
        }

        unlocked = true;
        skillIcon.color = Color.white;

        onSkillUnlocked?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(skillDesc, skillName, unlocked, skillPrice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if (_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }
}
