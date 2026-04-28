using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public ShootSound shootSound;
    public bool canShoot = true;

    [Header("Bullet Spawn")]
    public float spawnOffset = 1f;

    void Update()
    {
        if (!canShoot)
            return;

        // don't shoot while clicking UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("SHOOTING");

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - firePoint.position).normalized;

        // spawn bullet slightly in front of player
        Vector2 spawnPos =
            (Vector2)firePoint.position + direction * spawnOffset;

        GameObject bullet =
            Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
            bulletScript.SetDirection(direction);

        if (shootSound != null)
            shootSound.PlayShootSound();
    }
}