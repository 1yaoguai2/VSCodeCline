using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity对象池管理系统
/// </summary>
public class ObjectPool : MonoBehaviour
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static ObjectPool Instance;

    /// <summary>
    /// 对象池配置类
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        public string tag;       // 对象标识
        public GameObject prefab; // 预制体
        public int size;         // 初始数量
    }

    public List<Pool> pools; // 对象池配置列表
    public Dictionary<string, Queue<GameObject>> poolDictionary; // 对象池字典

    private void Awake()
    {
        // 初始化单例
        Instance = this;
    }

    void Start()
    {
        // 初始化对象池字典
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // 遍历所有配置池
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // 初始化指定数量的对象
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // 初始状态为未激活
                objectPool.Enqueue(obj);
            }

            // 将对象池加入字典
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary>
    /// 从对象池生成对象
    /// </summary>
    /// <param name="tag">对象标识</param>
    /// <param name="position">生成位置</param>
    /// <param name="rotation">生成旋转</param>
    /// <returns>生成的对象</returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // 检查对象池是否存在
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("对象池 " + tag + " 不存在");
            return null;
        }

        // 从队列中取出对象
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // 激活并设置对象属性
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // 调用对象的初始化方法
        if (objectToSpawn.TryGetComponent<IPooledObject>(out var pooledObj))
        {
            pooledObj.OnObjectSpawn();
        }

        // 将对象重新加入队列
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}

/// <summary>
/// 可池化对象接口
/// </summary>
public interface IPooledObject
{
    /// <summary>
    /// 对象生成时调用
    /// </summary>
    void OnObjectSpawn();
}
