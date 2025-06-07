using UnityEngine;

public class Examples : MonoBehaviour
{
    public int instanceCount = 4000;
    public float moveSpeed;

    public Vector2 min;
    public Vector2 max;

    public Mesh mesh;
    public Material material;
    private ExampleAgent batchBulletAgent;
    private void Awake()
    {
        Application.targetFrameRate = 99999;
        batchBulletAgent = BRGSystem.CreateBatchAgent<ExampleAgent>(mesh, material, 10000);
        batchBulletAgent.moveSpeed = moveSpeed;
        batchBulletAgent.min = min;
        batchBulletAgent.max = max;
        batchBulletAgent.Initialize(instanceCount);
    }
    private void FixedUpdate()
    {
        batchBulletAgent.Update();
    }

    private void OnDestroy()
    {
        batchBulletAgent.Dispose();
    }
}
