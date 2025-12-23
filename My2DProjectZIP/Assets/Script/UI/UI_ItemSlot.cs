using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    protected UI ui;

    public InventoryItem item;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    // Start is called before the first frame update
    public void UpdateSlot(InventoryItem _item)
    {
        if (_item != null)
        {
            itemImage.color = Color.white;
            item = _item;
            itemImage.sprite = item.data.itemIcon;
            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void ClearUpSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
        {
            return;
        }

        //Ctrl+左键删除选中物品
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            ui.itemToolTip.HideToolTip();
            return;
        }
        //Debug.Log("点击了：" + item.data.itemName);
        if (item.data.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(item.data);
        }
        ui.itemToolTip.HideToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;
        //鼠标进入事件
        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);

        Vector2 mousePosition = Input.mousePosition;
        float xOffset = 0;
        float yOffset = 0;
        if (mousePosition.x >= Screen.width / 2)
            xOffset = -Screen.width / 8;
        else
            xOffset = Screen.width / 8;

        if (mousePosition.y >= Screen.height / 2)
            yOffset = -Screen.height / 6;
        else
            yOffset = Screen.height / 6;
        ui.itemToolTip.transform.position = new Vector3(mousePosition.x + xOffset, mousePosition.y + yOffset);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;
        //鼠标移除事件
        ui.itemToolTip.HideToolTip();
    }
}
