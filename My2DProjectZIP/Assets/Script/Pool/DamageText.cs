using TMPro;
using DG.Tweening;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private float _floatHeight = 2f;
    [SerializeField] private float _duration = 1.5f;

    [Header("出现动画")]
    public float appearDuration = 1.2f;        // 出现动画时长
    public float maxScale = 1.8f;             // 最大放大比例
    public Ease appearEase = Ease.OutBack;     // 出现缓动类型
    private Vector3 _originalScale;

    private Sequence _animation;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Initialize(int damage, bool isCritical , Color _color)
    {
        _text.text = "-" + damage.ToString();
        _text.color = _color;
        _text.fontSize = isCritical ? 7 : 4;

        PlayAnimation();
    }

    private void PlayAnimation()
    {
        _animation?.Kill();

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * _floatHeight;

        _animation = DOTween.Sequence();
        // 第一部分：出现动画（慢慢变大）
        _animation.Append(transform.DOScale(_originalScale * maxScale, appearDuration * 0.6f)
            .SetEase(appearEase));
        // 第二部分：回弹到原始大小
        _animation.Append(transform.DOScale(_originalScale, appearDuration * 0.4f)
            .SetEase(Ease.OutBounce));
        _animation.Append(transform.DOMove(endPos, _duration).SetEase(Ease.OutQuad));
        _animation.Join(_text.DOFade(0, _duration * 0.8f).SetEase(Ease.InQuad));
        _animation.OnComplete(() => {
            _animation?.Kill();
            _text.color = Color.white; // 重置颜色
            _text.alpha = 1f; // 重置透明度

            //放回对象池
            ObjectPoolManager.instance.ReturnShadowPool(PoolType.Damage, gameObject);
        });
    }

}