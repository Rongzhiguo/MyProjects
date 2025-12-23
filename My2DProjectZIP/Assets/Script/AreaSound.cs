using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{

    [SerializeField][Tooltip("区域内需要播放的音效下标")] private int areaSoundIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<player>() != null)
        {
            AudioManager.instance?.StopCorountineSFX(areaSoundIndex);
            AudioManager.instance?.PlaySFX(areaSoundIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<player>() != null)
        {
            AudioManager.instance?.StopSFXWithTime(areaSoundIndex);
        }
    }
}
