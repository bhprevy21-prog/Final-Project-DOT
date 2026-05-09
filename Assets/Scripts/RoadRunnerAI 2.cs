using UnityEngine;
using Pathfinding;

public class RoadRunnerAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 70f;
    public float wanderRadius = 25f;
    public float repathTime = 2f;

    [Header("NPC Attack")]
    public float attackRange = 2f;
    public float attackDurationMin = 1f;
    public float attackDurationMax = 3f;
    public float damageTickRate = 0.3f;

    [Header("Audio")]
    public AudioSource spawnSound;

    private AIPath ai;
    private float repathTimer;

    private NPC targetNPC;
    private bool isAttacking = false;

    // =========================
    // START (SAFE SPAWN)
    // =========================
    void Start()
    {
        ai = GetComponent<AIPath>();
        ai.maxSpeed = moveSpeed;

        // SNAP TO VALID NODE (CRITICAL FIX)
        var nearest = AstarPath.active.GetNearest(transform.position);
        if (nearest.node != null && nearest.node.Walkable)
            transform.position = (Vector3)nearest.position;

        if (spawnSound != null)
            spawnSound.Play();

        PickNewDestination();
    }

    // =========================
    // UPDATE LOOP (SIMPLE LIKE ENEMYMOVE)
    // =========================
    void Update()
    {
        if (ai == null || !ai.enabled) return;

        if (isAttacking)
            return;

        repathTimer -= Time.deltaTime;

        if (targetNPC == null)
            targetNPC = FindClosestNPC();

        if (targetNPC != null)
        {
            float dist = Vector2.Distance(transform.position, targetNPC.transform.position);

            if (dist <= attackRange)
            {
                StartAttack();
                return;
            }

            ai.destination = targetNPC.transform.position;
        }
        else
        {
            if (repathTimer <= 0f || ai.reachedDestination)
            {
                PickNewDestination();
                repathTimer = repathTime;
            }
        }
    }

    // =========================
    // WANDER (ENEMY MOVE STYLE)
    // =========================
    void PickNewDestination()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            Vector3 guess = transform.position + new Vector3(offset.x, offset.y, 0);

            var nearest = AstarPath.active.GetNearest(guess);

            if (nearest.node != null && nearest.node.Walkable)
            {
                ai.destination = (Vector3)nearest.position;
                return;
            }
        }

        ai.destination = transform.position;
    }

    // =========================
    // NPC FINDING
    // =========================
    NPC FindClosestNPC()
    {
        NPC[] npcs = FindObjectsOfType<NPC>();

        NPC closest = null;
        float best = Mathf.Infinity;

        foreach (NPC n in npcs)
        {
            if (n == null) continue;

            float d = Vector2.Distance(transform.position, n.transform.position);

            if (d < best)
            {
                best = d;
                closest = n;
            }
        }

        return closest;
    }

    // =========================
    // ATTACK SYSTEM (SAFE STATE)
    // =========================
    void StartAttack()
    {
        if (targetNPC == null) return;

        isAttacking = true;
        StartCoroutine(AttackRoutine());
    }

    System.Collections.IEnumerator AttackRoutine()
    {
        float attackTime = Random.Range(attackDurationMin, attackDurationMax);
        float timer = 0f;

        while (timer < attackTime)
        {
            if (targetNPC != null)
                targetNPC.HitBird();

            timer += damageTickRate;
            yield return new WaitForSeconds(damageTickRate);
        }

        isAttacking = false;
        targetNPC = null;

        PickNewDestination();
    }
}