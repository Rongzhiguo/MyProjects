using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistTime;
    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closestEnemy;

    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCryStal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestEnemy)
    {
        crystalExistTime = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestEnemy = _closestEnemy;
    }

    public void ChooseRandomEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, SkillManager.instance.blackhole.GetBlackhloleRadius(), whatIsEnemy);
        if (colliders.Length > 0)
            closestEnemy = colliders[Random.Range(0, colliders.Length)].transform;

    }

    private void Update()
    {
        crystalExistTime -= Time.deltaTime;
        if (crystalExistTime < 0)
        {
            FinishCrystal();
        }

        if (canMove && closestEnemy != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestEnemy.position, moveSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, closestEnemy.position) < .5f)
            {
                FinishCrystal();
                canMove = false;
            }
        }

        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                PlayerManager.instance.player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());

                //获取玩家身上项链位置是否穿戴物品
                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);
                if (equipedAmulet != null)
                {
                    equipedAmulet.Effect(hit.transform);
                }
            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            anim.SetTrigger("Explode");
            canGrow = true;
        }
        else
            SelfDestroy();
    }

    public void SelfDestroy() => Destroy(gameObject);
}
