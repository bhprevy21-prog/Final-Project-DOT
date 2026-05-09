using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 10f;
    public float rotateSpeed = 720f;
    public float hitDistance = 0.2f;
    public float lifeTime = 5f;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target == null)
            FindClosestEnemy();

        if (target == null)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            return;
        }

        // rotate toward target
        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );

        // move
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // MANUAL HIT CHECK (more reliable than trigger)
        float dist = Vector2.Distance(transform.position, target.position);

        if (dist <= hitDistance)
        {
            Enemy enemy = target.GetComponent<Enemy>();

            if (enemy != null)
                enemy.DieFromTurret();

            Destroy(gameObject);
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float shortest = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject enemy in enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();

            // skip dead enemies
            if (e == null)
                continue;

            float dist = Vector2.Distance(transform.position, enemy.transform.position);

            if (dist < shortest)
            {
                shortest = dist;
                closest = enemy.transform;
            }
        }

        target = closest;
    }
}