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
    [Header("Panic State")]
public float panicSpeed = 125f;

private bool isPanicking = false;
private Vector2 panicDirection;
private bool touchingBorder = false;
private float borderTimer = 0f;
[Header("Visual")]
public Sprite[] possibleSprites;
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

       SetRandomSprite();
        PickNewTarget();
    }

    // =========================
    // UPDATE
    // =========================

   void Update()
{
    aliveTime += Time.deltaTime;

    if (isPanicking && touchingBorder)
    {
        borderTimer -= Time.deltaTime;

        if (borderTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    if (leaving)
    {
        MoveToExit();
        return;
    }

    HandleMovement();
    HandleLifeChecks();
}

    void HandleMovement()
{
    if (!canMove)
        return;

    if (isPanicking)
    {
        HandlePanicMovement();
        return;
    }

    Vector2 toTarget = targetPoint - rb.position;
    float dist = toTarget.magnitude;

    if (dist < stopDistance)
    {
        PickNewTarget();
        return;
    }

    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPoint, speed * Time.deltaTime);
    Vector2 separation = GetSeparationForce() * 0.2f;

    rb.MovePosition(newPos + separation * Time.deltaTime);
}
void HandlePanicMovement()
{
    rb.MovePosition(rb.position + panicDirection * panicSpeed * Time.deltaTime);
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

       if (isPanicking)
    return;

if (isClean)
    happiness = Mathf.Min(happiness + 20, 100);
else
{
    happiness -= 50;
    if (happiness < 0) happiness = 0;
}

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
    if (isPanicking)
        return;

    happiness += amount;

    if (happiness <= 0)
    {
        happiness = 0;
        EnterPanicMode();
    }
}

   void EnterPanicMode()
{
    isPanicking = true;
    canMove = true;

    panicDirection = Random.insideUnitCircle.normalized;
}

 void GiveNegativeReview()
{
    if (hasReported) return;
    hasReported = true;

    Debug.Log("NPC left unhappy");
}

void GivePositiveReview()
{
    if (hasReported) return;
    hasReported = true;

    Debug.Log("NPC left happy");
}
    // =========================
    // VISUAL
    // =========================

    void SetRandomSprite()
{
    SpriteRenderer sr = GetComponent<SpriteRenderer>();

    if (sr == null)
        return;

    if (possibleSprites == null || possibleSprites.Length == 0)
        return;

    sr.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
}
void OnTriggerEnter2D(Collider2D other)
{
    if (!isPanicking)
        return;

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
        borderTimer = 0f;
    }
}
}