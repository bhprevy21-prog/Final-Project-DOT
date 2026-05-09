using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Turret : MonoBehaviour
{
    [Header("Combat")]
    public float range = 7f;
    public float fireCooldown = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject interactPrompt;
    public Slider batteryBar;

    private float timer = 0f;
    private bool playerInRangeLastFrame = false;

    [Header("Battery")]
    public float maxBattery = 100f;
    public float currentBattery;
    public bool isPowered = true;

    [Header("Recharge")]
    public float rechargeTime = 2f;
    public bool isRecharging = false; // public for future battery powerup use

    [Header("Audio")]
    public AudioSource rotateAudio;
    public AudioSource rechargeAudio;

    [Header("Player Detection")]
    public float interactRange = 1f;
    private Transform player;

    [Header("Health")]
    public int health = 50;

    [Header("Rotation")]
public Transform rotatePart;
public float rotateSpeed = 360f;

    void Start()
    {
        currentBattery = maxBattery;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        UpdateBatteryUI();
        SetBatteryBarVisible(false);

        if (rotateAudio != null)
            rotateAudio.Stop();

        if (rechargeAudio != null)
            rechargeAudio.Stop();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (isPowered)
        {
            GameObject target = GetClosestEnemyInRange();

            if (target != null)
            {
                RotateToward(target.transform.position);

                if (timer <= 0f)
                {
                    Shoot(target);
                    timer = fireCooldown;
                }
            }
            else
            {
                StopRotateSound();
            }
        }
        else
        {
            StopRotateSound();
        }

        if (currentBattery <= 0 && isPowered)
        {
            isPowered = false;
            Debug.Log("Turret out of battery!");
        }

        HandleRechargeInput();
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Destroy(gameObject);
    }

   void RotateToward(Vector3 targetPos)
{
    if (rotatePart == null) return;

    Vector2 dir = targetPos - rotatePart.position;

    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    Quaternion targetRot = Quaternion.Euler(0, 0, angle);

    float angleDifference = Quaternion.Angle(rotatePart.rotation, targetRot);

    rotatePart.rotation = Quaternion.RotateTowards(
        rotatePart.rotation,
        targetRot,
        rotateSpeed * Time.deltaTime
    );

    if (angleDifference > 2f)
    {
        if (rotateAudio != null && !rotateAudio.isPlaying)
            rotateAudio.Play();
    }
    else
    {
        StopRotateSound();
    }
}

    void StopRotateSound()
    {
        if (rotateAudio != null && rotateAudio.isPlaying)
            rotateAudio.Stop();
    }

    void HandleRechargeInput()
    {
        if (player == null || isRecharging) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool playerInRange = dist <= interactRange;

        if (playerInRange && !playerInRangeLastFrame)
        {
            if (!isPowered)
                interactPrompt.SetActive(true);
        }

        if (!playerInRange && playerInRangeLastFrame)
        {
            interactPrompt.SetActive(false);
        }

        playerInRangeLastFrame = playerInRange;

        if (playerInRange && !isPowered && Input.GetKeyDown(KeyCode.E))
        {
            interactPrompt.SetActive(false);
            StartCoroutine(Recharge());
        }
    }

    IEnumerator Recharge()
    {
        isRecharging = true;

        SetBatteryBarVisible(true);

        if (rechargeAudio != null)
            rechargeAudio.Play();

        while (currentBattery < maxBattery)
        {
            currentBattery += 1f;

            if (currentBattery > maxBattery)
                currentBattery = maxBattery;

            UpdateBatteryUI();

            yield return new WaitForSeconds(0.05f);
        }

        if (rechargeAudio != null)
            rechargeAudio.Stop();

        isPowered = true;
        isRecharging = false;

        UpdateBatteryUI();
        SetBatteryBarVisible(false);

        Debug.Log("DEBUG: Turret FULLY RECHARGED (" + currentBattery + ")");
    }

    GameObject GetClosestEnemyInRange()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < range && distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    void Shoot(GameObject target)
{
    if (currentBattery <= 0) return;

    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    currentBattery -= 5f;
    UpdateBatteryUI();
}

    void UpdateBatteryUI()
    {
        if (batteryBar != null)
            batteryBar.value = currentBattery;
    }

    void SetBatteryBarVisible(bool visible)
    {
        if (batteryBar != null)
            batteryBar.gameObject.SetActive(visible);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
    public void AddCharge(float amount)
{
    currentBattery += amount;

    if (currentBattery > maxBattery)
        currentBattery = maxBattery;

    if (currentBattery > 0)
        isPowered = true;

    UpdateBatteryUI();
}
}