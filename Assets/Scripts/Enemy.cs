using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float launchSpeed = 8f;

    [Header("Avoidance")]
    public float separationRadius = 1.5f;
    public float separationStrength = 3f;
    [Header("Death")]
public Sprite deadSprite;
private SpriteRenderer sr;
private bool isDead = false;

    private Rigidbody2D rb;
    private Vector2 moveDir;

    private bool isLaunched = false;
    public bool canBeHit = true;

    private AIPath aiPath;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Only move when launched
        if (!isLaunched) return;

        Vector2 separation = GetSeparationForce();

        Vector2 finalDir = (moveDir + separation).normalized;

        rb.velocity = finalDir * launchSpeed;
    }

    // =========================
    // BULLET HIT
    // =========================
    public void OnHitByBullet()
    {
        if (!canBeHit) return;

        canBeHit = false;

        // stop AI pathing so it doesn't fight physics
        if (aiPath != null)
            aiPath.enabled = false;

        LaunchToBorder();
    }

    // =========================
    // LAUNCH SYSTEM
    // =========================
    void LaunchToBorder()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");

        if (borders.Length == 0) return;

        GameObject chosen = borders[Random.Range(0, borders.Length)];

        moveDir = (chosen.transform.position - transform.position).normalized;

        isLaunched = true;
    }

    // =========================
    // BORDER BEHAVIOR (ONLY WHEN LAUNCHED)
    // =========================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isLaunched) return; // 🔥 CRITICAL FIX

        if (!other.CompareTag("Border")) return;

        if (!IsInCameraView())
        {
            Destroy(gameObject);
        }
        else
        {
            // bounce/re-roll direction instead of spamming launch
            LaunchToBorder();
        }
    }

    // =========================
    // CAMERA CHECK
    // =========================
    bool IsInCameraView()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewPos.x > 0 && viewPos.x < 1 &&
               viewPos.y > 0 && viewPos.y < 1;
    }

    // =========================
    // SEPARATION (ENEMY AVOIDANCE)
    // =========================
    Vector2 GetSeparationForce()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            separationRadius
        );

        Vector2 force = Vector2.zero;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (!hit.CompareTag("Enemy"))
                continue;

            Vector2 diff = (Vector2)transform.position - (Vector2)hit.transform.position;

            float dist = diff.magnitude;

            if (dist <= 0.01f)
                continue;

            force += diff.normalized / dist;
        }

        return force.normalized * separationStrength;
    }

    // =========================
    // DEBUG
    // =========================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    void OnDisable()
    {
        Debug.Log(gameObject.name + " was DISABLED");
    }

    void OnDestroy()
    {
        Debug.Log(gameObject.name + " was DESTROYED");
        FindObjectOfType<EnemyDirectorUI>()?.ResetTimer();
    }
    public void DieFromTurret()
{
    if (isDead) return;

    isDead = true;
    canBeHit = false;

    if (aiPath != null)
        aiPath.enabled = false;

    if (rb != null)
        rb.velocity = Vector2.zero;

    // swap sprite
    if (sr != null && deadSprite != null)
        sr.sprite = deadSprite;

    // disable collider so nothing weird happens
    Collider2D col = GetComponent<Collider2D>();
    if (col != null)
        col.enabled = false;

    // dramatic short delay
    Destroy(gameObject, 0.35f);
}
}