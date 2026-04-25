using UnityEngine;
using UnityEngine.UI;

public class StatueHealth : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;

    public Slider healthBar; // Assign in Inspector

    void Start()
    {
        currentHealth = maxHealth;

        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        Debug.Log("Statue took damage: " + amount + " | Current HP: " + currentHealth);

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            StatueGameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Statue healed: " + amount + " | Current HP: " + currentHealth);

        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    void StatueGameOver()
    {
        Debug.Log("STATUE DESTROYED - GAME OVER");
        GameManager.instance.StatueGameOver();
    }
    [Header("Cleanliness")]
public bool isClean = true; // default = clean

public void MakeDirty()
{
    isClean = false;
    Debug.Log("Statue is now DIRTY");
}

public void CleanStatue()
{
    isClean = true;
    Debug.Log("Statue is now CLEAN");
}
}