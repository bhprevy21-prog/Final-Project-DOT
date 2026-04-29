using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        rb.velocity = dir.normalized * speed;
    }

   void OnTriggerEnter2D(Collider2D collision)
{
    Debug.Log("Bullet hit: " + collision.name);

    if (collision.CompareTag("Enemy"))
    {
        Debug.Log("Enemy detected!");

        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            Debug.Log("Calling enemy hit");
            enemy.OnHitByBullet();
        }

        Destroy(gameObject);
    }
}
}