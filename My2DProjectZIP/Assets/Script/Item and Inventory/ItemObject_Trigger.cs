using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    [SerializeField] private ItemObject myItemObject => GetComponentInParent<ItemObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player>() != null)
        {
            if (collision.GetComponent<CharacterStats>().isDead)
                return;
            myItemObject.PickupItem();
        }
    }
}
