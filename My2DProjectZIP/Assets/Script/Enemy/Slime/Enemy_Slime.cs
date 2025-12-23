using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlimeType
{
    big,
    medium,
    small,
}

public class Enemy_Slime : Enemy
{
    [Header("史莱姆专属")]
    [SerializeField][Tooltip("史莱姆的类型")] private SlimeType slimeType;
    [SerializeField][Tooltip("史莱姆的预制体")] private GameObject slimePrefab;
    [SerializeField][Tooltip("史莱姆死亡后可创建多少复制体")] private int slimeToCreate;
    [SerializeField][Tooltip("创建后的最小初始速度")] private Vector2 minCreateVelocity;
    [SerializeField][Tooltip("创建后的最大初始速度")] private Vector2 maxCreateVelocity;



    public SlimeIdleState idleState { get; private set; }
    public SlimeMoveState moveState { get; private set; }
    public SlimeBattleState battleState { get; private set; }
    public SlimeAttackState attackState { get; private set; }
    public SlimeStunnedState stunnedState { get; private set; }
    public SlimeDeadState deadState { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        SetupDefailtFacingDir(-1);
        idleState = new SlimeIdleState(this, stateMachine, "isIdle", this);
        moveState = new SlimeMoveState(this, stateMachine, "isMove", this);
        battleState = new SlimeBattleState(this, stateMachine, "isMove", this);
        attackState = new SlimeAttackState(this, stateMachine, "isAttack", this);
        stunnedState = new SlimeStunnedState(this, stateMachine, "isStun", this);
        deadState = new SlimeDeadState(this, stateMachine, "isIdle", this);
    }

    protected override void Start()
    {
        base.Start();
        InitState();
    }

    public void InitState() => stateMachine.Initialize(idleState);

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }
        return false;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);

        if (slimeType == SlimeType.small)
            return;

        CreateSlime(slimeToCreate, slimePrefab);
    }

    private void CreateSlime(int _amountOfSlime, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlime; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);
            if (newSlime != null)
                newSlime.GetComponent<Enemy_Slime>().SetupSlime(facinDir);
        }
    }

    public void SetupSlime(int _facinDir)
    {
        if (facinDir != _facinDir)
            Flip();

        float xVelocity = Random.Range(minCreateVelocity.x, maxCreateVelocity.x);
        float yVelocity = Random.Range(minCreateVelocity.y, maxCreateVelocity.y);

        isKnocked = true;

        GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * -facinDir, yVelocity);

        Invoke("CancelKnockback", 1.5f);
    }

    private void CancelKnockback() => isKnocked = false;

}
