using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

/// <summary>
/// 对象池管理系统
/// 负责创建和管理对象池
/// </summary>
public partial struct ECSObjectPoolSystem : ISystem
{
    private EntityQuery _poolQuery;
    
    public void OnCreate(ref SystemState state)
    {
        _poolQuery = state.GetEntityQuery(ComponentType.ReadOnly<PooledObject>());
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var pool in SystemAPI.Query<RefRO<PooledObject>>())
        {
            var prefab = pool.ValueRO.Prefab;
            var poolSize = pool.ValueRO.PoolSize;
            
            for (int i = 0; i < poolSize; i++)
            {
                var entity = ecb.Instantiate(prefab);
                ecb.AddComponent(entity, new PooledObjectInstance
                {
                    PrefabEntity = prefab
                });
                ecb.AddComponent<PooledObjectTag>(entity);
                ecb.AddComponent<LocalTransform>(entity);
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

/// <summary>
/// 对象生成系统
/// 负责激活和初始化池化对象
/// </summary>
public partial struct ObjectSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (transform, entity) in 
            SystemAPI.Query<RefRW<LocalTransform>>()
                .WithEntityAccess()
                .WithAll<PooledObjectTag>())
        {
            ecb.SetComponent(entity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = transform.ValueRO.Scale
            });
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
