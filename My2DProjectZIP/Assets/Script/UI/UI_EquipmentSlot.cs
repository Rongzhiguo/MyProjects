using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot
{
    [Tooltip("孔位类型")] public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = "装备孔位 - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
        {
            return;
        }
        Inventory.instance.UnEquipItem(item.data as ItemData_Equipment);
        Inventory.instance.AddItem(item.data as ItemData_Equipment);

        ui.itemToolTip.HideToolTip();
        ClearUpSlot();
    }
}
