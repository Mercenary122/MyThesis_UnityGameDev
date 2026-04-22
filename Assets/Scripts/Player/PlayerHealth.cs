using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // Input System namespace
#endif

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    private float maxHealth = 100f;
    public float chipSpeed = 2f;

    [Header("UI References")]
    public Image frontHealthBar;
    public Image backHealthBar;
    public GameObject deathScreen; // Assign the death screen UI panel

    [Header("Audio Settings")]
    public AudioClip deathClip;       // Death sound effect
    public AudioSource audioSource;   // AudioSource for playing sounds

    public bool isDead = false;

    void Start()
    {
        health = maxHealth;
        if (frontHealthBar != null) frontHealthBar.fillAmount = 1f;
        if (backHealthBar != null) backHealthBar.fillAmount = 1f;

        // Ensure death screen is hidden on start
        if (deathScreen != null) deathScreen.SetActive(false);
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        UpdateHealthUI();

        // Only trigger death when alive and health reaches zero
        if (!isDead && health <= 0f)
        {
            Die();
        }
    }

    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar != null ? frontHealthBar.fillAmount : 0f;
        float fillB = backHealthBar != null ? backHealthBar.fillAmount : 0f;
        float hFraction = health / maxHealth;

        if (fillB > hFraction)
        {
            if (frontHealthBar != null) frontHealthBar.fillAmount = hFraction;
            if (backHealthBar != null)
            {
                backHealthBar.color = Color.red;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete *= percentComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
            }
        }

        if (fillF < hFraction)
        {
            if (backHealthBar != null)
            {
                backHealthBar.color = Color.green;
                backHealthBar.fillAmount = hFraction;
            }
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete *= percentComplete;
            if (frontHealthBar != null)
                frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
    }

    public bool RestoreHealth(float healAmount)
    {
        // 1. Return false if already at full health
        if (health >= maxHealth)
        {
            return false;
        }

        // 2. Restore health
        health += healAmount;
        health = Mathf.Clamp(health, 0f, maxHealth);
        lerpTimer = 0f; // Reset health bar animation timer

        // 3. Return true to indicate healing was applied
        return true;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Died");

        if (audioSource != null && deathClip != null)
        {
            // PlayOneShot allows overlapping audio playback
            audioSource.PlayOneShot(deathClip);
        }

        // 1. Disable input system to prevent movement
        // Disable PlayerInput component
#if ENABLE_INPUT_SYSTEM
        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null) input.enabled = false;
#endif

        // 2. Disable weapon firing
        // Find and disable WeaponHolder to prevent shooting
        Transform weaponHolder = transform.Find("PlayerCameraRoot/WeaponHolder"); // Search by hierarchy path
        if (weaponHolder != null) weaponHolder.gameObject.SetActive(false);

        // 3. Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 4. Show death screen UI
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }
}