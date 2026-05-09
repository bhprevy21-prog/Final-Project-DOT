using UnityEngine;

public class StatueInteraction : MonoBehaviour
{
    public StatueHealth statue;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("NPC"))
            return;

        NPC npc = other.GetComponent<NPC>();
        if (npc == null)
            return;

        Debug.Log("NPC entered statue zone");

        // simple rule:
        if (statue != null && statue.isClean)
        {
            statue.Heal(20);
            Debug.Log("Statue rewarded for clean state");
        }
        else if (statue != null)
        {
            statue.TakeDamage(50);
            Debug.Log("Statue damaged due to dirty state");
        }

        // let NPC handle its own logic internally
    }
}