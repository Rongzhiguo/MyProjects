using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    /// <summary>
    /// 拥有的货币数量
    /// </summary>
    public int currency;

    /// <summary>
    /// 技能树 的解锁情况信息
    /// </summary>
    public SerializableDictionary<string, bool> skillTree;

    /// <summary>
    /// 库存信息
    /// </summary>
    public SerializableDictionary<string, int> inventory;

    /// <summary>
    /// 已穿戴的装备信息
    /// </summary>
    public List<string> EquipmentID;

    /// <summary>
    /// 场景内所有检查点
    /// </summary>
    public SerializableDictionary<string, bool> checkPoints;

    /// <summary>
    /// 最接玩家的检查点的ID
    /// </summary>
    public string closeCheckPointID;

    /// <summary>
    /// 玩家死亡掉落灵魂点相关参数 x,y,灵魂点数
    /// </summary>
    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public SerializableDictionary<string, float> volumeSettings;


    //玩家当前位置
    //public float[] transformPosition;

    public GameData()
    {
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;

        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        EquipmentID = new List<string>();

        closeCheckPointID = string.Empty;
        checkPoints = new SerializableDictionary<string, bool>();

        volumeSettings = new SerializableDictionary<string, float>();
    }
}
