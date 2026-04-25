using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    [Header("Combat")]
    public float range = 7f;
    public float fireCooldown = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject interactPrompt;

public UnityEngine.UI.Slider batteryBar;
    private float timer = 0f;

    private bool playerInRangeLastFrame = false;

    [Header("Battery")]
    public float maxBattery = 100f; 
    public float currentBattery;
    public bool isPowered = true;

    [Header("Recharge")]
    public float rechargeTime = 2f;
    private bool isRecharging = false;

    [Header("Player Detection")]
    public float interactRange = 1f;
    private Transform player;

[Header("Health")]
public int health = 50;

public void TakeDamage(int dmg)
{
    health -= dmg;

    if (health <= 0)
    {
        Destroy(gameObject);
    }
}
  void Start()
{
    currentBattery = maxBattery;
    player = GameObject.FindGameObjectWithTag("Player").transform;

    UpdateBatteryUI();

    SetBatteryBarVisible(false); // 👈 hidden at start
}

    void Update()
    {
        timer -= Time.deltaTime;

        // 🔋 If powered, allow shooting
        if (isPowered)
        {
            GameObject target = GetClosestEnemyInRange();

            if (target != null && timer <= 0f)
            {
                Shoot(target);
                timer = fireCooldown;
            }
        }

        // 🔌 Check battery depletion
        if (currentBattery <= 0 && isPowered)
        {
            isPowered = false;
            Debug.Log("Turret out of battery!");
        }

        HandleRechargeInput();
    }

   void HandleRechargeInput()
{
    if (player == null || isRecharging) return;

    float dist = Vector2.Distance(transform.position, player.position);
    bool playerInRange = dist <= interactRange;

    // 👇 ENTER RANGE
    if (playerInRange && !playerInRangeLastFrame)
    {
        if (!isPowered)
        {
            interactPrompt.SetActive(true);
        }
    }

    // 👇 EXIT RANGE
    if (!playerInRange && playerInRangeLastFrame)
    {
        interactPrompt.SetActive(false);
    }

    playerInRangeLastFrame = playerInRange;

    // 🔌 interact
    if (playerInRange && !isPowered && Input.GetKeyDown(KeyCode.E))
    {
        interactPrompt.SetActive(false); // hide during charge
        StartCoroutine(Recharge());
    }
}

  IEnumerator Recharge()
{
    isRecharging = true;

    SetBatteryBarVisible(true); // 👈 ensure visible during recharge

    float target = maxBattery;

    while (currentBattery < target)
    {
        currentBattery += 1f;

        if (currentBattery > target)
            currentBattery = target;

        UpdateBatteryUI();

        yield return new WaitForSeconds(0.05f);
    }

    isPowered = true;
    isRecharging = false;

    UpdateBatteryUI();

    SetBatteryBarVisible(false); // 👈 hide when fully charged

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

    Vector2 direction = (target.transform.position - firePoint.position).normalized;

    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    bullet.GetComponent<Bullet>().SetDirection(direction);

    currentBattery -= 5f;

    UpdateBatteryUI(); // 🔋 update bar

    Debug.Log("Battery: " + currentBattery);
}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
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
}