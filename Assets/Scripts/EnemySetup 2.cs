using UnityEngine;
using Pathfinding;

public class EnemyMove : MonoBehaviour
{
    private AIPath ai;
    private float repathTimer;

    public float repathTime = 3f;
    public float wanderRadius = 20f;

    void Start()
    {
        ai = GetComponent<AIPath>();
        PickNewDestination();
    }

    void Update()
    {
        if (ai == null || !ai.enabled) return;

        repathTimer -= Time.deltaTime;

        if (repathTimer <= 0f || ai.reachedDestination)
        {
            PickNewDestination();
            repathTimer = repathTime;
        }
    }

    public void PickNewDestination()
    {
        Vector2 randomOffset =
            Random.insideUnitCircle * wanderRadius;

        Vector3 guess =
            transform.position + (Vector3)randomOffset;

        var node =
            AstarPath.active.GetNearest(guess).node;

        if (node != null)
            ai.destination = (Vector3)node.position;
    }
}