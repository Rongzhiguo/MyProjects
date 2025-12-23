using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField][Tooltip("掉落物品数量")] private int possibleItemDrop;
    [SerializeField][Tooltip("掉落物数据")] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0,100) <= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        for (int i = 0; i < possibleItemDrop; i++)
        {
            if (dropList.Count <= 0)
            {
                break;
            }
            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];
            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        if (_itemData == null)
        {
            Debug.Log("_itemData为空");
            return;
        }
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(10, 15));
        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
