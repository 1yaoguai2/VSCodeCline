using Unity.Entities;
using UnityEngine;

/// <summary>
/// 对象池配置Authoring组件
/// 用于在Unity编辑器中配置对象池
/// </summary>
public class ECSObjectPoolAuthoring : MonoBehaviour
{
    /// <summary>
    /// 预制体引用
    /// </summary>
    public GameObject Prefab;

    /// <summary>
    /// 对象池大小
    /// </summary>
    public int PoolSize = 10;
}

/// <summary>
/// 对象池Baker
/// 将MonoBehaviour配置转换为ECS组件
/// </summary>
public class ECSObjectPoolBaker : Baker<ECSObjectPoolAuthoring>
{
    public override void Bake(ECSObjectPoolAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        var prefabEntity = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new PooledObject
        {
            Prefab = prefabEntity,
            PoolSize = authoring.PoolSize
        });
    }
}
