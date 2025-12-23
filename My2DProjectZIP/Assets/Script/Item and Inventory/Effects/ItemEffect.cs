using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "新物品效果", menuName = "数据/物品效果")]
public class ItemEffect : ScriptableObject
{
    [TextArea]
    [Tooltip("特效效果描述")] public string effectDesc;
    /// <summary>
    /// 效果执行
    /// </summary>
    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("触发装备效果！！");
    }
}
