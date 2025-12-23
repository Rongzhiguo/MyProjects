using UnityEngine;

public class PlayerGroundedState : playerState
{
    

    public PlayerGroundedState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GameEventsManager.Instance.OnPrimaryAttack += PerformPrimaryAttack;
    }

    public override void Exit()
    {
        base.Exit();
        GameEventsManager.Instance.OnPrimaryAttack -= PerformPrimaryAttack;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.R) && player.skill.blackhole.blackHoleUnlocked)
        {
            if (player.skill.blackhole.cooldownTimer > 0)
            {
                player.fx.ShowPopUpTextFx("Skill Cooldown");
                return;
            }
            stateMachine.ChangeState(player.blackholeState);
        }

        if (!player.IsGrounded())
            stateMachine.ChangeState(player.airState);

        //if (Input.GetKeyDown(KeyCode.Space) && player.IsGrounded())
        //    stateMachine.ChangeState(player.jumpState);

        if (Input.GetKey(KeyCode.Mouse0))
            PerformPrimaryAttack();

        if (Input.GetKey(KeyCode.Q) && player.skill.parry.parryUnlocked)
        {
            if (player.skill.parry.cooldownTimer > 0)
                return;
            stateMachine.ChangeState(player.counterAttackState);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasNoSword() && player.skill.sword_Skill.sowrdUnlocked)
            stateMachine.ChangeState(player.aimSwordSteate);
    }

    private void PerformPrimaryAttack() => stateMachine.ChangeState(player.primaryAttack);

    private bool HasNoSword()
    {
        if (!player.sword)
        {
            return true;
        }
        player.sword.GetComponent<Sword_Skill_Controller>().ReturnSword();
        return false;
    }
}
