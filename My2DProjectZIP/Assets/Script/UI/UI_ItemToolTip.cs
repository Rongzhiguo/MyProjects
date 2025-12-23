using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private Image itemIcon;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void ShowToolTip(ItemData_Equipment item)
    {
        if (item == null)
            return;

        string itemType = "材料";
        switch (item.equipmentType)
        {
            case EquipmentType.Weapon:
                itemType = "武器";
                break;
            case EquipmentType.Armor:
                itemType = "盔甲";
                break;
            case EquipmentType.Amulet:
                itemType = "项链";
                break;
            case EquipmentType.Flask:
                itemType = "瓶子";
                break;
            default:
                break;
        }
        itemNameText.text = item.itemName;
        itemTypeText.text = "类型:"+itemType;
        itemIcon.sprite = item.itemIcon;

        //物品描述暂时没有，所以不显示
        itemDescText.text = item.GetDesc();


        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
