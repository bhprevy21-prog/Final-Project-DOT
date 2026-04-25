using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("Movement Area")]
    public Vector2 pointA = new Vector2(-21, 51);
    public Vector2 pointB = new Vector2(41, 51);
    public Vector2 pointC = new Vector2(-21, -66);

    [Header("Movement")]
    public float speed = 1f;
    public float stopDistance = 1.5f;
    public bool canMove = true;

    [Header("Happiness")]
    public int happiness = 100;

    private Vector2 targetPoint;
    private float aliveTime = 0f;

    private bool leaving = false;
    private bool hasReported = false;

    public bool statueTriggeredThisWave = false;

    private Rigidbody2D rb;

    // =========================
    // START
    // =========================

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SetRandomColor();
        PickNewTarget();
    }

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        aliveTime += Time.deltaTime;

        if (leaving)
        {
            MoveToExit();
            return;
        }

        HandleMovement();
        HandleLifeChecks();
    }

    // =========================
    // MOVEMENT (FIXED)
    // =========================

    void HandleMovement()
{
    if (!canMove)
        return;

    Vector2 toTarget = targetPoint - rb.position;
    float dist = toTarget.magnitude;

    if (dist < stopDistance)
    {
        PickNewTarget();
        return;
    }

    // ✅ CONSTANT SPEED (NO ACCELERATION, NO SNAP)
    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPoint, speed * Time.deltaTime);

    // ✅ add light separation so they don’t stack
    Vector2 separation = GetSeparationForce() * 0.2f;

    rb.MovePosition(newPos + separation * Time.deltaTime);
}

    void PickNewTarget()
    {
        targetPoint = GetRandomPointInTriangle();
    }

    Vector2 GetRandomPointInTriangle()
    {
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }

        return pointA + r1 * (pointB - pointA) + r2 * (pointC - pointA);
    }

    // =========================
    // SEPARATION (STABLE)
    // =========================

    Vector2 GetSeparationForce()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);

        Vector2 force = Vector2.zero;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Vector2 diff = (Vector2)transform.position - (Vector2)hit.transform.position;
            float dist = Mathf.Max(diff.magnitude, 0.1f);

            force += diff.normalized / dist;
        }

        return force;
    }

    // =========================
    // LIFE CHECKS
    // =========================

    void HandleLifeChecks()
    {
        if (aliveTime >= 30f && !leaving)
        {
            GivePositiveReview();
            leaving = true;
        }

        if (happiness <= 0 && !leaving)
        {
            GiveNegativeReview();
            leaving = true;
        }
    }

    // =========================
    // EXIT
    // =========================

    void MoveToExit()
    {
        Vector2 exit = new Vector2(36, 51);

        rb.MovePosition(Vector2.MoveTowards(rb.position, exit, speed * Time.deltaTime));

        if (Vector2.Distance(rb.position, exit) < 1f)
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // STATUE INTERACTION
    // =========================

    public void InteractWithStatue(bool isClean)
    {
        if (statueTriggeredThisWave)
            return;

        statueTriggeredThisWave = true;
        canMove = false;

        if (isClean)
            happiness += 20;
        else
            happiness -= 50;

        happiness = Mathf.Clamp(happiness, 0, 100);

        PickNewTarget();
        StartCoroutine(ResumeMovement());
    }

    IEnumerator ResumeMovement()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    // =========================
    // DAMAGE
    // =========================

    public void HitBird() => ModifyHappiness(-5);
    public void HitPoop() => ModifyHappiness(-10);
    public void HitBullet() => ModifyHappiness(-20);

    void ModifyHappiness(int amount)
    {
        happiness += amount;
    }

    // =========================
    // REVIEWS
    // =========================

    void GiveNegativeReview()
    {
        if (hasReported) return;
        hasReported = true;

        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null)
            wm.NPCFinishedNegative();
    }

    void GivePositiveReview()
    {
        if (hasReported) return;
        hasReported = true;

        WaveManager wm = FindFirstObjectByType<WaveManager>();
        if (wm != null)
            wm.NPCFinishedPositive();
    }

    // =========================
    // VISUAL
    // =========================

    void SetRandomColor()
    {
        GetComponent<SpriteRenderer>().color =
            new Color(Random.value, Random.value, Random.value);
    }
}