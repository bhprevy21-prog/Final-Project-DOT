using System.Collections;
using UnityEngine;

public class RoadRunnerAttack : MonoBehaviour
{
    [Header("Attack")]
    public float attackDurationMin = 1f;
    public float attackDurationMax = 3f;
    public float damageTickRate = 0.3f;

    private RoadRunnerMovement movement;
    private bool isAttacking;

    void Start()
    {
        movement = GetComponent<RoadRunnerMovement>();
    }

    void Update()
    {
        if (movement == null || isAttacking) return;

        NPC target = movement.currentTarget;

        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.transform.position);

        if (dist <= movement.attackRange)
        {
            StartCoroutine(Attack(target));
        }
    }

    IEnumerator Attack(NPC target)
    {
        isAttacking = true;

        float attackTime = Random.Range(attackDurationMin, attackDurationMax);
        float timer = 0f;

        while (timer < attackTime)
        {
            if (target != null)
                target.HitBird();

            timer += damageTickRate;
            yield return new WaitForSeconds(damageTickRate);
        }

        movement.currentTarget = null;
        isAttacking = false;
    }
}