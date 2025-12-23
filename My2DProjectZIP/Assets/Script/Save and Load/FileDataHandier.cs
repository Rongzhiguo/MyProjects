using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandier
{
    /// <summary>
    /// 数据存储路径
    /// </summary>
    private string dataDirPath = "";

    /// <summary>
    /// 数据存储文件名
    /// </summary>
    private string dataFileName = "";

    /// <summary>
    /// 是否加密数据
    /// </summary>
    private bool encryptData = false;

    /// <summary>
    /// 秘钥
    /// </summary>
    private string codeWord = "1c55118eebbae293";

    public FileDataHandier(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        this.dataDirPath = _dataDirPath;
        this.dataFileName = _dataFileName;
        this.encryptData = _encryptData;
    }

    /// <summary>
    /// 将GameData数据用json形式存本地
    /// </summary>
    /// <param name="_data"></param>
    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(_data, true);

            if (encryptData)
                dataToStore = EncryptDecrypt(dataToStore);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("保存文件时出错，路径为：" + fullPath + "\n" + e);
            throw;
        }
    }

    /// <summary>
    /// 将本地的json文件经过转化后变成GameData数据让其读取数据
    /// </summary>
    /// <returns></returns>
    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("读取文件异常，路径为：" + fullPath + "\n" + e);
                throw;
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    /// <summary>
    /// 加密 and 解密
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";

        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }
}
