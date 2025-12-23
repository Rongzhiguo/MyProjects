using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinksSpeed;
    private float blackholeTime;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreatHotKeys = true;
    private bool cloneAttackReleased;

    private int amountOfAttacks;
    private float cloneAttackCooldowm = .3f;
    private float cloneAttackTimer;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHodKey = new List<GameObject>();

    public bool playerCanExitState { get; private set; }


    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinksSpeed, int _amountOfAttacks, float _cloneAttackCooldowm, float _blakeholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinksSpeed = _shrinksSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldowm = _cloneAttackCooldowm;
        blackholeTime = _blakeholeDuration;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTime -= Time.deltaTime;

        if (blackholeTime <= 0)
        {
            blackholeTime = Mathf.Infinity;
            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FinishBlackholeAbility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinksSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            amountOfAttacks--;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset = Random.Range(0, 100) > 50 ? .5f : -.5f;
            float second = 2f;

            if (SkillManager.instance.clone_Skill.crystalInseadOfClone)
            {
                SkillManager.instance.crystal.CreatCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomTarget();
                second = 1f;
            }
            else
                SkillManager.instance.clone_Skill.CreatClone(targets[randomIndex], new Vector3(xOffset, 0));
            cloneAttackTimer = cloneAttackCooldowm;

            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackholeAbility", second);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        DestroyHotKeys();
        if (targets.Count > 0)
        {
            cloneAttackReleased = true;
            canCreatHotKeys = false;
            if (!SkillManager.instance.clone_Skill.crystalInseadOfClone)
                PlayerManager.instance.player.fx.MakeTransprent(true);
        }
        else
        {
            FinishBlackholeAbility();
        }
    }

    private void FinishBlackholeAbility()
    {
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;
        DestroyHotKeys();
    }

    private void DestroyHotKeys()
    {
        if (createdHodKey.Count <= 0)
            return;
        for (int i = 0; i < createdHodKey.Count; i++)
        {
            Destroy(createdHodKey[i]);
        }
        createdHodKey.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.FreezeTime(true);
            CreatHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) => collision.GetComponent<Enemy>()?.FreezeTime(false);

    private void CreatHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("热键列表中已经没有可用的热键了！");
            return;
        }
        if (!canCreatHotKeys) return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHodKey.Add(newHotKey);
        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);
        newHotKey.GetComponent<Blackhole_HotKey_Controller>().SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
