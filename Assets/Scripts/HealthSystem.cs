using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] public int maxHealth; //this does not need to be for the player as the player stats handles that.
    [SerializeField] private Image healthBar;

    [SerializeField] private TMPro.TextMeshProUGUI healthText; //debug only

    [HideInInspector] public bool isDead = false;

    public void Start()
    {
        health = maxHealth;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }
    }
    public int GetHealth() => health;
    public void Damage(int damageAmount)
    {
        if (!isDead)
        {
            // Ensure health doesn't go below 0
            health = Mathf.Max(health - damageAmount, 0);

            UpdateHealthBar();

            // Check if health has reached 0
            if (health == 0)
            {
                // Mark the object as dead
                isDead = true;

                // Start coroutine to despawn after a few seconds
                StartCoroutine(DespawnAfterDelay(3f));
            }
        }
    }

    public int Heal(int healAmount)
    {
        // Ensure health doesn't exceed maxHealth
        health = Mathf.Min(health + healAmount, maxHealth);

        UpdateHealthBar();

        return health;
    }

    private void UpdateHealthBar()
    {
        // Check if healthBar is not null before updating fill
        if (healthBar != null)
        {
            // Update the fill of the health bar
            healthBar.fillAmount = (float)health / maxHealth;
        }
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        DisableObjectInput();
        yield return new WaitForSeconds(delay);
        if (gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
        else if (gameObject.tag == "Player")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    // Disable input for this object if it has the FirstPersonController script
    public void DisableObjectInput()
    {
        // Check if the GameObject has the FirstPersonController script attached
        PlayerMovement controller = GetComponent<PlayerMovement>();
        if (controller != null)
        {
            // Disable input by disabling the script
            controller.enabled = false;
        }
    }
}