using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Wander,
        Chase,
        AttackingStatue,
        AttackingPlayer,
        AttackingNPC,
        Enraged
    }

    public State currentState = State.Wander;

    [Header("Stats")]
    public float speed = 2f;
    public float chaseSpeed = 4f;
    public float health = 5f;
    public float detectionRadius = 10f;
    public float stopDistance = 1.8f;

    private Rigidbody2D rb;
    private Transform target;

    private float checkTimer = 0f;
    private bool isInvincible = false;

    // =========================
    // WANDER SYSTEM
    // =========================

    private Vector2 wanderTarget;

    private Vector2 A = new Vector2(-21, 51);
    private Vector2 B = new Vector2(41, 51);
    private Vector2 C = new Vector2(-21, -66);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PickNewWanderTarget();
    }

    void Update()
    {
        if (currentState == State.Enraged)
            return;

        checkTimer += Time.deltaTime;

        if (checkTimer >= Random.Range(10f, 15f))
        {
            checkTimer = 0f;
            ScanForTargets();
        }

        HandleState();
    }

    // =========================
    // STATE HANDLER
    // =========================

    void HandleState()
    {
        switch (currentState)
        {
            case State.Wander:
                Wander();
                break;

            case State.Chase:
                if (target != null)
                {
                    Vector2 dir = (target.position - transform.position);
                    rb.velocity = dir.normalized * chaseSpeed;
                }
                break;
        }
    }

    // =========================
    // WANDER (FIXED + SMOOTH)
    // =========================

    void Wander()
    {
        Vector2 dir = (wanderTarget - rb.position);
        float dist = dir.magnitude;

        if (dist < 0.5f)
        {
            PickNewWanderTarget();
            return;
        }

        Vector2 separation = GetSeparationForce();

        rb.velocity = (dir.normalized + separation * 1.5f).normalized * speed;
    }

    void PickNewWanderTarget()
    {
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }

        wanderTarget = A + r1 * (B - A) + r2 * (C - A);
    }

    // =========================
    // TARGET SCANNING
    // =========================

    void ScanForTargets()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        Transform bestTarget = null;
        int bestPriority = -1;

        foreach (var hit in hits)
        {
            int priority = -1;

            if (hit.CompareTag("Statue")) priority = 0;
            if (hit.CompareTag("Player")) priority = 1;
            if (hit.GetComponent<NPC>() != null) priority = 2;

            if (priority > bestPriority)
            {
                bestPriority = priority;
                bestTarget = hit.transform;
            }
        }

        if (bestTarget != null)
        {
            target = bestTarget;

            if (bestPriority == 0) StartCoroutine(AttackStatue());
            if (bestPriority == 1) StartCoroutine(AttackPlayer());
            if (bestPriority == 2) StartCoroutine(AttackNPC());
        }
    }

    // =========================
    // STATUE ATTACK (+ CORRUPTION)
    // =========================

    IEnumerator AttackStatue()
{
    currentState = State.AttackingStatue;

    while (target != null && health > 0)
    {
        float dist = Vector2.Distance(transform.position, target.position);

if (dist > stopDistance)
{
    Vector2 dir = (target.position - transform.position).normalized;

    // ✅ STOP BEFORE ENTERING
    rb.velocity = dir * chaseSpeed;
}
else
{
    // ✅ HARD STOP (THIS IS WHAT YOU WERE MISSING)
    rb.velocity = Vector2.zero;

    // push slightly away so they don’t sink in
    Vector2 pushBack = (transform.position - target.position).normalized;
    transform.position += (Vector3)(pushBack * 0.05f);

    StatueHealth statue = target.GetComponent<StatueHealth>();

    if (statue != null)
    {
        statue.TakeDamage(5);

        if (Random.value <= 0.1f)
            statue.isClean = false;
    }

    yield return new WaitForSeconds(1f);
}

        yield return null;
    }

    currentState = State.Wander;
}
    // =========================
    // PLAYER ATTACK
    // =========================

    IEnumerator AttackPlayer()
    {
        currentState = State.AttackingPlayer;

        PlayerMovement2D move = target.GetComponent<PlayerMovement2D>();
        PlayerShoot shoot = target.GetComponent<PlayerShoot>();

        while (target != null)
        {
            Vector2 toTarget = (target.position - transform.position);
            float dist = toTarget.magnitude;

            if (dist > stopDistance)
            {
                rb.velocity = toTarget.normalized * chaseSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;

                if (move != null) move.canMove = false;
                if (shoot != null) shoot.canShoot = false;

                isInvincible = true;

                Vector2 randomPos = new Vector2(Random.Range(-20, 40), Random.Range(-60, 50));

                float timer = 0f;

                while (timer < 2f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, randomPos, 5f * Time.deltaTime);

                    if (target != null)
                        target.position = transform.position;

                    timer += Time.deltaTime;
                    yield return null;
                }

                if (move != null) move.canMove = true;
                if (shoot != null) shoot.canShoot = true;

                yield return new WaitForSeconds(2f);

                isInvincible = false;

                break;
            }

            yield return null;
        }

        currentState = State.Wander;
    }

    // =========================
    // NPC ATTACK
    // =========================

    IEnumerator AttackNPC()
    {
        currentState = State.AttackingNPC;

        NPC npc = target.GetComponent<NPC>();

        while (npc != null)
        {
            Vector2 toTarget = (npc.transform.position - transform.position);
            float dist = toTarget.magnitude;

            if (dist > stopDistance)
            {
                rb.velocity = toTarget.normalized * chaseSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;

                npc.canMove = false;
                npc.HitBird();

                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }

        currentState = State.Wander;
    }

    // =========================
    // DAMAGE
    // =========================

    public void TakeDamage(int dmg)
    {
        if (isInvincible) return;

        health -= dmg;

        if (health <= 0)
        {
            StartCoroutine(EnragedRun());
        }
        else
        {
            currentState = State.Chase;
        }
    }

    // =========================
    // ENRAGED DEATH RUN
    // =========================

    IEnumerator EnragedRun()
    {
        currentState = State.Enraged;

        float timer = 0f;
        Vector2 dir = Random.insideUnitCircle.normalized;

        while (timer < 7f)
        {
            rb.velocity = dir * (chaseSpeed * 2f);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // =========================
    // SEPARATION SYSTEM
    // =========================

    Vector2 GetSeparationForce()
{
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f);

    Vector2 force = Vector2.zero;

    foreach (var hit in hits)
    {
        if (hit.gameObject == gameObject) continue;

        Vector2 diff = (Vector2)transform.position - (Vector2)hit.transform.position;
        float dist = Mathf.Max(diff.magnitude, 0.1f);

        float strength = 1f;

        // ✅ STRONGER PUSH FROM STATUE
        if (hit.CompareTag("Statue"))
            strength = 3f;

        force += diff.normalized * (strength / dist);
    }

    return force;
}
}