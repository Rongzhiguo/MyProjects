using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : SingletonMono<SaveManager>
{
    //public static SaveManager instance;

    [SerializeField] private string filePath = "idbfs/Rong1477603306";
    [SerializeField] private string fileName;
    [SerializeField][Tooltip("是否加密")] private bool encryptData;

    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandier dataHandier;

    [ContextMenu("删除游戏缓存数据文件")]
    public void DeleteSaveData()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            dataHandier = new FileDataHandier(filePath, fileName, encryptData);
        else
            dataHandier = new FileDataHandier(Application.persistentDataPath, fileName, encryptData);
        dataHandier.Delete();
    }

    protected override void OnSingletonInitialized()
    {
        base.OnSingletonInitialized();
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            dataHandier = new FileDataHandier(filePath, fileName, encryptData);
        else
            dataHandier = new FileDataHandier(Application.persistentDataPath, fileName, encryptData);
        saveManagers = FindAllSaveManagers();
    }

    //private void Awake()
    //{
    //    if (instance != null)
    //        Destroy(instance.gameObject);
    //    else
    //        instance = this;

    //    if (Application.platform == RuntimePlatform.WebGLPlayer)
    //        dataHandier = new FileDataHandier(filePath, fileName, encryptData);
    //    else
    //        dataHandier = new FileDataHandier(Application.persistentDataPath, fileName, encryptData);
    //    saveManagers = FindAllSaveManagers();
    //}

    private void Start()
    {
        LoadGame();
    }

    /// <summary>
    /// 新的游戏
    /// </summary>
    public void NewGame()
    {
        gameData = new GameData();
    }

    /// <summary>
    /// 加载存档游戏
    /// </summary>
    public void LoadGame()
    {
        //从本地读取缓存数据，并加载到游戏
        gameData = dataHandier.Load();

        if (gameData == null)
        {
            NewGame();
        }

        foreach (var saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }


    }

    /// <summary>
    /// 保存游戏数据
    /// </summary>
    public void SaveGame()
    {
        foreach (var saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandier.Save(gameData);
    }


    private void OnApplicationQuit()
    {
        SaveGame();
    }

    /// <summary>
    /// 查找所有对象中继承ISaveManager对象并添加至List里
    /// </summary>
    /// <returns></returns>
    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();
        return new List<ISaveManager>(saveManagers);
    }


    public bool HasSavedData()
    {
        if (dataHandier.Load() != null)
            return true;
        return false;
    }
}
