using UnityEngine;
using System;

// 改进的场景单例模式，使用Lazy<T>实现线程安全
public class ImprovedSceneMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    //使用Lazy实现线程安全的延迟初始化，无需显式锁机制
    private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateSingleton);
    //添加isApplicationQuitting标志，正确处理应用退出时的实例管理
    private static bool isApplicationQuitting = false;

    public static T Instance
    {
        get
        {
            if (isApplicationQuitting)
            {
                Debug.LogWarning($"[单例] 实例 '{typeof(T)}' 已被销毁，返回null");
                return null;
            }
            return LazyInstance.Value;
        }
    }

    private static T CreateSingleton()
    {
        var existingInstance = FindObjectOfType<T>();
        if (existingInstance != null)
        {
            return existingInstance;
        }

        var singletonObject = new GameObject($"[Singleton] {typeof(T)}");
        var instance = singletonObject.AddComponent<T>();
        DontDestroyOnLoad(singletonObject);
        return instance;
    }

    protected virtual void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (this == Instance)
        {
            isApplicationQuitting = true;
        }
    }

    protected virtual void Awake()
    {
        if (Instance != this)
        {
            Debug.LogWarning($"检测到单例 '{typeof(T)}' 的多个实例，正在销毁重复实例");
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    protected virtual void Initialize() { }

    public static bool IsExisted => Instance is not null;
}
