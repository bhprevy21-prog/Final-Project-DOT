using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("Movement Area")]
    public Vector2 pointA = new Vector2(-21, 51);
    public Vector2 pointB = new Vector2(41, 51);
    public Vector2 pointC = new Vector2(-21, -66);

    [Header("Movement")]
    public float speed = 75f;
    public float stopDistance = 1.5f;
    public bool canMove = true;

    [Header("Happiness")]
    public int happiness = 100;

    [Header("Panic")]
    public float panicSpeed = 125f;

    [Header("Visual")]
    public Sprite[] possibleSprites;

    // =========================
    // STATE
    // =========================
    private Rigidbody2D rb;

    private Vector2 targetPoint;
    private Vector2 panicDirection;

    private bool isPanicking;
    private bool isLeaving;
    private bool hasReported;

    private float aliveTime;

    // border panic timer
    private bool touchingBorder;
    private float borderTimer;

    // =========================
    // START
    // =========================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SetRandomSprite();
        PickNewTarget();
    }

    // =========================
    // UPDATE
    // =========================
    void Update()
    {
        aliveTime += Time.deltaTime;

        if (isLeaving)
        {
            MoveToExit();
            return;
        }

        if (isPanicking)
        {
            HandlePanic();
            HandleBorderDeath();
            return;
        }

        HandleMovement();
        HandleLifeCycle();
    }

    // =========================
    // MOVEMENT
    // =========================
    void HandleMovement()
    {
        if (!canMove) return;

        Vector2 toTarget = targetPoint - rb.position;

        if (toTarget.magnitude < stopDistance)
        {
            PickNewTarget();
            return;
        }

        Vector2 next = Vector2.MoveTowards(
            rb.position,
            targetPoint,
            speed * Time.deltaTime
        );

        rb.MovePosition(next + GetSeparationForce());
    }

    void PickNewTarget()
    {
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }

        targetPoint =
            pointA +
            r1 * (pointB - pointA) +
            r2 * (pointC - pointA);
    }

    Vector2 GetSeparationForce()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);

        Vector2 force = Vector2.zero;

        foreach (var h in hits)
        {
            if (h.gameObject == gameObject) continue;

            Vector2 diff = (Vector2)transform.position - (Vector2)h.transform.position;
            float dist = Mathf.Max(diff.magnitude, 0.1f);

            force += diff.normalized / dist;
        }

        return force * 0.2f;
    }

    // =========================
    // LIFE CYCLE / REVIEWS
    // =========================
    void HandleLifeCycle()
    {
        if (aliveTime >= 30f && !hasReported)
        {
            GivePositiveReview();
            BeginLeaving();
        }

        if (happiness <= 0 && !hasReported)
        {
            GiveNegativeReview();
            BeginLeaving();
        }
    }

    void BeginLeaving()
    {
        isLeaving = true;
    }

    void MoveToExit()
    {
        Vector2 exit = new Vector2(36, 51);

        rb.MovePosition(
            Vector2.MoveTowards(rb.position, exit, speed * Time.deltaTime)
        );

        if (Vector2.Distance(rb.position, exit) < 1f)
            Destroy(gameObject);
    }

    // =========================
    // DAMAGE SYSTEM
    // =========================
    public void HitBird() => ModifyHappiness(-5);
    public void HitPoop() => ModifyHappiness(-10);
    public void HitBullet() => ModifyHappiness(-20);

    void ModifyHappiness(int amount)
    {
        if (isPanicking) return;

        happiness += amount;
        happiness = Mathf.Clamp(happiness, 0, 100);

        if (happiness <= 0)
            EnterPanic();
    }

    void EnterPanic()
    {
        isPanicking = true;
        canMove = true;

        panicDirection = Random.insideUnitCircle.normalized;
    }

    void HandlePanic()
    {
        rb.MovePosition(rb.position + panicDirection * panicSpeed * Time.deltaTime);
    }

    void HandleBorderDeath()
    {
        if (!touchingBorder) return;

        borderTimer -= Time.deltaTime;

        if (borderTimer <= 0f)
            Destroy(gameObject);
    }

    // =========================
    // REVIEWS
    // =========================
    void GiveNegativeReview()
    {
        hasReported = true;
        Debug.Log("NPC left unhappy (negative review)");
    }

    void GivePositiveReview()
    {
        hasReported = true;
        Debug.Log("NPC left happy (positive review)");
    }

    // =========================
    // VISUAL
    // =========================
    void SetRandomSprite()
    {
        var sr = GetComponent<SpriteRenderer>();

        if (sr == null || possibleSprites.Length == 0)
            return;

        sr.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
    }

    // =========================
    // BORDER
    // =========================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPanicking) return;

        if (other.CompareTag("Border"))
        {
            touchingBorder = true;
            borderTimer = 2f;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Border"))
        {
            touchingBorder = false;
        }
    }
}