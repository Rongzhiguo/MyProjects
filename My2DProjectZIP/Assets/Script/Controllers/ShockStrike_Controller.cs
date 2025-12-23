using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private float speed;
    private CharacterStats targetStats;
    private int damage;

    private Animator anim;
    private bool triggered;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }


    // Update is called once per frame
    void Update()
    {


        if (triggered || !targetStats)
            return;
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        if (Vector2.Distance(transform.position, targetStats.transform.position) < .1f)
        {
            anim.transform.localPosition = new Vector3(0, 0.5f);
            anim.transform.localRotation = Quaternion.identity;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            triggered = true;
            anim.SetTrigger("Hit");
            Invoke("DamageAndSelfDestroy", .2f);
        }
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShoke(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }
}
