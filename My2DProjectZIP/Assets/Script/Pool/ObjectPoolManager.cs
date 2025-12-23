using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PoolType
{
    Damage,
    Dash,
    HitFX0,
    HitFX1,
}

[Serializable]
public class ObjectPool
{
    public string prefabName;
    public GameObject prefab;
    public int currenceCount;
    public PoolType type;
    [HideInInspector] public Transform Parent;
}


public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    [SerializeField] private List<ObjectPool> prefabsPool;
    private Dictionary<PoolType, Queue<GameObject>> dictionPoolObjects;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

        Invoke("NewFillPool", 1f);
    }

    private void NewFillPool()
    {
        dictionPoolObjects = new Dictionary<PoolType, Queue<GameObject>>();
        for (int i = 0; i < prefabsPool.Count; i++)
        {
            Transform poolParent = new GameObject($"[Pool]{prefabsPool[i].prefabName}").transform;
            poolParent.SetParent(transform);

            prefabsPool[i].Parent = poolParent;

            dictionPoolObjects.Add(prefabsPool[i].type, new Queue<GameObject>());
            FillPool(prefabsPool[i]);
        }
    }

    /// <summary>
    /// 从对象池中取对象
    /// </summary>
    /// <returns></returns>
    public GameObject GetFormTypePool(PoolType _type)
    {
        if (!dictionPoolObjects.ContainsKey(_type))
            return null;

        if (dictionPoolObjects[_type].Count <= 0)
        {
            foreach (ObjectPool item in prefabsPool)
            {
                if (item.type == _type)
                { 
                    FillPool(item);
                    break;
                }
            }
        }

        var pool = dictionPoolObjects[_type].Dequeue();
        pool.SetActive(true);
        return pool;
    }

    /// <summary>
    /// 扩容方法
    /// </summary>
    /// <param name="prefabsPool"></param>
    private void FillPool(ObjectPool prefabsPool)
    {
        for (int k = 0; k < prefabsPool.currenceCount; k++)
        {
            GameObject newShadow = Instantiate(prefabsPool.prefab);
            newShadow.transform.SetParent(prefabsPool.Parent);
            ReturnShadowPool(prefabsPool.type, newShadow);
        }
    }

    /// <summary>
    ///  返回对象池，根据类型
    /// </summary>
    /// <param name="type">返回的类型</param>
    /// <param name="newShadow">返回的对象</param>
    public void ReturnShadowPool(PoolType type, GameObject newShadow)
    {
        if (!dictionPoolObjects.ContainsKey(type))
            Destroy(newShadow, 0.5f);

        newShadow.SetActive(false);
        dictionPoolObjects[type].Enqueue(newShadow);
    }

}
