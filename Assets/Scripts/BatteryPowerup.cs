using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPowerup : MonoBehaviour
{
    [Header("Stats")]
    public float radius = 12f;
    public float chargePerSecond = 3f;
    public int health = 50;

    private List<Turret> turretsInRange = new List<Turret>();

    void Start()
    {
        StartCoroutine(ChargeTurrets());
    }

    void Update()
    {
        FindTurrets();
    }

    void FindTurrets()
    {
        turretsInRange.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Turret"))
            {
                Turret turret = hit.GetComponent<Turret>();

                if (turret != null && !turretsInRange.Contains(turret))
                {
                    turretsInRange.Add(turret);
                }
            }
        }
    }

    IEnumerator ChargeTurrets()
    {
        while (true)
        {
            foreach (Turret turret in turretsInRange)
            {
                if (turret != null)
                    turret.AddCharge(chargePerSecond);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}