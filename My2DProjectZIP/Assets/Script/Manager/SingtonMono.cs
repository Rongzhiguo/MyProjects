using UnityEngine;
using System;

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _isApplicationQuitting = false;

    // 线程安全的单例访问器
    public static T instance
    {
        get
        {
            // 应用程序退出时返回null
            if (_isApplicationQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            // 线程安全锁
            lock (_lock)
            {
                if (_instance == null)
                {
                    // 1. 查找场景中现有实例
                    _instance = FindFirstObjectByType<T>();

                    // 2. 创建新实例（如果不存在）
                    if (_instance == null)
                    {
                        // 创建新GameObject
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T).Name} (Singleton)";

                        // 标记为跨场景不销毁
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log($"[Singleton] Created new instance of {typeof(T)}");
                    }
                    //else
                    //{
                    //    // 确保现有实例持久化
                    //    DontDestroyOnLoad(_instance.gameObject);
                    //}
                }
                return _instance;
            }
        }
    }

    // 安全的Awake实现
    protected virtual void Awake()
    {
        // 防止重复实例
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] Multiple instances detected. Destroying extra instance of {typeof(T)}");
            Destroy(gameObject);
            return;
        }

        // 设置实例引用
        _instance = this as T;

        // 标记为跨场景不销毁
        //if (transform.parent == null)
        //    DontDestroyOnLoad(gameObject);

        // 自定义初始化
        OnSingletonInitialized();
    }

    // 可选的初始化方法
    protected virtual void OnSingletonInitialized()
    {
        // 子类可重写此方法进行初始化
    }

    // 应用程序退出处理
    protected virtual void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    // 对象销毁处理
    protected virtual void OnDestroy()
    {
        // 确保只清除当前实例的引用
        if (_instance == this)
        {
            _instance = null;
            Debug.Log($"[Singleton] Instance of {typeof(T)} destroyed");
        }
    }

    // 编辑器辅助：在Inspector中显示警告
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        // 防止在编辑器模式下预置单例
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
        {
            Debug.LogError($"[Singleton] {typeof(T)} should not be added to prefabs directly. It will be created at runtime.");
            UnityEditor.EditorApplication.delayCall += () => {
                if (this != null) DestroyImmediate(this);
            };
        }
    }
#endif
}