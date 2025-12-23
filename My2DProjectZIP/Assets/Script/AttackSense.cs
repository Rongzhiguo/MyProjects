using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSense : MonoBehaviour
{
    public static AttackSense instance;

    [SerializeField] private float globleShakeForce = 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    public void HitPause(int duration) => StartCoroutine(Pause(duration));

    /// <summary>
    /// 控制游戏时间流逝速度
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    IEnumerator Pause(int _duration)
    {
        float pauseTime = _duration / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }

}
