using UnityEngine;
using Pathfinding;

public class EnemyMove : MonoBehaviour
{
    Transform target;
    AIPath ai;

    void Start()
    {
        ai = GetComponent<AIPath>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    void Update()
    {
        if (target == null) return;

        ai.destination = new Vector3(target.position.x, target.position.y, 0f);
    }
}