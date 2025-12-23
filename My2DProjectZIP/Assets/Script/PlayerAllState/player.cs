using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class player : Entity
{
    [Header("攻击效果配置")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    public bool isBusy { get; private set; }
    [Header("移动速度")]
    public float runSpeed;
    public float jumpSpeed;
    private float defaultRunSpeed;
    private float defaultJumpSpeed;

    [Tooltip("剑返回时候的推力速度")]
    public float swordReturnimpact;

    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;

    [Header("连击相关数据")]
    [SerializeField][Tooltip("连击持续时间")] private float comboTimeDuration;
    private float comboTime; //连击时间
    public int comboAttackAmount { get; private set; }
    public CinemachineImpulseSource impulseSource { get; private set; }
    public CinemachineVirtualCamera virtualCamera;
    [Tooltip("镜头缩放数值")] public float scaleAdd;
    private float defauleScale;

    public Volume volume;
    ChromaticAberration aberration;


    public float dashDir { get; private set; }

    public GameObject sword { get; private set; }

    public PlayerFX fx { get; private set; }

    public Joystick joystick;


    #region 管理器模块
    public SkillManager skill { get; private set; }
    #endregion

    #region 状态机
    public playerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimSwordState aimSwordSteate { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    public PlayerBlackholeState blackholeState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new playerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "isIdle");
        runState = new PlayerRunState(this, stateMachine, "isRun");
        jumpState = new PlayerJumpState(this, stateMachine, "isJump");
        airState = new PlayerAirState(this, stateMachine, "isJump");
        dashState = new PlayerDashState(this, stateMachine, "isDash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "isWallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "isJump");
        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "isAttacking");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "isCounterAttack");
        aimSwordSteate = new PlayerAimSwordState(this, stateMachine, "isAimSword");
        catchSwordState = new PlayerCatchSwordState(this, stateMachine, "isCatchSword");
        blackholeState = new PlayerBlackholeState(this, stateMachine, "isJump");
        deadState = new PlayerDeadState(this, stateMachine, "isDead");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        skill = SkillManager.instance;
        stateMachine.Initialize(idleState);

        defaultRunSpeed = runSpeed;
        defaultJumpSpeed = jumpSpeed;
        defaultDashSpeed = dashSpeed;

        impulseSource = GetComponent<CinemachineImpulseSource>();

        defauleScale = virtualCamera.m_Lens.OrthographicSize;

        aberration = volume.profile.components[0] as ChromaticAberration;

        fx = GetComponentInChildren<PlayerFX>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        stateMachine.currentState.FixedUpdate();

        comboTime -= Time.deltaTime;
        if (comboTime <= 0 && comboAttackAmount != 0)
        {
            comboAttackAmount = 0;
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, defauleScale, 0.5f);
            DOTween.To(() => aberration.intensity.value, x => aberration.intensity.value = x, 0, 0.5f);
        }
    }

    /// <summary>
    /// 重置连击持续时间并且增加连击计数
    /// </summary>
    public void SetComboTime()
    {
        comboTime = comboTimeDuration;
        comboAttackAmount++;

        if (comboAttackAmount <= 7)
        {
            var orthographicSize = virtualCamera.m_Lens.OrthographicSize - scaleAdd;
            DOTween.To(() => virtualCamera.m_Lens.OrthographicSize, x => virtualCamera.m_Lens.OrthographicSize = x, orthographicSize, 0.2f);
            //aberration.intensity.value += 0.02f;
        }
        else
            virtualCamera.m_Lens.OrthographicSize -= 0.01f;
    }

    /// <summary>
    /// 设置URP色差方法
    /// </summary>
    /// <param name="_value"></param>
    public void SetUpAberration(float _value)
    {
        DOTween.To(() => aberration.intensity.value, x => aberration.intensity.value = x, _value, 0.2f);
    }

    /// <summary>
    /// 还原URP色差值
    /// </summary>
    public void DefauleAberration()
    {
        DOTween.To(() => aberration.intensity.value, x => aberration.intensity.value = x, 0, 0.3f);
    }


    private float coyoteTime = 0.15f;
    private float jumpBufferTime = 0.2f;
    private float jumpInputCooldown = 0.15f; // 跳跃输入冷却时间

    private float _lastGroundedTime = -1;
    private float _lastJumpPressTime = -1;
    private bool _isJumpInputLocked; // 跳跃输入锁

    // Update is called once per frame
    protected override void Update()
    {
        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();
        InputCheckJump();

        if (Input.GetKeyDown(KeyCode.L))
            PlayerManager.instance.currency += 1000;

        CheckForDashInput();

        //按V 释放卡莎Q技能弹道效果技能
        if (Input.GetKeyDown(KeyCode.V))
            skill.crossed.CanUseSkill();

        // 按F 释放水晶技能
        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.UseFlask();
    }

    private void InputCheckJump()
    {
        if (stats.currentHealth <= 0)
            return;

        HandleJumpInput();

        if (IsGrounded())
        {
            _lastGroundedTime = Time.time;
        }

        // 跳跃缓冲检测
        bool hasJumpBuffered = Time.time - _lastJumpPressTime <= jumpBufferTime;
        bool inCoyoteTime = Time.time - _lastGroundedTime <= coyoteTime;
        if (hasJumpBuffered && (IsGrounded() || inCoyoteTime))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            stateMachine.ChangeState(jumpState);
            _lastJumpPressTime = 0; // 清除缓冲
        }
    }

    private void HandleJumpInput()
    {
        if (_isJumpInputLocked) return;

        if (Input.GetKeyDown(KeyCode.Space))
        { 
            _lastJumpPressTime = Time.time;
            StartCoroutine(JumpInputCooldown());
        }
    }

    private IEnumerator JumpInputCooldown()
    {
        _isJumpInputLocked = true;
        yield return new WaitForSeconds(jumpInputCooldown);
        _isJumpInputLocked = false;
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        runSpeed = runSpeed * (1 - _slowPercentage);
        jumpSpeed = jumpSpeed * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        animator.speed = animator.speed * (1 - _slowPercentage);
        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        runSpeed = defaultRunSpeed;
        jumpSpeed = defaultJumpSpeed;
        dashSpeed = defaultDashSpeed;
    }

    public void AssingNewSwrod(GameObject newSword)
    {
        sword = newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }

    public IEnumerator BusyFor(float _secondes)
    {
        isBusy = true;
        yield return new WaitForSeconds(_secondes);
        isBusy = false;
    }

    private void CheckForDashInput()
    {
        if (!skill.dash_Skill.dashUnlocked)
            return;

        //冲刺之前判断正前方是否挨着障碍物
        if (Iswalled()) return;
        if (Input.GetKeyDown(KeyCode.LeftShift) && skill.dash_Skill.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");
            if (joystick.Horizontal != 0)
            {
                dashDir = joystick.Horizontal;
            }
            if (dashDir == 0)
                dashDir = facinDir;
            stateMachine.ChangeState(dashState);
        }
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinshTrigger();

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroknockbackPower()
    {
        knockbackPower = Vector2.zero;
    }
}
