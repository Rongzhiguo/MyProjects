using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    public SkeletonIdleState idleState { get; private set; }
    public SkeletonMoveState moveState { get; private set; }
    public SkeletonBattleState battleState { get; private set; }
    public SkeletonAttackState attackState { get; private set; }
    public SkeletonStunnedState stunnedState { get; private set; }
    public SkeletonDeadState deadState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        idleState = new SkeletonIdleState(this, stateMachine, "isIdle", this);
        moveState = new SkeletonMoveState(this, stateMachine, "isMove", this);
        battleState = new SkeletonBattleState(this, stateMachine, "isMove", this);
        attackState = new SkeletonAttackState(this, stateMachine, "isAttack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "isStun", this);
        deadState = new SkeletonDeadState(this, stateMachine, "isIdle", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }
}
