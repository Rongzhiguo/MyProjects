using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : SingletonMono<PlayerManager>,ISaveManager
{
    public player player;

    [Tooltip("当前拥有的货币数量")] public int currency;

    /// <summary>
    /// 所需要的货币与身上已有的货币进行比较，满足则扣除
    /// </summary>
    /// <param name="_price">需要的货币数量</param>
    /// <returns></returns>
    public bool HaveEnoughMoney(int _price)
    {
        if (_price > currency)
        {
            return false;
        }
        currency -= _price;
        return true;
    }


    public int GetCurrentAmount() => currency;

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
