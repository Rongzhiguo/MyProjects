using System.Text;
using UnityEngine;

public enum ItemType
{
    Material,
    Equipment,
}

[CreateAssetMenu(fileName = "新物品数据", menuName = "数据/物品")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public string itemID;

    [Range(0, 100)]
    [Tooltip("装备掉落概率")] public float dropChance;

    protected StringBuilder sb = new StringBuilder();

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = UnityEditor.AssetDatabase.GetAssetPath(this);
        itemID = UnityEditor.AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public virtual string GetDesc()
    {
        return "";
    }
}
