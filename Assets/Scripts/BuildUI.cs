using UnityEngine;
using System.Collections;
public class BuildUI : MonoBehaviour
{
    public WaveManager waveManager;
    public GameObject turretPrefab;

    public Transform player;
    public float spawnDistance = 2f;

    public PlayerMovement2D playerMovement;

    public void BuyTurret()
    {
        if (waveManager.playerMoney < 100)
        {
            Debug.Log("Not enough money!");
            return;
        }

        // 💰 spend money
        waveManager.AddMoney(-100);

        // 📍 calculate position in front of player
        Vector3 spawnPos = player.position + player.up * spawnDistance;

        // 🔫 spawn turret
        Instantiate(turretPrefab, spawnPos, Quaternion.identity);

        // 🧊 stun player
        StartCoroutine(StunPlayer(0.5f));
    }

    IEnumerator StunPlayer(float duration)
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}