using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    private SpriteRenderer thisSprite;
    private player _player;

    [Header("显示时间")]
    [SerializeField] private float activeTime;
    private float activeStart;

    [Header("不透明度控制")]
    private float alpha;
    [SerializeField][Range(0, 1)] private float alphaSet;
    [SerializeField][Range(0, .99f)] private float alphaMultiplier;


    private void OnEnable()
    {
        thisSprite = GetComponent<SpriteRenderer>();
        _player = PlayerManager.instance.player;

        thisSprite.sprite = _player.sr.sprite;

        alpha = alphaSet;

        transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y + .2f);
        transform.localScale = _player.transform.localScale;
        transform.rotation = _player.transform.rotation;

        activeStart = Time.time;

    }


    // Update is called once per frame
    void Update()
    {
        alpha *= alphaMultiplier;

        thisSprite.color = new Color(thisSprite.color.r, thisSprite.color.g, thisSprite.color.b, alpha);

        if (Time.time >= activeStart + activeTime)
        {
            ObjectPoolManager.instance.ReturnShadowPool(PoolType.Dash, gameObject);
        }
    }
}
