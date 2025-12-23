using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttackEvents : MonoBehaviour
{
    private player _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponentInParent<player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void AnimationTrigger()
    {
        _player.AnimationTrigger();
    }

    private void AttackTrigger()
    {

        AudioManager.instance.PlaySFX(8);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(_player.attackCheck.position, _player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                _player.SetComboTime();
                //设置相机抖动
                _player.fx.ScreenSkake(_player.attackMovement[_player.primaryAttack.comboCounter]);

                if (_player.primaryAttack.comboCounter == 2)
                    AttackSense.instance.HitPause(6);  //控制游戏时间流逝速度

                EnemyStats _target = hit.GetComponent<EnemyStats>();
                if (_target != null)
                    _player.stats.DoDamage(_target);
                Inventory.instance.GetEquipment(EquipmentType.Weapon)?.Effect(hit.transform);
            }
        }
    }

    private Vector3 GetAttackForce(int comboCounter)
    {
        return _player.attackMovement[comboCounter];
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword_Skill.CreatSword();
    }
}
