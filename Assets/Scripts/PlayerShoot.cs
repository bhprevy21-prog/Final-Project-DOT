using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public bool canShoot = true;
    private float shootCooldown = 0.5f;
private float shootTimer = 0f;

    [Header("Bullet Spawn")]
    public float spawnOffset = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSFX;

    void Update()
{
    if (!canShoot)
        return;

    if (shootTimer > 0f)
        shootTimer -= Time.deltaTime;

    if (EventSystem.current != null &&
        EventSystem.current.IsPointerOverGameObject())
        return;

    if (Input.GetMouseButtonDown(0) && shootTimer <= 0f)
    {
        Shoot();
        shootTimer = shootCooldown;
    }
}

    void Shoot()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction =
            (mousePos - firePoint.position).normalized;

        Vector2 spawnPos =
            (Vector2)firePoint.position + direction * spawnOffset;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation =
            Quaternion.Euler(0, 0, angle);

        GameObject bullet =
            Instantiate(bulletPrefab, spawnPos, rotation);

        Bullet bulletScript =
            bullet.GetComponent<Bullet>();

        if (bulletScript != null)
            bulletScript.SetDirection(direction);

        // play new sound
        if (audioSource != null && shootSFX != null)
            audioSource.PlayOneShot(shootSFX);
    }
}