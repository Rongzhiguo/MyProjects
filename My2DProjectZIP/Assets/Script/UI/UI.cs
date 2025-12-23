using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UI : MonoBehaviour, ISaveManager
{
    [Header("结束场景")]
    [SerializeField] private UI_FadeScene fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]
    [SerializeField] private GameObject charcaterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_CraftWindow craftWindow;
    public UI_SkillToolTip skillToolTip;
    public UI_VolumeSlider[] volumeSetting;


    public Transform craftListButton;

    // Start is called before the first frame update
    private void Awake()
    {
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        SwitchTo(inGameUI);
        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(charcaterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);

        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);

        if (Input.GetKeyDown(KeyCode.Escape))
            SwitchTo(inGameUI);
    }

    public void SwitchTo(GameObject _menu)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScene>() != null;
            if (!fadeScreen)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);

        if (_menu != null)
        {
            //切换页签的时候 也需要播放声音
            //AudioManager.instance.PlaySFX(?)
            _menu.SetActive(true);
        }

        InitMenuData(_menu);

        if (GameManager.instance != null)
            GameManager.instance.PauseGame(_menu == inGameUI);
    }

    /// <summary>
    /// 初始化材料页签的合成相关数据
    /// </summary>
    /// <param name="_menu"></param>
    private void InitMenuData(GameObject _menu)
    {
        //如果是材料背包，则需要选择左侧合成按钮中的第一个进行数据初始化
        if (_menu == craftUI)
        {
            craftListButton.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();
            craftListButton.GetChild(0).GetComponent<UI_CraftList>().SetupDefaultCraftWindow();
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }
        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScene>() == null)
            {
                return;
            }
        }

        inGameUI.SetActive(true);
        if (GameManager.instance != null)
            GameManager.instance.PauseGame(true);
    }

    public void SwitchOnEndScreen()
    {
        //SwitchTo(null);
        fadeScreen.FadeOut();
        StartCoroutine(EndScreen());
    }

    IEnumerator EndScreen()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.RestarScene();

    #region 装备背包翻页效果

    public void UpPage()
    {
        Inventory.instance.GetCurrentPage(-1);
    }

    public void DownPage()
    {
        Inventory.instance.GetCurrentPage(1);
    }

    #endregion

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string, float> pair in _data.volumeSettings)
        {
            foreach (var item in volumeSetting)
            {
                if (pair.Key == item.parametr)
                {
                    item.LoadSlider(pair.Value);
                    break;
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach (var item in volumeSetting)
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);
        }
    }
}
