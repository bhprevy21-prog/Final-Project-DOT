using UnityEngine;

public class StatueInteraction : MonoBehaviour
{
    public StatueHealth statue;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered trigger: " + other.name);

        if (other.CompareTag("NPC"))
        {
            Debug.Log("NPC detected in statue zone");

            NPC npc = other.GetComponent<NPC>();
            if (npc != null)
            {
                Debug.Log("Calling InteractWithStatue()");
                npc.InteractWithStatue(statue.isClean);
            }
        }
    }
}