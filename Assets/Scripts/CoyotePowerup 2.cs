using System.Collections;
using UnityEngine;
using Pathfinding;

public class CoyotePowerup : MonoBehaviour
{
    enum CoyoteState { Caged, Chase, Scared }

    [Header("Detection")]
    public float roadrunnerDetectRadius = 60f;
    public float roadrunnerPriorityRadius = 20f;
    public float npcScareRadius = 10f;
    public float catchDistance = 1.2f;

    [Header("Movement")]
    public float startChaseSpeed = 50f;
    public float scaredSpeed = 80f;
    public float minSpeed = 40f;
    public float maxSpeed = 100f;

    [Header("Visual")]
    public Sprite cagedSprite;
    public Sprite chaseSprite;
    public Sprite scaredSprite;

    [Header("References")]
    public GameObject cageObject;

    private CoyoteState state = CoyoteState.Caged;

    private Vector3 homePosition;
    private Transform currentRoadRunner;

    private AIPath ai;
    private AIDestinationSetter destSetter;
    private SpriteRenderer sr;

    private Coroutine searchRoutine;
    private Coroutine speedRoutine;

    // =========================
    // INIT
    // =========================
    IEnumerator Start()
    {
        ai = GetComponent<AIPath>();
        destSetter = GetComponent<AIDestinationSetter>();
       sr = GetComponentInChildren<SpriteRenderer>();
        yield return null;

        if (ai == null || destSetter == null)
        {
            Debug.LogError("Coyote missing AI components!");
            yield break;
        }

        homePosition = transform.position;

        if (cageObject != null)
        {
            cageObject.transform.SetParent(null, true);
            cageObject.transform.position = homePosition;
        }

        EnterCagedMode();
    }

    void Update()
    {
        if (state == CoyoteState.Caged)
            CheckPriorityRoadRunner();

        if (state == CoyoteState.Chase)
            HandleChase();

        if (state == CoyoteState.Scared)
            HandleScared();

        CheckNPCFear();
    }

    // =========================
    // CAGED
    // =========================
    void EnterCagedMode()
    {
        state = CoyoteState.Caged;

        transform.position = homePosition;

        ai.canMove = false;
        ai.maxSpeed = 0;

        destSetter.target = null;

        if (sr && cagedSprite)
            sr.sprite = cagedSprite;

        if (cageObject)
            cageObject.SetActive(false);

        StopAllCoroutines();
       searchRoutine = StartCoroutine(CagedSearchRoutine());

        Debug.Log("Coyote -> CAGED");
    }

    IEnumerator CagedSearchRoutine()
{
    // ⛔ wait before FIRST scan (this fixes the "instant check at 0")
    yield return new WaitForSeconds(5f);

    while (state == CoyoteState.Caged)
    {
        Debug.Log("Coyote scanning for RoadRunner...");

        Transform rr = FindRoadRunner(roadrunnerDetectRadius);

        if (rr != null)
        {
            Debug.Log("RoadRunner detected! Starting chase.");
            EnterChaseMode(rr);
            yield break;
        }

        Debug.Log("No RoadRunner found.");

        // ⏳ wait BETWEEN scans
        yield return new WaitForSeconds(5f);
    }
}

    // =========================
    // CHASE
    // =========================
  void EnterChaseMode(Transform rr)
{
    if (rr == null)
    {
        Debug.LogWarning("EnterChaseMode blocked: RoadRunner is null");
        return;
    }

    if (ai == null)
    {
        ai = GetComponent<AIPath>();
        if (ai == null)
        {
            Debug.LogError("Coyote missing AIPath component!");
            return;
        }
    }

    if (destSetter == null)
    {
        destSetter = GetComponent<AIDestinationSetter>();
        if (destSetter == null)
        {
            Debug.LogError("Coyote missing AIDestinationSetter component!");
            return;
        }
    }

    if (!rr.gameObject.activeInHierarchy)
    {
        Debug.LogWarning("RoadRunner is inactive, ignoring chase.");
        return;
    }

    state = CoyoteState.Chase;
    currentRoadRunner = rr;

    // hard reset AI (fixes A* bugs)
    ai.enabled = false;
    ai.enabled = true;

    ai.canMove = true;
    ai.maxSpeed = startChaseSpeed;

    destSetter.target = rr;

    if (sr != null && chaseSprite != null)
        sr.sprite = chaseSprite;

    if (cageObject != null)
        cageObject.SetActive(true);

    StopAllStateCoroutines();
    speedRoutine = StartCoroutine(RandomizeSpeedRoutine());

    Debug.Log("Coyote ENTERED CHASE MODE");
}
    void HandleChase()
    {
        if (currentRoadRunner == null)
        {
            EnterScaredMode();
            return;
        }

        float dist = Vector2.Distance(transform.position, currentRoadRunner.position);

        if (dist <= catchDistance)
        {
            Destroy(currentRoadRunner.gameObject);
            EnterScaredMode();
        }
    }

    // =========================
    // SCARED
    // =========================
    void EnterScaredMode()
    {
        state = CoyoteState.Scared;

        ai.canMove = true;
        ai.maxSpeed = scaredSpeed;

        if (sr && scaredSprite)
            sr.sprite = scaredSprite;

        if (cageObject)
            cageObject.SetActive(true);

        StopAllCoroutines();
    }

    void HandleScared()
    {
        Transform rr = FindRoadRunner(roadrunnerPriorityRadius);

        if (rr != null)
        {
            EnterChaseMode(rr);
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            homePosition,
            scaredSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, homePosition) < 0.5f)
        {
            EnterCagedMode();
        }
    }

    // =========================
    // NPC FEAR
    // =========================
    void CheckNPCFear()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, npcScareRadius);

        foreach (var h in hits)
        {
            if (h.GetComponent<NPC>())
            {
                EnterScaredMode();
                return;
            }
        }
    }

    // =========================
    // DETECTION
    // =========================
    void CheckPriorityRoadRunner()
    {
        if (state == CoyoteState.Chase) return;

        Transform rr = FindRoadRunner(roadrunnerPriorityRadius);

        if (rr != null)
            EnterChaseMode(rr);
    }

    Transform FindRoadRunner(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var h in hits)
        {
            if (h.CompareTag("RoadRunner"))
                return h.transform;
        }

        return null;
    }
    IEnumerator RandomizeSpeedRoutine()
{
    while (state == CoyoteState.Chase)
    {
        yield return new WaitForSeconds(3f);

        float change = Random.value < 0.5f ? -5f : 5f;

        ai.maxSpeed += change;
        ai.maxSpeed = Mathf.Clamp(ai.maxSpeed, minSpeed, maxSpeed);

        Debug.Log("Coyote speed: " + ai.maxSpeed);
    }
}
void StopAllStateCoroutines()
{
    if (searchRoutine != null)
    {
        StopCoroutine(searchRoutine);
        searchRoutine = null;
    }

    if (speedRoutine != null)
    {
        StopCoroutine(speedRoutine);
        speedRoutine = null;
    }
}
}