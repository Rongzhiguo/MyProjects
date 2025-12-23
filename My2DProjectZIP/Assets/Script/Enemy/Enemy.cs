using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    bool isAttack;

    [Header("格挡击退配置")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;

    [Header("移动速度")]
    public float moveSpeed;
    public float idleTimer;
    public float battleTime;
    private float defMoveSpeed;

    [Header("检测Player对象")]
    [SerializeField] private float playerCheckDistance;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("攻击配置")]
    public float agroDistance = 2;
    public float attackDistance;
    public float attackCD;
    public float minAttackCD;
    public float maxAttackCD;
    [HideInInspector] public float lastTimeAttack = 1.2f;

    public EntityFX fx { get; private set; }

    public RaycastHit2D isPlayerDetected() => Physics2D.Raycast(wallCheckGameobject.position, Vector2.right * facinDir, playerCheckDistance, whatIsPlayer);

    public EnemyStateMachine stateMachine { get; private set; }

    public string lastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
        defMoveSpeed = moveSpeed;
    }

    protected override void Start()
    {
        base.Start();
        fx = GetComponentInChildren<EntityFX>();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

    }

    /// <summary>
    /// 分配最后的动画名（用于死亡时候定格最后动画名）
    /// </summary>
    /// <param name="_anmiBoolName"></param>
    public virtual void AssignLastAnimName(string _anmiBoolName) => lastAnimBoolName = _anmiBoolName;


    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        animator.speed = animator.speed * (1 - _slowPercentage);
        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = defMoveSpeed;
    }

    /// <summary>
    /// 冰冻时间（使用黑洞技能时候冻结黑洞内所有怪物的移速和动作）
    /// </summary>
    /// <param name="_timeFreeze"></param>
    public virtual void FreezeTime(bool _timeFreeze)
    {
        if (_timeFreeze)
        {
            moveSpeed = 0;
            animator.speed = 0;
        }
        else
        {
            moveSpeed = defMoveSpeed;
            animator.speed = 1;
        }
    }

    /// <summary>
    /// 冻结时间
    /// </summary>
    /// <param name="_duration">时间静止的秒数</param>
    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimCoroutine(_duration));
    protected virtual IEnumerator FreezeTimCoroutine(float _seconds)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(_seconds);
        FreezeTime(false);
    }

    #region 反击窗口
    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    public virtual bool CanBeStunned()
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }
        return false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinshTrigger();

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + playerCheckDistance * facinDir, transform.position.y));
    }
}
