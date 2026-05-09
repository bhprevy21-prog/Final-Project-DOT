using System.Collections;
using UnityEngine;
using Pathfinding;

public class EnemyHunterAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float detectRadius = 5f;
    public float chargeTime = 2f;
    public float diveSpeed = 15f;
    public float grabRadius = 0.7f;
    public float successChance = 0.9f;

    [Header("Safety")]
    public float failSafetyTime = 0.5f; // adjustable now (was hardcoded before)

    private float failSafetyTimer = 0f;
    private bool diveEnded = false;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite chargeSprite;
    public Sprite diveSprite;

    private SpriteRenderer sr;
    private Transform player;
    private PlayerMovement2D move;
    private PlayerShoot shoot;

    private Rigidbody2D rb;
    private AIPath ai;
    private EnemyMove wander;

    private Vector2 diveTarget;

    private bool canAttempt = true;
    private bool isCharging = false;
    private bool isDiving = false;
    private bool hasUsedSuccessfulGrab = false;
    private bool playerLeftRadius = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ai = GetComponent<AIPath>();
        wander = GetComponent<EnemyMove>();
        sr = GetComponent<SpriteRenderer>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
        {
            player = p.transform;
            move = p.GetComponent<PlayerMovement2D>();
            shoot = p.GetComponent<PlayerShoot>();
        }
    }

    void Update()
    {
        if (player == null) return;
        if (hasUsedSuccessfulGrab) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // ENTRY LOGIC
        if (!isCharging && !isDiving && canAttempt)
        {
            if (dist <= detectRadius && playerLeftRadius)
            {
                playerLeftRadius = false;

                if (Random.value <= successChance)
                    StartCoroutine(ChargeAttack(false));
                else
                    StartCoroutine(FailDive());
            }

            if (dist > detectRadius)
                playerLeftRadius = true;
        }

        // DIVE LOGIC
        if (isDiving)
        {
            failSafetyTimer += Time.deltaTime;

            Vector2 dir = (diveTarget - (Vector2)transform.position).normalized;
            rb.velocity = dir * diveSpeed;

            float grabDist = Vector2.Distance(transform.position, player.position);

            // SUCCESS
            if (grabDist <= grabRadius && !diveEnded)
            {
                diveEnded = true;
                StartCoroutine(GrabPlayer());
                return;
            }

            // FAIL (SAFE BACKUP ONLY)
            if (failSafetyTimer >= failSafetyTime && !diveEnded)
            {
                diveEnded = true;
                StartCoroutine(FailDive());
            }
        }
    }

    // =========================
    // CHARGE ATTACK (NORMAL + FORCED)
    // =========================
    IEnumerator ChargeAttack(bool forced)
    {
        isCharging = true;
        isDiving = false;

        diveEnded = false;
        failSafetyTimer = 0f;

        LockAI();
        rb.velocity = Vector2.zero;

        sr.sprite = chargeSprite;

        float t = 0f;

        while (t < chargeTime)
        {
            if (player == null)
            {
                ResetToWander();
                yield break;
            }

            Vector2 dir = (player.position - transform.position).normalized;
            sr.flipX = dir.x < 0;

            t += Time.deltaTime;
            yield return null;
        }

        if (player == null)
        {
            ResetToWander();
            yield break;
        }

        // lock target at end of charge
        diveTarget = player.position;

        isCharging = false;
        isDiving = true;

        failSafetyTimer = 0f;
        diveEnded = false;

        sr.sprite = diveSprite;
    }

    // =========================
    // GRAB SUCCESS
    // =========================
    IEnumerator GrabPlayer()
    {
        isDiving = false;
        rb.velocity = Vector2.zero;

        move.canMove = false;
        shoot.canShoot = false;

        Vector2 randomOffset = Random.insideUnitCircle * 15f;
        Vector3 guess = transform.position + (Vector3)randomOffset;

        var nnInfo = AstarPath.active.GetNearest(guess, NNConstraint.Default);
        var node = nnInfo.node;
        Debug.Log("Node" + node.position);   
        Vector3 carryTarget = node != null ? (Vector3)node.position : transform.position;

        float timer = 0f;

        while (timer < 2f && player != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                carryTarget,
                6f * Time.deltaTime
            );

            player.position = transform.position;

            timer += Time.deltaTime;
            yield return null;
        }

        move.canMove = true;
        shoot.canShoot = true;

        sr.sprite = normalSprite;

        hasUsedSuccessfulGrab = true;

        ResetToWander();
    }

    // =========================
    // FAIL DIVE
    // =========================
    IEnumerator FailDive()
    {
        isDiving = false;
        rb.velocity = Vector2.zero;

        UnlockAI();

        sr.sprite = normalSprite;

        ResetToWander();

        yield return new WaitForSeconds(0.2f);

        diveEnded = false;
        failSafetyTimer = 0f;
    }

    // =========================
    // STATE CONTROL
    // =========================
    void LockAI()
    {
        if (ai != null) ai.enabled = false;
        if (wander != null) wander.enabled = false;
        rb.velocity = Vector2.zero;
    }

    void UnlockAI()
    {
        if (ai != null) ai.enabled = true;
        if (wander != null) wander.enabled = true;
    }

    void ResetToWander()
    {
        isCharging = false;
        isDiving = false;

        UnlockAI();

        sr.sprite = normalSprite;

        if (wander != null)
            wander.PickNewDestination();
    }

    // =========================
    // EXTERNAL FORCE ATTACK (DIRECTOR SYSTEM)
    // =========================
    public void ForceChargeAttack()
    {
        if (isCharging || isDiving) return;

        StartCoroutine(ChargeAttack(true));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}