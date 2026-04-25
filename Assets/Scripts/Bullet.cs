using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔴 ENEMY HIT
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(1); // better than instant destroy
            }

            Destroy(gameObject);
            return;
        }

        // 🧍 NPC HIT
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