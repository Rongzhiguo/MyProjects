using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveManager
{
    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="_data"></param>
    void LoadData(GameData _data);

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="_data"></param>
    void SaveData(ref GameData _data);
}
