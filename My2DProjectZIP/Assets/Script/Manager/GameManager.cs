using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMono<GameManager>, ISaveManager
{
    //public static GameManager instance;
    private Transform player;
    [SerializeField] private CheckPoint[] checkPoints;
    [SerializeField] private string closestCheckpoint;

    [Header("损失的货币")]
    [SerializeField][Tooltip("需要创建的死亡损失预制体")] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    //private void Awake()
    //{
    //    if (instance == null)
    //        instance = this;
    //    else
    //        Destroy(instance.gameObject);

    //    checkPoints = FindObjectsOfType<CheckPoint>();
    //}

    protected override void OnSingletonInitialized()
    {
        base.OnSingletonInitialized();
        checkPoints = FindObjectsOfType<CheckPoint>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player.transform;
    }

    /// <summary>
    /// 重新加载场景（一般在玩家死亡点击重新开始按钮调用方法）
    /// </summary>
    public void RestarScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data)
    {
        LoadCheckPoints(_data);
        LoadClosestCheckPoint(_data);
        LoadLostCurrency(_data);
    }

    /// <summary>
    /// 检查所有的监测点，并判断哪些已经激活，然后设置为激活状态
    /// </summary>
    /// <param name="_data"></param>
    private void LoadCheckPoints(GameData _data)
    {

        foreach (KeyValuePair<string, bool> pair in _data.checkPoints)
        {
            foreach (var checkPoint in checkPoints)
            {
                if (checkPoint.ID == pair.Key)
                {
                    if (pair.Value)
                        checkPoint.ActiveCheckPoint();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 加载损失点的货币数量和位置，并创建预制体
    /// </summary>
    /// <param name="_data"></param>
    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY + .1f), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    /// <summary>
    /// 将玩家的坐标移动至最进的检查点
    /// </summary>
    /// <param name="_data"></param>
    private void LoadClosestCheckPoint(GameData _data)
    {
        if (string.IsNullOrEmpty(_data.closeCheckPointID))
            return;
        closestCheckpoint = _data.closeCheckPointID;
        foreach (var checkPoint in checkPoints)
        {
            if (checkPoint.ID == closestCheckpoint)
            {
                player.position = checkPoint.transform.position;
                break;
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        SaveLostCurrency(_data);

        _data.checkPoints.Clear();
        foreach (var checkPoint in checkPoints)
        {
            _data.checkPoints.Add(checkPoint.ID, checkPoint.checkPointActive);
        }
        _data.closeCheckPointID = FindCloseCheckPoint()?.ID;
    }

    /// <summary>
    /// 保存损失点的货币数量和位置
    /// </summary>
    /// <param name="_data"></param>
    private void SaveLostCurrency(GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;
    }

    private CheckPoint FindCloseCheckPoint()
    {
        float closeDistance = Mathf.Infinity;

        CheckPoint closeCheckPoint = null;

        foreach (var checkPoint in checkPoints)
        {
            float disToCheckPoint = Vector2.Distance(player.position, checkPoint.transform.position);
            if (disToCheckPoint < closeDistance && checkPoint.checkPointActive)
            {
                closeDistance = disToCheckPoint;
                closeCheckPoint = checkPoint;
            }
        }

        return closeCheckPoint;
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame(bool _pause) => Time.timeScale = _pause ? 1 : 0;

}
