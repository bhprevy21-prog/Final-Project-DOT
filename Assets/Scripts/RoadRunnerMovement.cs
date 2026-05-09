using UnityEngine;
using Pathfinding;

public class RoadRunnerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 70f;
    public float wanderRadius = 25f;
    public float repathTime = 2f;
    public float attackRange = 2f;

    private AIPath ai;
    private float repathTimer;

    public NPC currentTarget;

    void Start()
    {
        ai = GetComponent<AIPath>();

        if (ai == null)
        {
            Debug.LogError("RoadRunner missing AIPath!");
            return;
        }

        // snap to nearest valid node
        var nearest = AstarPath.active.GetNearest(transform.position);

        if (nearest.node != null && nearest.node.Walkable)
        {
            transform.position = (Vector3)nearest.position;
        }
        else
        {
            Debug.LogError("RoadRunner spawned off nav graph!");
        }

        ai.maxSpeed = moveSpeed;

        // start wandering immediately
        PickWanderPoint();
        repathTimer = repathTime;
    }

    void Update()
    {
        if (ai == null || !ai.enabled)
            return;

        repathTimer -= Time.deltaTime;

        // lose dead target
        if (currentTarget == null)
            currentTarget = FindClosestNPC();

        // chase npc
        if (currentTarget != null)
        {
            float dist =
                Vector2.Distance(
                    transform.position,
                    currentTarget.transform.position
                );

            // close enough = stop (attack script handles attack)
            if (dist <= attackRange)
            {
                ai.destination = transform.position;
                return;
            }

            ai.destination = currentTarget.transform.position;
            return;
        }

        // no npc = wander
        if (repathTimer <= 0f || ai.reachedDestination)
        {
            PickWanderPoint();
            repathTimer = repathTime;
        }
    }

    void PickWanderPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset =
                Random.insideUnitCircle * wanderRadius;

            Vector3 guess =
                transform.position +
                new Vector3(offset.x, offset.y, 0);

            var nearest =
                AstarPath.active.GetNearest(guess);

            if (nearest.node != null &&
                nearest.node.Walkable)
            {
                ai.destination =
                    (Vector3)nearest.position;
                return;
            }
        }

        // fallback
        ai.destination = transform.position;
    }

    NPC FindClosestNPC()
    {
        NPC[] npcs = FindObjectsOfType<NPC>();

        NPC closest = null;
        float bestDistance = Mathf.Infinity;

        foreach (NPC npc in npcs)
        {
            if (npc == null)
                continue;

            float dist =
                Vector2.Distance(
                    transform.position,
                    npc.transform.position
                );

            if (dist < bestDistance)
            {
                bestDistance = dist;
                closest = npc;
            }
        }

        return closest;
    }
}