using UnityEngine;

public class Examples : MonoBehaviour
{
    public int instanceCount = 4000;
    public float moveSpeed;

    public Vector2 min;
    public Vector2 max;

    public Mesh mesh;
    public Material material;
    private ExampleAgent exampleAgent;

    private void Awake()
    {
        exampleAgent = BRGSystem.CreateBatchAgent<ExampleAgent>(mesh, material, 10000);
        exampleAgent.moveSpeed = moveSpeed;
        exampleAgent.min = min;
        exampleAgent.max = max;
        exampleAgent.Initialize(instanceCount);
    }

    private void FixedUpdate()
    {
        exampleAgent.Update();
    }

    private void OnDestroy()
    {
        exampleAgent.Dispose();
    }
}
