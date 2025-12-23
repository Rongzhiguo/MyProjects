using UnityEngine;

public class PlayerCatchSwordState : playerState
{
    private Transform sword;

    public PlayerCatchSwordState(player _player, playerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        sword = player.sword.transform;
        if (player.transform.position.x > sword.position.x && player.facinDir == 1)
        {
            player.Flip();
        }
        else if (player.transform.position.x < sword.position.x && player.facinDir == -1)
        {
            player.Flip();
        }

        rb.velocity = new Vector2(player.swordReturnimpact * -player.facinDir, rb.velocity.y);

        player.fx.PlayDustFX();
        player.fx.shakeMultiplier = 2;
        player.fx.ScreenSkake(player.fx.swordShakePwoer);
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", .1f);
    }

    public override void Update()
    {
        base.Update();
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
