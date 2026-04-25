using UnityEngine;

public class BirdPoop : MonoBehaviour
{
    public float slowMultiplier = 0.3f;

    private PlayerMovement2D player;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 🧍 PLAYER ENTERS POOP
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<PlayerMovement2D>();

            if (player != null)
            {
                player.ApplySlow(slowMultiplier);
            }
        }

        // 🧍 NPC STEPS IN POOP
        NPC npc = collision.GetComponent<NPC>();
        if (npc != null)
        {
            npc.HitPoop();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // 🚶 PLAYER LEAVES POOP
        if (collision.CompareTag("Player"))
        {
            if (player != null)
            {
                player.ResetSpeed();
            }

            player = null;
        }
    }

    void Update()
    {
        // 🧼 CLEANING INTERACTION
        if (player != null && Input.GetKeyDown(KeyCode.E))
        {
            player.ResetSpeed();
            Destroy(gameObject);
            Debug.Log("Poop cleaned!");
        }
    }
}