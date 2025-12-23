using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager : SingletonMono<ResManager>
{
    /// <summary>
    /// 同步加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">需要加载的对象名</param>
    /// <returns></returns>
    public T LoadRes<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)
            return (T)Instantiate(res);
        else
            return res;
    }

    public void LoadResAsync<T>(string name, System.Action<T> callback) where T : Object
    {
        StartCoroutine(LoadResIE(name, callback));
    }

    private IEnumerator LoadResIE<T>(string name, System.Action<T> callback) where T : Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(name);
        yield return request;

        if (request.asset is GameObject)
            callback(Instantiate(request.asset) as T);
        else
            callback(request.asset as T);
    }
}
