using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill_Controller : MonoBehaviour
{
    private player player;
    private SpriteRenderer sr;
    [SerializeField] private float colorLoosingSpeed;
    private float cloneTimer;
    private Animator anim;
    private float attackMultiplier;

    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;

    private int facinDir = 1;
    private bool canDuplicateClone;
    private float chanceToDuplicate;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;
        if (cloneTimer < 0)
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorLoosingSpeed));
            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }
    public void SetupClone(Transform _newTransform, float _cloneTimer, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDuplicateClone, float _chanceToDuplicate, player _player, float _attackMultiplier)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));

        player = _player;
        transform.position = _newTransform.position + _offset;
        cloneTimer = _cloneTimer;
        closestEnemy = _closestEnemy;
        canDuplicateClone = _canDuplicateClone;
        chanceToDuplicate = _chanceToDuplicate;
        attackMultiplier = _attackMultiplier;
        FaceCloseTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = .1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //hit.GetComponent<Enemy>().DamageEffect();
                //player.stats.DoDamage(hit.GetComponent<CharacterStats>());

                hit.GetComponent<Entity>().SetuoKonckbackDir(transform);
                PlayerStats playerStats = player.GetComponent<PlayerStats>();
                playerStats.CloneDoDamage(hit.GetComponent<EnemyStats>(), attackMultiplier);

                //克隆体是否可以产生攻击特效
                if (player.skill.clone_Skill.canApplyOnHitEffect)
                {
                    Inventory.instance.GetEquipment(EquipmentType.Weapon)?.Effect(hit.transform);
                }

                if (canDuplicateClone)
                {
                    if (Random.Range(0, 100) < chanceToDuplicate)
                        SkillManager.instance.clone_Skill.CreatClone(hit.transform, new Vector3(0.5f * facinDir, .2f));
                }
            }
        }
    }

    private void FaceCloseTarget()
    {
        if (closestEnemy != null)
        {
            if (transform.position.x > closestEnemy.position.x)
            {
                facinDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
