using System.Collections;
using UnityEngine;

public class BirdTrap : MonoBehaviour
{
    [Header("Trap Stats")]
    public float radius = 10f;
    public float pullSpeed = 8f;

    [Header("Bird Catch")]
    public Sprite caughtBirdSprite;
    public float birdDespawnDelay = 0.3f;

    private GameObject targetedBird;
    private bool used = false;

    void Update()
    {
        if (used)
            return;

        // find one bird if we don't already have one
        if (targetedBird == null)
            FindBird();

        // pull that bird toward trap
        if (targetedBird != null)
            PullBird();
    }

    void FindBird()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        float closestDistance = Mathf.Infinity;
        GameObject closestBird = null;

        foreach (Collider2D hit in hits)
        {
            // NPC penalty
            NPC npc = hit.GetComponent<NPC>();
            if (npc != null)
            {
                npc.happiness = 0;
                continue;
            }

            // only birds
            if (!hit.CompareTag("Enemy"))
                continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestBird = hit.gameObject;
            }
        }

        if (closestBird != null)
        {
            targetedBird = closestBird;

EnemyHunterAttack attack =
    targetedBird.GetComponent<EnemyHunterAttack>();

if (attack != null)
{
    attack.enabled = false;
    Debug.Log("Disabled bird attack AI");
}
            Debug.Log("BirdTrap targeted: " + targetedBird.name);
        }
    }

    void PullBird()
    {
        if (targetedBird == null)
            return;

        targetedBird.transform.position = Vector2.MoveTowards(
            targetedBird.transform.position,
            transform.position,
            pullSpeed * Time.deltaTime
        );

        if (Vector2.Distance(targetedBird.transform.position, transform.position) < 0.5f)
        {
            StartCoroutine(CatchBird(targetedBird));
            used = true;
        }
    }

    IEnumerator CatchBird(GameObject bird)
    {
        SpriteRenderer sr = bird.GetComponent<SpriteRenderer>();

        if (sr != null && caughtBirdSprite != null)
            sr.sprite = caughtBirdSprite;

        yield return new WaitForSeconds(birdDespawnDelay);

        if (bird != null)
            Destroy(bird);

        Destroy(gameObject); // destroy trap too
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
            return;

        NPC npc = other.GetComponent<NPC>();
        if (npc != null)
        {
            npc.happiness = 0;
            Debug.Log("NPC triggered BirdTrap penalty");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}