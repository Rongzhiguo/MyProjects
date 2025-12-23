using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;

    protected Rigidbody2D rb;

    protected bool triggerCalled;
    protected float startTimer;

    private string animatName;
    public EnemyState(Enemy _enemyBase, EnemyStateMachine _enemyState, string _animatName)
    {
        enemyBase = _enemyBase;
        stateMachine = _enemyState;
        animatName = _animatName;
    }

    public virtual void Enter()
    {
        enemyBase.animator.SetBool(animatName, true);
        triggerCalled = false;
        rb = enemyBase.rb;
    }

    public virtual void Update()
    {
        startTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        enemyBase.animator.SetBool(animatName, false);
        enemyBase.AssignLastAnimName(animatName);
    }

    public virtual void AnimationFinshTrigger()
    {
        triggerCalled = true;
    }
}
