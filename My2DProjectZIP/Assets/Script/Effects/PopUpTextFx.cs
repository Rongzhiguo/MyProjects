using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextFx : MonoBehaviour
{
    [Header("出现动画")]
    public float appearDuration = 1.2f;        // 出现动画时长
    public float maxScale = 1.8f;             // 最大放大比例
    public Ease appearEase = Ease.OutBack;     // 出现缓动类型

    [Header("停留设置")]
    public float displayDuration = 2f;         // 停留时间

    [Header("消失动画")]
    public float disappearDuration = 1.5f;     // 消失动画时长
    public float moveDistance = 100f;          // 上移距离
    public Ease disappearEase = Ease.OutCubic; // 消失缓动类型

    private TextMeshPro _textComponent;
    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    private Color _originalColor;
    private Sequence _animationSequence;

    void Awake()
    {
        _textComponent = GetComponent<TextMeshPro>();
        _originalPosition = transform.localPosition;
        _originalScale = transform.localScale;
        _originalColor = _textComponent.color;

        // 初始状态设置为透明
        SetInitialState();
    }

    void OnDestroy()
    {
        // 清理DOTween序列
        _animationSequence?.Kill();
    }

    private void SetInitialState()
    {
        // 设置初始透明度和缩放
        _textComponent.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0);
        transform.localScale = Vector3.zero;
    }

    public void PlayAnimation(string message = null)
    {
        // 清理之前的动画
        _animationSequence?.Kill();

        // 重置位置
        transform.localPosition = _originalPosition;

        // 更新文本内容（如果提供了新消息）
        if (!string.IsNullOrEmpty(message))
        {
            _textComponent.text = message;
        }

        // 重置为初始状态
        SetInitialState();

        // 创建动画序列
        _animationSequence = DOTween.Sequence();

        // 第一部分：出现动画（慢慢变大）
        _animationSequence.Append(transform.DOScale(_originalScale * maxScale, appearDuration * 0.6f)
            .SetEase(appearEase));

        // 第二部分：回弹到原始大小
        _animationSequence.Append(transform.DOScale(_originalScale, appearDuration * 0.4f)
            .SetEase(Ease.OutBounce));

        // 同时进行淡入效果
        _animationSequence.Join(_textComponent.DOFade(1f, appearDuration * 0.5f));

        // 停留一段时间
        _animationSequence.AppendInterval(displayDuration);

        // 第三部分：上移并淡出
        _animationSequence.Append(transform.DOLocalMoveY(
                _originalPosition.y + moveDistance,
                disappearDuration)
            .SetEase(disappearEase));

        _animationSequence.Join(_textComponent.DOFade(0f, disappearDuration * 0.8f));

        // 同时缩小效果（可选）
        _animationSequence.Join(transform.DOScale(_originalScale * 0.7f, disappearDuration * 0.6f)
            .SetEase(Ease.InBack));

        // 动画完成后重置
        _animationSequence.OnComplete(() =>
        {
            transform.localScale = _originalScale;
            _textComponent.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0);
        });

        // 开始播放动画
        _animationSequence.Play();
    }
}
