using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    //public CharacterStats playerStats;

    [Tooltip("玩家初始状态下的物品信息")] public List<ItemData> startingEquipment;

    public List<InventoryItem> equipment;

    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    public List<InventoryItem> inventory;

    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    public List<InventoryItem> stash;

    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent; //装备包裹
    [SerializeField] private Transform stashSlotParent;  //材料包裹
    [SerializeField] private Transform equipmentSlotParent;  //已装备包裹
    [SerializeField] private Transform statSlotParent;

    private UI_ItemSlot[] inventoryItemSlot; //装备库存位置
    private UI_ItemSlot[] stashItemSlot;    //材料
    private UI_EquipmentSlot[] equipmentSlots; //已装备信息
    private UI_StatSlot[] statSlot;

    //[Header("Item cooldown(物品冷却)")]
    private float lastTimeUsedFlask; //装备位瓶的CD时间
    private float lastTimeUsedArmor; //装备位盔甲的CD时间

    public float flaskCooldown { get; private set; }


    [Header("数据库")]
    public List<ItemData> itemDataBase;
    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    [Space]
    [Tooltip("装备背包最大容量")]
    public int inventoryMaxCount = 40;

    private int inventoryPage;
    private int inventoryMaxPage;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlots = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();

        statSlot = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        inventoryPage = 1;

        AddStartingItems();

    }

    /// <summary>
    /// 为玩家添加初始装备
    /// </summary>
    private void AddStartingItems()
    {
        //从本地缓存添加玩家身上装备数据
        foreach (var item in loadedEquipment)
        {
            EquipItem(item);
        }

        //把本地缓存中的装备和材料数据分别添加到玩家对应的背包中
        if (loadedItems.Count > 0)
        {
            foreach (var item in loadedItems)
            {
                for (int i = 0; i < item.stackSize; i++)
                {
                    AddItem(item.data);
                }
            }
            return;
        }

        for (int i = 0; i < startingEquipment.Count; i++)
        {
            if (startingEquipment[i] != null)
            {
                AddItem(startingEquipment[i]);
            }
        }
    }

    /// <summary>
    /// 添加玩家穿戴的装备(穿装备)
    /// </summary>
    /// <param name="_item"></param>
    public void EquipItem(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);

        ItemData_Equipment oldEquip = null;

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquip = item.Key;
            }
        }

        if (oldEquip != null)
        {
            UnEquipItem(oldEquip);
            AddItem(oldEquip);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();
        RemoveItem(_item);
    }

    /// <summary>
    /// 移除玩家穿戴的装备(拖装备)
    /// </summary>
    /// <param name="oldEquip"></param>
    public void UnEquipItem(ItemData_Equipment oldEquip)
    {
        if (equipmentDictionary.TryGetValue(oldEquip, out InventoryItem value))
        {
            equipment.Remove(value);
            equipmentDictionary.Remove(oldEquip);
            oldEquip.RemoveModifiers();
        }
    }

    public void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].ClearUpSlot();
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlots[i].slotType)
                {
                    equipmentSlots[i].UpdateSlot(item.Value);
                    break;
                }
            }
        }

        UpDateInventorys();

        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].ClearUpSlot();
        }

        //更新材料背包数据
        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }

        //更新属性面数据
        UpdateStatsUI();

    }

    public void UpdateStatsUI()
    {
        for (int i = 0; i < statSlot.Length; i++)
        {
            statSlot[i].UpdateStatValueUI();
        }
    }

    /// <summary>
    /// 更新背包方法
    /// </summary>
    private void UpDateInventorys()
    {
        //inventoryItemSlot ： 一页的最大容量
        //身上所拥有的物品数量 inventory

        //最大页数
        inventoryMaxPage = Mathf.CeilToInt((float)inventory.Count / (float)inventoryItemSlot.Length);

        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].ClearUpSlot();
        }

        //更新装备背包数据
        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            var index = ((inventoryPage - 1) * inventoryItemSlot.Length) + i;
            if (index < inventory.Count)
                inventoryItemSlot[i].UpdateSlot(inventory[index]);
            else
                break;
        }


        //for (int i = 0; i < inventory.Count; i++)
        //{
        //    inventoryItemSlot[i].UpdateSlot(inventory[i]);
        //}
    }

    /// <summary>
    /// 获取当前的页数
    /// </summary>
    public void GetCurrentPage(int _pageindex)
    {
        inventoryPage = inventoryPage + _pageindex;

        if (inventoryPage < 1)
            inventoryPage = 1;
        else if (inventoryPage > inventoryMaxPage)
            inventoryPage = inventoryMaxPage;

        UpDateInventorys();
    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(_item);
        }
        else if (_item.itemType == ItemType.Material)
        {
            AddToStash(_item);
        }

        UpdateSlotUI();
    }

    public void RemoveItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment)
        {
            RemoveToInventory(_item);
        }
        else if (_item.itemType == ItemType.Material)
        {
            RemoveToStash(_item);
        }
        UpdateSlotUI();
    }

    /// <summary>
    /// 是否能将装备添加到装备背包（是否已达上限）
    /// </summary>
    /// <returns></returns>
    public bool CanAddItem()
    {
        if (inventory.Count >= inventoryMaxCount)
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// 物品合成方法
    /// </summary>
    /// <param name="_itemToCraft">需要合成的产物</param>
    /// <param name="_requiredMaterials">合成所需要的材料列表</param>
    /// <returns></returns>
    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    {
        List<InventoryItem> materialRemove = new List<InventoryItem>();
        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)
                {
                    Debug.Log("材料背包中" + _requiredMaterials[i].data.itemName + "数量不足" + _requiredMaterials[i].stackSize);
                    return false;
                }
                else
                {
                    materialRemove.Add(_requiredMaterials[i]);
                }
            }
            else
            {
                Debug.Log("材料背包中尚未找到" + _requiredMaterials[i].data.itemName + "物品");
                return false;
            }
        }

        for (int i = 0; i < materialRemove.Count; i++)
        {
            for (int k = 0; k < materialRemove[i].stackSize; k++)
            {
                RemoveItem(materialRemove[i].data);
            }
        }

        AddItem(_itemToCraft);
        Debug.Log("恭喜成功打造" + _itemToCraft.itemName + "一件");
        return true;
    }


    /// <summary>
    /// 添加材料方法
    /// </summary>
    /// <param name="_item"></param>
    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    }

    /// <summary>
    /// 添加装备方法
    /// </summary>
    /// <param name="_item"></param>
    private void AddToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }


    /// <summary>
    /// 移除材料方法
    /// </summary>
    /// <param name="_item"></param>
    private void RemoveToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize > 1)
            {
                value.RemoveStack();
            }
            else
            {
                stash.Remove(value);
                stashDictionary.Remove(_item);
            }
        }
    }

    /// <summary>
    /// 移除装备方法
    /// </summary>
    /// <param name="_item"></param>
    private void RemoveToInventory(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize > 1)
            {
                value.RemoveStack();
            }
            else
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
        }
    }


    public List<InventoryItem> GetEquipmentList() => equipment;

    public List<InventoryItem> GetStashList() => stash;

    /// <summary>
    /// 获取身上穿戴某个位置的装备
    /// </summary>
    /// <param name="_equipmentType"></param>
    /// <returns></returns>
    public ItemData_Equipment GetEquipment(EquipmentType _equipmentType)
    {
        ItemData_Equipment equipedItem = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == _equipmentType)
            {
                equipedItem = item.Key;
                break;
            }
        }
        return equipedItem;
    }

    /// <summary>
    /// 使用身上瓶子装备位的装备
    /// </summary>
    public void UseFlask()
    {
        ItemData_Equipment currentFlask = GetEquipment(EquipmentType.Flask);
        if (currentFlask == null)
            return;

        bool canUseFlask = Time.time > lastTimeUsedFlask;

        //物品使用正在冷却
        if (!canUseFlask)
            return;

        currentFlask.Effect(null);
        lastTimeUsedFlask = Time.time + currentFlask.itemCooldown;
        flaskCooldown = currentFlask.itemCooldown;
    }

    public bool CanUseArmor()
    {
        ItemData_Equipment currentArmor = GetEquipment(EquipmentType.Armor);
        if (currentArmor == null)
            return false;
        if (Time.time > lastTimeUsedArmor)
        {
            lastTimeUsedArmor = Time.time + +currentArmor.itemCooldown;
            return true;
        }
        return false;
    }

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, int> pair in _data.inventory)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemID == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;
                    loadedItems.Add(itemToLoad);
                    break;
                }
            }
        }

        foreach (var loadItemID in _data.EquipmentID)
        {
            foreach (var item in itemDataBase)
            {
                if (item != null && item.itemID == loadItemID)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        _data.EquipmentID.Clear();

        //背包装备字典
        foreach (KeyValuePair<ItemData, InventoryItem> pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }

        //背包材料字典
        foreach (KeyValuePair<ItemData, InventoryItem> pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }

        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> pair in equipmentDictionary)
        {
            _data.EquipmentID.Add(pair.Key.itemID);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("填满ItemDataBase")]
    private void FillUpItenDataBase()
    {
        itemDataBase = new List<ItemData>(GetItemDataBase());
    }

    private List<ItemData> GetItemDataBase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetName = UnityEditor.AssetDatabase.FindAssets("", new[] { "Assets/Script/Data/Items" });
        foreach (var SOName in assetName)
        {
            var SOpath = UnityEditor.AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(SOpath);
            itemDataBase.Add(itemData);
        }
        return itemDataBase;
    }
#endif

}
