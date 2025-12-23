using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "Main";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UI_FadeScene fadeScene;

    AsyncOperation async;

    private void Start()
    {
        continueButton.SetActive(SaveManager.instance.HasSavedData());
    }

    /// <summary>
    /// 继续（开始）游戏按钮
    /// </summary>
    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 加载场景的延时方法，用于展现淡入淡出效果
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScene.FadeOut();
        yield return new WaitForSeconds(_delay);
        //SceneManager.LoadScene(sceneName);
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = true;
    }
}
