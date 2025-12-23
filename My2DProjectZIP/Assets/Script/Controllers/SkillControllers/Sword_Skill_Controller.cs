using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float retrunSpeed;

    [Header("Pierce 穿刺类型信息")]
    private int pierceAmount;

    [Header("Bounce 弹跳类型信息")]
    private float bounceSpeed; //弹跳速度
    private bool isBouncing;  //是否处于弹跳状态
    private int bounceAmount;  //弹跳次数
    private List<Transform> enemyTarget;  //敌人列表
    private int targerIndex;

    [Header("Spin 定点旋转信息")]
    private float maxTravelDistance;  //与玩家的最大距离
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }


    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, player _player, float _freezeTimeDuration, float _retrunSpeed)
    {
        player = _player;
        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;
        freezeTimeDuration = _freezeTimeDuration;
        retrunSpeed = _retrunSpeed;
        if (pierceAmount <= 0)
            anim.SetBool("isRotat", true);

        spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7);
    }

    public void SetupBounce(bool _isBouncing, int _bounceAmount, float _bounceSpeed)
    {
        isBouncing = _isBouncing;
        bounceAmount = _bounceAmount;
        bounceSpeed = _bounceSpeed;
        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void SetupSpin(bool _isSpinning, float _spinDuration, float _maxTravelDistance, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        spinDuration = _spinDuration;
        maxTravelDistance = _maxTravelDistance;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //rb.isKinematic = false;
        transform.parent = null;
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, retrunSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, player.transform.position) < 1)
            {
                player.CatchTheSword();
            }
        }
        BounceLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(transform.position, player.transform.position) >= maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }
            if (wasStopped)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);
                spinTimer -= Time.deltaTime;
                hitTimer -= Time.deltaTime;
                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1);
                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                        {
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                        }
                    }
                }

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }
            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void BounceLogic()
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targerIndex].position, bounceSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, enemyTarget[targerIndex].position) < 0.1f)
            {
                SwordSkillDamage(enemyTarget[targerIndex].GetComponent<Enemy>());
                targerIndex++;
                bounceAmount--;
                if (targerIndex >= enemyTarget.Count)
                {
                    targerIndex = 0;
                }
                if (bounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning) return;

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);

            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        enemyTarget.Add(hit.transform);
                    }
                }
            }
        }

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        player.stats.DoDamage(enemyStats);

        if (player.skill.sword_Skill.timeStopUnlocked)
            enemy.FreezeTimeFor(freezeTimeDuration);

        if (player.skill.sword_Skill.vulnurableUnlocked)
            enemyStats.MakeVulnurableFor(freezeTimeDuration);

        //获取玩家身上项链位置是否穿戴物品
        ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
        if (equipedAmulet != null)
        {
            equipedAmulet.Effect(enemy.transform);
        }
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        GetComponentInChildren<ParticleSystem>().Play();

        if (isBouncing && enemyTarget.Count > 0) return;

        anim.SetBool("isRotat", false);
        transform.parent = collision.transform;
    }
}
