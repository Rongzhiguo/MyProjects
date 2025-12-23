using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolManager : SingletonMono<PoolManager>
{
    //对象池管理字典
    private Dictionary<string, List<GameObject>> poolDitc = new();

    [SerializeField][Tooltip("总父节点")] private Transform allParent;

    private GameObject goParent;

    public void GetObj(string name, UnityAction<GameObject> objAction)
    {
        GameObject go;
        if (poolDitc.ContainsKey(name) && poolDitc[name].Count > 0)
        {
            go = poolDitc[name][0];
            poolDitc[name].Remove(go);
            go.SetActive(true);
            SetParent(go);
            objAction(go);
        }
        else
        {
            ResManager.instance.LoadResAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                o.SetActive(true);
                SetParent(o);
                objAction(o);
            });
        }
    }


    private void SetParent(GameObject go)
    {

        //临时节点对总父节点
        if (!(goParent != null && goParent.name == go.name + "Father"))
        {
            //创建一个临时父节点
            goParent = Instantiate(new GameObject(), allParent.position, Quaternion.identity);

            goParent.name = go.name + "Father";

            goParent.transform.SetParent(allParent);

        }

        //创建对象对临时节点
        go.transform.SetParent(goParent.transform);

    }
    public void PushObj(GameObject obj, bool isDestroy = false)
    {

        string name = obj.name;
        if (isDestroy)
        {
            Destroy(obj);
            return;
        }
        //如果字典里面没有这个对象，就创建一个新的列表
        if (!poolDitc.ContainsKey(name))
        {
            poolDitc.Add(name, new List<GameObject>() { obj });
        }

        //如果列表里面有这个对象，就放回去
        if (poolDitc.ContainsKey(name))
        {
            poolDitc[name].Add(obj);
        }

        obj.SetActive(false);
        SetParent(obj);
    }
}
