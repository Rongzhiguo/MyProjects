using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "雷击效果", menuName = "数据/物品效果/雷击")]
public class LightningStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;
    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab, _enemyPosition.position, Quaternion.identity);
        Destroy(newThunderStrike, 1);
    }
}
