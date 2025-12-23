using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
        {
            return;
        }
        item.data = _data;
        itemImage.sprite = _data.itemIcon;
        itemText.text = _data.itemName;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //base.OnPointerDown(eventData);
        ItemData_Equipment craftData = item.data as ItemData_Equipment;
        if (craftData == null)
        {
            Debug.Log("请配置正确的装备合成，目前物品类型为：" + item.data.itemType.ToString());
            return;
        }
        ui.craftWindow.SetupCraftWindow(craftData);
        //Inventory.instance.CanCraft(craftData, craftData.craftingMaterials);

    }
}
