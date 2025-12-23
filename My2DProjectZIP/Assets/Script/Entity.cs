using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public SpriteRenderer sr { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody2D rb { get; private set; }

    public int facinDir { get; private set; } = 1;
    public bool facinRight { get; private set; } = true;

    /// <summary>
    /// 击退方向
    /// </summary>
    public int knokcbackDir { get; private set; }


    public CharacterStats stats { get; private set; }

    public CapsuleCollider2D cd { get; private set; }

    [Header("击退效果数据")]
    [SerializeField] protected Vector2 knockbackPower;
    [SerializeField][Tooltip("击退偏移量")] protected Vector2 knockbackOffset;
    [SerializeField] protected float konckbackDuration;
    protected bool isKnocked;


    #region  角色地板检测相关数据
    [Header("角色检测相关数据")]

    public Transform attackCheck;
    public float attackCheckRadius;

    [SerializeField] protected Transform groundCheckGameobject;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    public bool IsGrounded() => Physics2D.Raycast(groundCheckGameobject.position, Vector3.down, groundCheckDistance, whatIsGround);

    [SerializeField] protected Transform wallCheckGameobject;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIswall;
    public bool Iswalled() => Physics2D.Raycast(wallCheckGameobject.position, Vector3.right * facinDir, wallCheckDistance, whatIswall);
    #endregion


    public Action onFlipped;

    protected virtual void Awake()
    {

    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        //animator = transform.Find("Animator").GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();

        if (wallCheckGameobject == null)
            wallCheckGameobject = transform;
    }

    // Update is called once per frame

    protected virtual void FixedUpdate()
    {
    }
    protected virtual void Update()
    {
    }

    /// <summary>
    /// 设置实体缓慢靠近
    /// </summary>
    /// <param name="_slowPercentage"></param>
    /// <param name="_slowDuration"></param>
    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    /// <summary>
    /// 恢复实体原本的速度
    /// </summary>
    protected virtual void ReturnDefaultSpeed()
    {
        animator.speed = 1;
    }

    public virtual void DamageImpact()
    {
        if (knockbackPower.x != 0 && knockbackPower.y != 0)
        {
            StartCoroutine("HitKnockback");
        }
    }


    /// <summary>
    /// 设置被攻击者的面朝方向
    /// </summary>
    /// <param name="_damageDirection">攻击者</param>
    public virtual void SetuoKonckbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knokcbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knokcbackDir = 1;
    }

    /// <summary>
    /// 重新设置击退的力
    /// </summary>
    /// <param name="_knockbackpower"></param>
    public void SetupKnockBackPorer(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;

    protected virtual IEnumerator HitKnockback()
    {
        isKnocked = true;
        float xOffset = UnityEngine.Random.Range(knockbackOffset.x, knockbackOffset.y);
        rb.velocity = new Vector2((knockbackPower.x + xOffset) * knokcbackDir, knockbackPower.y);
        yield return new WaitForSeconds(konckbackDuration);
        isKnocked = false;
        SetupZeroknockbackPower();
    }

    protected virtual void SetupZeroknockbackPower()
    {
    }

    #region Velocity
    public void SetZeroVelocity()
    {
        //如果正在被击退并且对象未死亡，则速度不归0
        if (isKnocked) return;
        rb.velocity = new Vector2(0, 0);
    }
    float SafeFloat(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value))
            return 0f;
        return value;
    }

    public void SetVelocity(float _xVelocity, float _yCelocity)
    {
        if (isKnocked) return;
        float safeX = SafeFloat(_xVelocity);
        float safeY = SafeFloat(_yCelocity);
        rb.velocity = new Vector2(safeX, safeY);
        FileControllers(rb.velocity.x);
    }
    public virtual void FileControllers(float _x)
    {
        if (_x > 0 && !facinRight)
            Flip();
        else if (_x < 0 && facinRight)
            Flip();
    }

    public virtual void Flip()
    {
        facinDir = facinDir * -1;
        facinRight = !facinRight;
        transform.Rotate(0, 180 * facinDir, 0);
        if (onFlipped != null)
            onFlipped();
    }

    /// <summary>
    /// 设置默认朝向
    /// </summary>
    /// <param name="_direction"></param>
    public virtual void SetupDefailtFacingDir(int _direction)
    {
        facinDir = _direction;
        if (facinDir == -1)
            facinRight = false;
    }
    #endregion

    public virtual void Die()
    {

    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheckGameobject.position, new Vector3(groundCheckGameobject.position.x, groundCheckGameobject.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheckGameobject.position, new Vector3(wallCheckGameobject.position.x + (wallCheckDistance * facinDir), wallCheckGameobject.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
}
