using UnityEngine;

//场景单例，退出场景时会销毁，不设置DontDestroyOnLoad属性
public class SceneMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T s_instance;
    private static readonly object s_locker = new object();

    public static T Instance
    {
        get
        {
            lock (s_locker)
            {
                if (s_instance is null)
                {
                    T[] objs = FindObjectsOfType<T>();
                    if (objs.Length >= 1)
                    {
                        s_instance = objs[0];
                        //多余的删除
                        for (int i = 1; i < objs.Length; i++)
                        {
                            Destroy(objs[i]);
                        }
                    }

                    if (s_instance is null)
                    {
                        GameObject singleton = new GameObject();
                        s_instance = singleton.AddComponent<T>();
                        singleton.name = "[Singleton]" + typeof(T).ToString();
                    }
                }

                return s_instance;
            }
        }
    }

    protected void OnDestroy()
    {
        CleanUp();
        if (s_instance == (this as T))
        {
            s_instance = null;
        }
    }

    protected void Awake()
    {
        Initialize();
        if (s_instance == null)
        {
            s_instance = this as T;
        }
        else if (s_instance != this)
        {
            Debug.LogError($"在场景中存在多个单例[{typeof(T)}]");
            Destroy(this);
        }
    }

    //初始化放这里
    protected virtual void Initialize()
    {
    }

    //清理放这里
    protected virtual void CleanUp()
    {
    }

    //实例是否存在
    public static bool IsExisted => s_instance is not null;
}