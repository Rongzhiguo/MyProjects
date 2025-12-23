using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeGroundState : EnemyState
{
    protected Enemy_Slime enemy;
    protected Transform player;
    public SlimeGroundState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName , Enemy_Slime _enemy) : base(_enemyBase, _enemyState, _animatName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        bool playerIsDead = player.GetComponent<CharacterStats>().isDead;
        if (!playerIsDead && (enemy.isPlayerDetected() || Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.agroDistance))
        {
            stateMachine.ChangeState(enemy.battleState);
        }
    }
}
