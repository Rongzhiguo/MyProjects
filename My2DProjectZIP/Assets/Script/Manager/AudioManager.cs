using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMono<AudioManager>
{
    //public static AudioManager instance;

    [SerializeField][Tooltip("音乐播放的有效距离")] private float sfxMinDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    [Tooltip("是否播放背景音乐")] public bool playBgm;
    private int bgmIndex;  //正在播放的BGM下标位置

    private bool canPlaySFX;
    private Coroutine stopSFXWithTimeCoroutine;

    protected override void OnSingletonInitialized()
    {
        base.OnSingletonInitialized();
        Invoke("AllowSFX", 2);
    }

    private void Update()
    {
        if (!playBgm)
            StopAllBGM();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
                PlayBGM(bgmIndex);
        }
    }

    public void PlaySFX(int _sfxIndex, Transform _source = null)
    {
        //if (sfx[_sfxIndex].isPlaying)
        //    return;


        if (!canPlaySFX)
            return;

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinDistance)
            return;


        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(.7f, 1.2f);
            sfx[_sfxIndex].Play();
        }
    }


    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="_index"></param>
    public void StopSFX(int _index)
    {
        if (_index > sfx.Length)
        {
            Debug.Log("SFX音频下标越界！");
            return;
        }
        sfx[_index].Stop();
    }

    /// <summary>
    /// 开始播放SFX之前检查是否有渐出音效协程
    /// </summary>
    /// <param name="_index"></param>
    public void StopCorountineSFX(int _index)
    {
        if (stopSFXWithTimeCoroutine != null)
        {
            StopCoroutine(stopSFXWithTimeCoroutine);
            sfx[_index].volume = 1;
        }
    }

    /// <summary>
    /// 渐出停止音效
    /// </summary>
    /// <param name="_index"></param>
    public void StopSFXWithTime(int _index)
    {
        if (_index > sfx.Length)
        {
            Debug.Log("SFX音频下标越界！");
            return;
        }

        if (instance)
            stopSFXWithTimeCoroutine = StartCoroutine(DecreaseVolume(sfx[_index]));
    }

    /// <summary>
    /// 降低音量
    /// </summary>
    /// <returns></returns>
    IEnumerator DecreaseVolume(AudioSource _audio)
    {
        float defVolume = _audio.volume;
        while (_audio.volume > .1f)
        {
            _audio.volume -= _audio.volume * .2f;
            yield return new WaitForSeconds(.3f);

            if (_audio.volume <= .1f)
            {
                _audio.Stop();
                _audio.volume = defVolume;
                break;
            }
        }
    }



    public void PlayBGM(int _bgmIndex)
    {
        bgmIndex = _bgmIndex;

        StopAllBGM();
        bgm[bgmIndex].Play();
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    /// <summary>
    /// 是否可以播放SFX  用于游戏初始化时不播放sfx音效
    /// </summary>
    private void AllowSFX() => canPlaySFX = true;


    private void OnDestroy()
    {
        if (stopSFXWithTimeCoroutine != null)
            StopCoroutine(stopSFXWithTimeCoroutine);
    }
}
