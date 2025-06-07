using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class ExampleAgent : BaseBatchAgent
{
    public float moveSpeed;
    public Vector2 min;
    public Vector2 max;
    public NativeArray<float3> positions;
    public NativeArray<float3> targets;
    public NativeArray<Random> randoms;

    public void Initialize(int count)
    {
        positions = new NativeArray<float3>(count, Allocator.Persistent);
        targets = new NativeArray<float3>(count, Allocator.Persistent);
        randoms = new NativeArray<Random>(count, Allocator.Persistent);
        for (int i = 0; i < count; i++)
        {
            randoms[i] = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
        }
        for (int i = 0; i < count; i++)
        {
            visibleInstances[i] = true;
        }
        SetInstanceCount(count);
    }

    public void SetInstanceCount(int count)
    {
        this.instanceCountRef.Value = count;
    }

    public void Update()
    {
        new MoveJob
        {
            positions = positions,
            targets = targets,
            randoms = randoms,
            min = min,
            max = max,
            moveSpeed = moveSpeed,
            buffer = buffer,
            deltaTime = Time.deltaTime
        }.Schedule(instanceCountRef.Value, 64).Complete();
    }

    public override void Dispose()
    {
        positions.Dispose();
        targets.Dispose();
        randoms.Dispose();
    }

    [BurstCompile]
    struct MoveJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] public NativeArray<float3> positions;
        [NativeDisableParallelForRestriction] public NativeArray<float3> targets;
        [NativeDisableParallelForRestriction] public NativeArray<Random> randoms;
        [NativeDisableParallelForRestriction] public NativeArray<float4> buffer;

        public Vector2 min;
        public Vector2 max;
        public float moveSpeed;
        public float deltaTime;

        public void Execute(int i)
        {
            var pos = positions[i];
            var target = targets[i];
            var random = randoms[i];

            float3 dir = target - pos;
            float dist = math.length(dir);

            if (dist < 0.05f)
            {
                targets[i] = new float3(random.NextFloat(min.x, max.x), random.NextFloat(min.y, max.y), 0);
                randoms[i] = random;
            }
            else
            {
                float3 move = math.normalize(dir) * moveSpeed * deltaTime;
                if (math.length(move) > dist) move = dir;
                pos += move;
            }

            positions[i] = pos;
            SetTRS(i, pos, quaternion.identity, 1);
        }

        [BurstCompile]
        public void SetTRS(int index, float3 pos, quaternion rot, float scale)
        {
            var rotMat = math.mul(new float3x3(rot), float3x3.Scale(scale));
            buffer[index * 3 + 0] = new float4(rotMat.c0.x, rotMat.c0.y, rotMat.c0.z, rotMat.c1.x);
            buffer[index * 3 + 1] = new float4(rotMat.c1.y, rotMat.c1.z, rotMat.c2.x, rotMat.c2.y);
            buffer[index * 3 + 2] = new float4(rotMat.c2.z, pos.x, pos.y, pos.z);
        }
    }
}
