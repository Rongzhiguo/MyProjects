using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossed_Skill : Skill
{
    [SerializeField] private GameObject crossedPrefab;
    [SerializeField] private int creatCrossedCount;
    private GameObject currentCrossed;

    public override void UseSkill()
    {
        base.UseSkill();
        currentCrossed = Instantiate(crossedPrefab, player.transform.position, Quaternion.identity);
        Crossed_Skill_Controller crossedScript = currentCrossed.GetComponent<Crossed_Skill_Controller>();
        crossedScript.Init(player, FindClosestEnemy(currentCrossed.transform, 25));
    }
}
