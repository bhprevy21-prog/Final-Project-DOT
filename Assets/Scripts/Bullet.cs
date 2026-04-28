using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;

    public void SetDirection(Vector2 dir)
    {
        rb.velocity = dir.normalized * speed;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
                enemy.TakeDamage(1);

            Destroy(gameObject);
            return;
        }

        NPC npc = collision.GetComponent<NPC>();
        if (npc != null)
        {
            npc.HitBullet();
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Statue"))
        {
            Destroy(gameObject);
            return;
        }
    }
}