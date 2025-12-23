using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDesc;
    [SerializeField] private Button craftButton;

    [SerializeField] private Transform materialList;

    public void SetupCraftWindow(ItemData_Equipment _data)
    {
        if (_data == null)
        {
            return;
        }

        craftButton.onClick.RemoveAllListeners();

        itemName.text = _data.itemName;
        itemIcon.sprite = _data.itemIcon;

        for (int i = 0; i < materialList.childCount; i++)
        {
            materialList.GetChild(i).gameObject.SetActive(false);
        }
        if (_data.craftingMaterials.Count > materialList.childCount)
        {
            Debug.Log("合成所需材料过多，最大限度应该控制在" + materialList.childCount + "以内。");
            return;
        }
        for (int i = 0; i < _data.craftingMaterials.Count; i++)
        {
            materialList.GetChild(i).gameObject.SetActive(true);
            materialList.GetChild(i).GetComponent<Image>().sprite = _data.craftingMaterials[i].data.itemIcon;
            materialList.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = _data.craftingMaterials[i].stackSize.ToString();
        }

        itemIcon.sprite = _data.itemIcon;
        itemName.text = _data.itemName;
        itemDesc.text = _data.GetDesc();

        craftButton.onClick.AddListener( () => { Inventory.instance.CanCraft(_data, _data.craftingMaterials); });
    }
}
