using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected player _player;

    public Transform messageTransform;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private GameObject entityStatusUI;
    private Material originalMat;
    private Color currentColor;

    [Header("状态颜色")]
    [SerializeField] private Color chillColor;  //冰冻颜色
    [SerializeField] private Color[] igniteColor; //点燃颜色
    [SerializeField] private Color[] shockColor; //闪电颜色

    [Header("特效粒子状态")]
    [SerializeField] private ParticleSystem igniteFX;
    [SerializeField] private ParticleSystem chillFX;
    [SerializeField] private ParticleSystem shockFX;

    [Header("打到目标后目标产生的受击效果")]
    [SerializeField] private PoolType hitFXType;
    [SerializeField] private GameObject hitFXPrefab;
    [SerializeField][Tooltip("X的缩放是否根据方向判断")] private bool critical;



    protected virtual void Start()
    {
        sr = transform.Find("Animator").GetComponent<SpriteRenderer>();

        originalMat = sr.material;
        currentColor = sr.color;
        _player = PlayerManager.instance.player;
    }

    public void ShowPopUpTextFx(string message)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1.5f, 3);
        var positionOffset = new Vector3(randomX, randomY, 0);

        messageTransform.GetComponent<PopUpTextFx>().PlayAnimation(message);
        messageTransform.transform.position = transform.position + positionOffset;
    }

    /// <summary>
    /// 对象精灵是否显示
    /// </summary>
    /// <param name="_transprent">true不显示 false 显示</param>
    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;

        entityStatusUI.SetActive(!_transprent);
    }

    IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(.2f);
        sr.color = currentColor;
        sr.material = originalMat;
    }

    void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    void CancelColorChange()
    {
        CancelInvoke();
        sr.color = currentColor;

        igniteFX.Stop();
        chillFX.Stop();
        shockFX.Stop();
    }

    public void ChillFxFor(float _seconds)
    {
        chillFX.Play();
        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void IgniteFxFor(float _seconds)
    {
        igniteFX.Play();
        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        shockFX.Play();
        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }

    private void ChillColorFx()
    {
        sr.color = chillColor;
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    /// <summary>
    /// 创建受击动画
    /// </summary>
    public void CreatHitFX(Transform _target)
    {
        if (hitFXPrefab == null)
            return;

        GameObject newHitFX;
        if (_target.GetComponent<EntityFX>().critical)
        {

            //newHitFX = Instantiate(hitFXPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity);
            newHitFX = ObjectPoolManager.instance.GetFormTypePool(hitFXType);
            if (newHitFX != null)
            {
                float zRotation = Random.Range(-90, 90);
                float xPosition = Random.Range(-.3f, .3f);
                float yPosition = Random.Range(-.3f, .3f);
                newHitFX.transform.position = _target.position + new Vector3(xPosition, yPosition);
                newHitFX.transform.Rotate(new Vector3(0, 0, zRotation));
            }
        }
        else
        {
            //newHitFX = Instantiate(hitFXPrefab, _target.position + new Vector3(0, -0.7f), Quaternion.identity);
            newHitFX = ObjectPoolManager.instance.GetFormTypePool(hitFXType);
            if (newHitFX != null)
            {
                newHitFX.transform.position = _target.position + new Vector3(0, -0.7f);
                float zRotation = Random.Range(15, 30);
                newHitFX.transform.Rotate(new Vector3(0, 0, zRotation));
                newHitFX.transform.localScale = new Vector3(GetComponent<Entity>().facinDir, 1, 1);
            }
        }

        //Destroy(newHitFX, 0.5f);
        StartCoroutine(ReturnPool(newHitFX, 0.5f));
    }

    IEnumerator ReturnPool(GameObject _newHitFX , float _seconds)
    {
        yield return new WaitForSecondsRealtime(_seconds);
        ObjectPoolManager.instance.ReturnShadowPool(hitFXType, _newHitFX);
    }

}
