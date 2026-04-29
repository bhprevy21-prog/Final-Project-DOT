using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float launchSpeed = 8f;
[Header("Avoidance")]
public float separationRadius = 1.5f;
public float separationStrength = 3f;
    private Rigidbody2D rb;
    private Vector2 moveDir;

    private bool isLaunched = false;

private AIPath aiPath;
    void Awake()
{
    rb = GetComponent<Rigidbody2D>();
    aiPath = GetComponent<AIPath>();
}

    void Update()
    {
        if (isLaunched)
{
    Vector2 separation = GetSeparationForce();

    Vector2 finalDir =
        (moveDir + separation).normalized;

    rb.velocity = finalDir * launchSpeed;
    return;
}
    }

    void FixedUpdate()
    {
        if (!isLaunched)
        {
            rb.velocity = Vector2.zero;
        }
    }

   public void OnHitByBullet()
{
    if (aiPath != null)
        aiPath.enabled = false;

    LaunchToBorder();
}

    void LaunchToBorder()
    {
        GameObject[] borders = GameObject.FindGameObjectsWithTag("Border");

        if (borders.Length == 0) return;

        GameObject chosen = borders[Random.Range(0, borders.Length)];

        Vector2 dir = (chosen.transform.position - transform.position).normalized;

        moveDir = dir;
        isLaunched = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Border")) return;

        if (!IsInCameraView())
        {
            Destroy(gameObject);
        }
        else
        {
            LaunchToBorder();
        }
    }

    bool IsInCameraView()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewPos.x > 0 && viewPos.x < 1 &&
               viewPos.y > 0 && viewPos.y < 1;
    }
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

        Vector2 diff =
            (Vector2)transform.position -
            (Vector2)hit.transform.position;

        float dist = diff.magnitude;

        if (dist <= 0.01f)
            continue;

        force += diff.normalized / dist;
    }

    return force.normalized * separationStrength;
}
void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(
        transform.position,
        separationRadius
    );
}
}