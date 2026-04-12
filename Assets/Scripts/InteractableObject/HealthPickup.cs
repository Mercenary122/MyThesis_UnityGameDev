using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How much health this pickup restores")]
    public float healAmount = 25f; // Typical health pack restore amount is around 15-25, like half-life

    [Header("Audio")]
    public AudioClip pickupSound;   // Pickup sound effect here
    [Range(0f, 1f)]
    public float volume = 1f;       // Volume level

    private void OnTriggerEnter(Collider other)
    {
        // 1. Only trigger when player hits on
        if (other.CompareTag("Player"))
        {
            // 2. Try to get player health component
            // Check self first, then parent (in case Collider is on a child object)
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth == null) playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                // 3. Try to restore health (uses bool return value)
                bool success = playerHealth.RestoreHealth(healAmount);

                // 4. If healing was successful (player was not at full health)
                if (success)
                {
                    // Play pickup sound (PlayClipAtPoint creates a temporary AudioSource)
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
                    }

                    // Destroy the health pickup object
                    Destroy(gameObject);
                }
                // If success is false (full health), nothing happens
            }
        }
    }
}