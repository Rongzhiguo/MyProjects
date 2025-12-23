using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("相机抖动")]
    public float shakeMultiplier;
    [Tooltip("剑返回时候抖动效果")] public Vector3 swordShakePwoer;
    [Tooltip("对象受到重击抖动效果")] public Vector3 shakePwoerDamage;
    private CinemachineImpulseSource screenShake;

    [Space]
    [SerializeField] private ParticleSystem dustFX;

    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    public void ScreenSkake(Vector3 _shakePwoer)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePwoer.x * _player.facinDir, _shakePwoer.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void PlayDustFX()
    {
        if (dustFX != null)
            dustFX.Play();
    }
}
