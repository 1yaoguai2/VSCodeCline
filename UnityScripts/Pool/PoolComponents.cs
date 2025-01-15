using Unity.Entities;

/// <summary>
/// 池化对象标记组件
/// 用于标识一个实体是否为池化对象
/// </summary>
public struct PooledObjectTag : IComponentData {}

/// <summary>
/// 池化对象配置组件
/// 存储预制体引用和池大小
/// </summary>
public struct PooledObject : IComponentData
{
    /// <summary>
    /// 预制体实体引用
    /// </summary>
    public Entity Prefab;

    /// <summary>
    /// 对象池大小
    /// </summary>
    public int PoolSize;
}

/// <summary>
/// 池化对象实例组件
/// 记录实例与预制体的关系
/// </summary>
public struct PooledObjectInstance : IComponentData
{
    /// <summary>
    /// 对应的预制体实体
    /// </summary>
    public Entity PrefabEntity;
}
