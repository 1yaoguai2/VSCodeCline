using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PoolTest : MonoBehaviour
{
    public GameObject Prefab;
    public int PoolSize = 10;

    private World _world;
    private EntityManager _entityManager;

    void Start()
    {
        // 获取默认世界和EntityManager
        _world = World.DefaultGameObjectInjectionWorld;
        _entityManager = _world.EntityManager;

        // 创建对象池
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var poolEntity = ecb.CreateEntity();
        ecb.AddComponent(poolEntity, new PooledObject
        {
            Prefab = ecb.CreateEntity(),
            PoolSize = PoolSize
        });
        
        ecb.Playback(_entityManager);
        ecb.Dispose();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        // 获取对象池系统
        var poolSystem = _world.GetExistingSystem<ECSObjectPoolSystem>();
        var spawnSystem = _world.GetExistingSystem<ObjectSpawnSystem>();

        // 生成对象
        var query = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<PooledObjectTag>());
        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        if (entities.Length > 0)
        {
            var randomPos = new float3(
                UnityEngine.Random.Range(-5f, 5f),
                0,
                UnityEngine.Random.Range(-5f, 5f)
            );

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            var entity = entities[0];
            ecb.SetComponent(entity, new LocalTransform
            {
                Position = randomPos,
                Rotation = quaternion.identity,
                Scale = 1
            });
            
            ecb.Playback(_entityManager);
            ecb.Dispose();

            // 触发对象生成
            spawnSystem.Update(_world.Unmanaged);
        }

        entities.Dispose();
    }
}
