using UnityEngine;

public class ParkBoots : MonoBehaviour
{
    [Header("Pickup")]
    public float pickupRange = 2f;

    private Transform player;
    private bool playerNearby = false;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null)
            return;

        float dist = Vector2.Distance(transform.position, player.position);
        playerNearby = dist <= pickupRange;

        // pickup
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            EquipBoots();
        }
    }

    void EquipBoots()
    {
        PlayerMovement2D move = player.GetComponent<PlayerMovement2D>();

        if (move != null)
        {
            move.EquipParkBoots();
            Debug.Log("ParkBoots equipped");
        }

        // remove pickup object
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}