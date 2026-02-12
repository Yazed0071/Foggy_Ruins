using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;



public class GuardianHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private TMP_Text healthText;
    

    private int currentHealth;

    private bool playerLost = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Guardian HP: " + currentHealth;
    }

    private void Die()
    {
        Debug.Log("Guardian Dead - You Lose");

        Time.timeScale = 0f;
        playerLost = true;
        
    }

    



    public bool IsShowLoseMenu()
    {
        return playerLost;
    }
}