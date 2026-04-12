using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent control

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    Target target;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip deathSound;       // Zombie Death scream sound

    [Header("Animation Control")]
    public Animator anim; // Used to play death animation

    // Prevent death from being triggered repeatedly every frame
    private bool isDead = false;

    void Start()
    {
        // Ensure the enemy has a Target component
        target = GetComponent<Target>();
        if (target == null)
            target = gameObject.AddComponent<Target>();

        target.health = maxHealth; // Synchronize the health value

        // Automatically find the Animator on child objects
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // Only trigger Die() when health reaches 0 and not already dead
        if (!isDead && target.health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true; // Mark as dead
        gameObject.tag = "Untagged";
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.tag = "Untagged";
        }
        Debug.Log("Enemy Dead");

        // 1. Play death animation
        if (anim != null)
        {
            anim.SetTrigger("doDie");
        }

        // 2. Disable NavMeshAgent to prevent corpse sliding
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        // 3. Disable collider to prevent corpse blocking player and bullets
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 4. Disable AI chase logic
        // Disable the zombie movement script (NavMeshZombie)
        NavMeshZombie aiScript = GetComponent<NavMeshZombie>();
        if (aiScript != null)
        {
            aiScript.enabled = false;
        }

        // 5. Play death sound effect
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        // ---------------------------

        if (anim != null) anim.SetTrigger("doDie");

        // 6. Handle corpse cleanup
        // Destroy corpse after a certain period of time to prevent performance issues
        Destroy(gameObject, 30f);

        
    }
}