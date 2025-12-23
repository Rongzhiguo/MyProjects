using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("玩家掉落")]
    [SerializeField][Tooltip("玩家掉落装备概率（0-100）")][Range(0, 100)] private float chanceTolooseItems;
    [SerializeField][Tooltip("玩家掉落材料概率（0-100）")][Range(0, 100)] private float chanceTolooseMateir;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;
        DropMateirs(inventory);
        DropEquipment(inventory);
    }

    /// <summary>
    /// 掉落装备方法
    /// </summary>
    /// <param name="inventory"></param>
    private void DropEquipment(Inventory inventory)
    {
        List<InventoryItem> itemsToUnEquip = new List<InventoryItem>();

        foreach (var item in inventory.GetEquipmentList())
        {
            if (Random.Range(0, 100) <= chanceTolooseItems)
            {
                DropItem(item.data);
                itemsToUnEquip.Add(item);
            }
        }

        for (int i = 0; i < itemsToUnEquip.Count; i++)
        {
            inventory.UnEquipItem(itemsToUnEquip[i].data as ItemData_Equipment);
            inventory.UpdateSlotUI();
        }
    }

    /// <summary>
    /// 掉落材料方法
    /// </summary>
    /// <param name="inventory"></param>
    private void DropMateirs(Inventory inventory)
    {
        List<InventoryItem> mateirToLoose = new List<InventoryItem>();

        foreach (var item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= chanceTolooseMateir)
            {
                DropItem(item.data);
                mateirToLoose.Add(item);
            }
        }

        for (int i = 0; i < mateirToLoose.Count; i++)
        {
            inventory.RemoveItem(mateirToLoose[i].data);
        }
    }
}
