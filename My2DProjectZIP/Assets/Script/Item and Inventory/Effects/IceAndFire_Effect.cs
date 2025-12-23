using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "冰火效果", menuName = "数据/物品效果/冰与火")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _respawnPosition)
    {
        player currentPlayer = PlayerManager.instance.player;
        bool thirdAttack = currentPlayer.primaryAttack.comboCounter == 2;
        if (thirdAttack)
        {
            GameObject newiceAndFire = Instantiate(iceAndFirePrefab, _respawnPosition.position, currentPlayer.transform.rotation);
            newiceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * currentPlayer.facinDir, 0);
            Destroy(newiceAndFire, 10);
        }
    }
}
