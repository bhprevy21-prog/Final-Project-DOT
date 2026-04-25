using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public ShootSound shootSound;
    public bool canShoot = true;

    void Update()
{
    if (!canShoot) return;

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

    Vector2 direction = (mousePos - firePoint.position);

    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

    bullet.GetComponent<Bullet>().SetDirection(direction);

    // 🔊 PLAY SOUND
    if (shootSound != null)
        shootSound.PlayShootSound();
}
}