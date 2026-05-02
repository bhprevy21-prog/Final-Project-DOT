using System.Collections.Generic;
using UnityEngine;

public class BirdAttackDirector : MonoBehaviour
{
    public static BirdAttackDirector Instance;

    public float timeSinceLastAttack;
    public float forcedAttackTime = 20f;

    public List<EnemyHunterAttack> birds = new List<EnemyHunterAttack>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (timeSinceLastAttack >= forcedAttackTime)
        {
            ForceAttack();
            timeSinceLastAttack = 0f;
        }
    }

    public void RegisterAttack()
    {
        timeSinceLastAttack = 0f;
    }

    void ForceAttack()
    {
        if (birds.Count == 0) return;

        EnemyHunterAttack bird =
            birds[Random.Range(0, birds.Count)];

        if (bird != null)
        {
            bird.ForceChargeAttack();
        }
    }
}