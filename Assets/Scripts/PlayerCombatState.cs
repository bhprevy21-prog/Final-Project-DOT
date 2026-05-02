using UnityEngine;

public class PlayerCombatState : MonoBehaviour
{
    public static PlayerCombatState Instance;

    public bool isBeingGrabbed = false;

    void Awake()
    {
        Instance = this;
    }
}