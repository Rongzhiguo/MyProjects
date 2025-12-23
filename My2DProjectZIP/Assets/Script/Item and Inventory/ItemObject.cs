using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private ItemData itemData;

    private void OnValidate()
    {
        SetupVisuals();
    }

    private void SetupVisuals()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.itemIcon;
        gameObject.name = "Item Object - " + itemData.itemName;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;
        SetupVisuals();
    }


    /// <summary>
    /// 拾取物品
    /// </summary>
    public void PickupItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            PlayerManager.instance.player.fx.ShowPopUpTextFx("Inventory is full"); //背包已满！
            return;
        }
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
