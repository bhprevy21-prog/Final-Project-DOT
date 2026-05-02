using UnityEngine;

public class EnemyDirectorUI : MonoBehaviour
{
    [Header("Tracking")]
    public Transform player;
    public float activationTime = 30f;
    public float detectionRadius = 15f;

    [Header("UI Arrow")]
    public Transform arrow;
    public float circleRadius = 4f;
    public float arrowSmoothSpeed = 8f;

    private float timer = 0f;

    void Update()
    {
        if (player == null || arrow == null) return;

        // 1. count time since last enemy death
        timer += Time.deltaTime;

        // 2. find nearest enemy
        GameObject nearestEnemy = FindNearestEnemy();

        // 3. only show helper after 30 seconds
        if (timer < activationTime || nearestEnemy == null)
        {
            arrow.gameObject.SetActive(false);
            return;
        }

        arrow.gameObject.SetActive(true);

        // 4. direction to enemy
        Vector2 direction = (nearestEnemy.transform.position - player.position).normalized;

        // 5. position arrow on circle perimeter
        Vector3 targetPos = player.position + (Vector3)direction * circleRadius;

        arrow.position = Vector3.Lerp(
            arrow.position,
            targetPos,
            Time.deltaTime * arrowSmoothSpeed
        );

        // 6. rotate arrow to face enemy
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0, 0, angle);
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject e in enemies)
        {
            float dist = Vector2.Distance(player.position, e.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = e;
            }
        }

        return nearest;
    }

    // call this whenever ANY enemy dies
    public void ResetTimer()
    {
        timer = 0f;
    }
}