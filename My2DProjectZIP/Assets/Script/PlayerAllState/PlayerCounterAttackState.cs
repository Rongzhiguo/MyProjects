using Unity.VisualScripting;
using UnityEngine;

public class PlayerCounterAttackState : playerState
{
    private bool canCreatClone;
    public PlayerCounterAttackState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.skill.parry.cooldownTimer = player.skill.parry.cooldown;
        canCreatClone = true;
        stateTime = player.counterAttackDuration;
        player.animator.SetBool("isSuccessCounterAttack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && hit.GetComponent<Enemy>().CanBeStunned())
            {
                player.animator.SetBool("isSuccessCounterAttack", true);
                stateTime = 10;

                //用于格挡成功后恢复玩家生命值
                player.skill.parry.UseSkill();

                if (canCreatClone)
                {
                    canCreatClone = false;
                    player.skill.parry.MakeMirageOnParry(hit.transform);
                }
            }
        }

        if (stateTime < 0 || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
