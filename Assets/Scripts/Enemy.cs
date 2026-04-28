using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Wander,
        Attack,
        Enraged
    }

    [Header("Stats")]
    public float speed = 2f;
    public float chaseSpeed = 4f;
    public float health = 5f;
    public float detectionRadius = 10f;
    public float stopDistance = 1.8f;

    [Header("Raycast Settings")]
    [SerializeField] LayerMask wallLayer;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite attackSprite;

    [Header("Poop")]
    public GameObject spawnPrefab;
    [Range(0f, 1f)] public float poopChance = 0.15f;

    [Header("Scan")]
    public float scanCooldown = 2f;

    private Rigidbody2D rb;
    private Transform target;
    private Coroutine currentRoutine;

    private float scanTimer;
    private bool isInvincible = false;
    private bool hasDroppedPoop = false;

    private Vector2 wanderTarget;

    private Vector2 A = new Vector2(-21, 51);
    private Vector2 B = new Vector2(41, 51);
    private Vector2 C = new Vector2(-21, -66);

    public State currentState = State.Wander;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PickNewWanderTarget();
        scanTimer = scanCooldown;
    }

    void Update()
    {
        if (currentState == State.Enraged)
            return;

        scanTimer -= Time.deltaTime;

        if (scanTimer <= 0f)
        {
            scanTimer = scanCooldown;
            ScanForTargets();
        }

        if (currentState == State.Wander)
            Wander();

        TryDropPoop();
    }

    void ScanForTargets()
    {
        if (currentState == State.Attack)
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        Transform bestTarget = null;
        int bestPriority = -1;

        foreach (Collider2D hit in hits)
        {
            int priority = -1;

            // Player > NPC > Statue
            if (hit.CompareTag("Player")) priority = 3;
            else if (hit.GetComponent<NPC>() != null) priority = 2;
            else if (hit.CompareTag("Statue")) priority = 1;

            if (priority > bestPriority)
            {
                bestPriority = priority;
                bestTarget = hit.transform;
            }
        }

        if (bestTarget == null)
            return;

        target = bestTarget;

        if (target.CompareTag("Player"))
            StartBehavior(AttackPlayer());

        else if (target.GetComponent<NPC>() != null)
            StartBehavior(AttackNPC());

        else if (target.CompareTag("Statue"))
            StartBehavior(AttackStatue());
    }

    void StartBehavior(IEnumerator routine)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(routine);
    }

    void Wander()
    {
        SetAttackVisual(false);

        Vector2 dir = wanderTarget - rb.position;
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

    IEnumerator AttackStatue()
    {
        currentState = State.Attack;
        SetAttackVisual(true);

        StatueHealth statue = target.GetComponent<StatueHealth>();

        while (target != null && statue != null)
        {
            float dist = Vector2.Distance(transform.position, target.position);

            if (dist > stopDistance)
            {
                Vector2 dir = (target.position - transform.position).normalized;
                rb.velocity = dir * chaseSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;

                statue.TakeDamage(5);

                if (Random.value <= 0.10f)
                    statue.isClean = false;

                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }

        ExitAttack();
    }

    IEnumerator AttackPlayer()
    {
        currentState = State.Attack;
        SetAttackVisual(true);

        PlayerMovement2D move = target.GetComponent<PlayerMovement2D>();
        PlayerShoot shoot = target.GetComponent<PlayerShoot>();

        while (target != null)
        {
            Vector2 toTarget = target.position - transform.position;
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

                Vector2 randomPos = GetRandomCarryPosition();

                float timer = 0f;

                while (timer < 2f && target != null)
                {
                    transform.position = Vector2.MoveTowards(
                        transform.position,
                        randomPos,
                        5f * Time.deltaTime
                    );

                    target.position = transform.position;

                    timer += Time.deltaTime;
                    yield return null;
                }

                if (move != null) move.canMove = true;
                if (shoot != null) shoot.canShoot = true;

                isInvincible = false;

                yield return new WaitForSeconds(1f);
                break;
            }

            yield return null;
        }

        ExitAttack();
    }

    IEnumerator AttackNPC()
    {
        currentState = State.Attack;
        SetAttackVisual(true);

        NPC npc = target.GetComponent<NPC>();

        while (npc != null)
        {
            Vector2 toTarget = npc.transform.position - transform.position;
            float dist = toTarget.magnitude;

            if (dist > stopDistance)
            {
                rb.velocity = toTarget.normalized * chaseSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;

                npc.canMove = false;

                Vector2 randomPos = GetRandomCarryPosition();

                float timer = 0f;

                while (timer < 3f && npc != null)
                {
                    transform.position = Vector2.MoveTowards(
                        transform.position,
                        randomPos,
                        5f * Time.deltaTime
                    );

                    npc.transform.position = transform.position;

                    timer += Time.deltaTime;
                    yield return null;
                }

                if (npc != null)
                {
                    npc.canMove = true;
                    npc.HitBird();
                }

                yield return new WaitForSeconds(1f);
                break;
            }

            yield return null;
        }

        ExitAttack();
    }

    Vector2 GetRandomCarryPosition()
    {
        float maxDistance = 100f;
        float padding = 1f;

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, maxDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, maxDistance, wallLayer);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, maxDistance, wallLayer);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, maxDistance, wallLayer);

        float minX = hitLeft ? hitLeft.point.x + padding : transform.position.x - 10f;
        float maxX = hitRight ? hitRight.point.x - padding : transform.position.x + 10f;
        float minY = hitDown ? hitDown.point.y + padding : transform.position.y - 10f;
        float maxY = hitUp ? hitUp.point.y - padding : transform.position.y + 10f;

        return new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }

    void TryDropPoop()
    {
        if (hasDroppedPoop || spawnPrefab == null)
            return;

        if (Random.value <= poopChance * Time.deltaTime)
        {
            Instantiate(spawnPrefab, transform.position, Quaternion.identity);
            hasDroppedPoop = true;
        }
    }

    void ExitAttack()
    {
        rb.velocity = Vector2.zero;
        target = null;
        currentRoutine = null;
        currentState = State.Wander;
        SetAttackVisual(false);
        PickNewWanderTarget();
    }

    void SetAttackVisual(bool attacking)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sprite = attacking ? attackSprite : normalSprite;
    }

    public void TakeDamage(int dmg)
    {
        if (isInvincible) return;

        health -= dmg;

        if (health <= 0)
            StartBehavior(EnragedRun());
    }

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

    Vector2 GetSeparationForce()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        Vector2 force = Vector2.zero;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector2 diff = (Vector2)transform.position - (Vector2)hit.transform.position;
            float dist = Mathf.Max(diff.magnitude, 0.1f);

            float strength = hit.CompareTag("Statue") ? 3f : 1f;
            force += diff.normalized * (strength / dist);
        }

        return force;
    }
}