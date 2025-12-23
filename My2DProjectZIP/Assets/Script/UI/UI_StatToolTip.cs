using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StatToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI desc;
    public void ShowStatToolTip(string _text)
    {
        desc.text = _text;
        gameObject.SetActive(true);
    }

    public void HideStatToolTip()
    {
        desc.text = "";
        gameObject.SetActive(false);
    }
}
