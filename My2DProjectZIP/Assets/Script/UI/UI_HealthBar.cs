using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform rectTransform;
    private Slider slider;
    [SerializeField] private Image healthBG;
    [SerializeField] private float buffTime;
    private float OldSliderValue;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponentInParent<Entity>();
        myStats = GetComponentInParent<CharacterStats>();
        rectTransform = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealhtUI;
        UpdateHealhtUI();
    }

    private void UpdateHealhtUI()
    {
        slider.maxValue = myStats.GetMaxHealthValueHP();
        slider.value = myStats.currentHealth;

        if (healthBG == null)
            return;
        var currentHP = slider.value / slider.maxValue;
        if (OldSliderValue != 0)
        {
            if (currentHP < OldSliderValue)
            {
                //需要动态改变healthBG
                StartCoroutine(UpdataHpEffect(currentHP, OldSliderValue));
            }
            else
            {
                OldSliderValue = currentHP;
            }
        }
        else
        {
            OldSliderValue = currentHP;
            healthBG.fillAmount = currentHP;
        }
    }

    IEnumerator UpdataHpEffect(float newHP, float oldHP)
    {
        float elapsdTime = 0;
        while (elapsdTime < buffTime)
        {
            elapsdTime += Time.deltaTime;
            healthBG.fillAmount = Mathf.Lerp(oldHP, newHP, elapsdTime / buffTime);
            yield return null;
        }

        healthBG.fillAmount = slider.value / slider.maxValue;
        OldSliderValue = slider.value / slider.maxValue;
    }

    private void FlipUI() => rectTransform.Rotate(0, 180, 0);

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdateHealhtUI;
    }
}
