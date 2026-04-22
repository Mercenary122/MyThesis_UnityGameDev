
using UnityEngine;
using UnityEngine.AI;

public class NavMeshZombie : MonoBehaviour
{
    [Header("Target & Navigation")]
    public Transform player;          // Player reference
    private NavMeshAgent agent;       // Navigation component

    [Header("Attack Settings")]
    public float attackDistance = 2f; // Distance to start attacking
    public float attackCooldown = 1.5f;// Attack cooldown
    private float lastAttackTime = 0f;

    [Header("Audio Control")]
    public AudioSource audioSource;
    public AudioClip attackSound;      // Attack bite sound
    public AudioClip[] chaseGrowls;    // Chase growl sounds (array for variety)
    public float growlInterval = 4f;   // Approximate interval between growls
    private float growlTimer = 0f;

    [Header("Animation Control")]
    // 1. Animator reference
    public Animator anim;

    void Start()
    {
        // Get navigation component
        agent = GetComponent<NavMeshAgent>();

        // 2. Auto-find Animator on child objects (zombie model)
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // Stop if player is null or dead
        if (player == null) return;

        // Calculate distance to player
        float distance = Vector3.Distance(transform.position, player.position);

        // Attack if within range, otherwise chase
        if (distance <= attackDistance)
        {
            AttackBehavior();
        }
        else
        {
            ChaseBehavior();
        }
    }

    void ChaseBehavior()
    {
        if (agent.isStopped) agent.isStopped = false;
        agent.SetDestination(player.position);

        if (anim != null)
        {
            anim.SetBool("isChasing", true);
        }

        // --- Random growl sounds during chase ---
        growlTimer -= Time.deltaTime; // Countdown
        if (growlTimer <= 0f && chaseGrowls.Length > 0)
        {
            // Play a random growl from the array
            AudioClip randomGrowl = chaseGrowls[Random.Range(0, chaseGrowls.Length)];
            if (audioSource != null && randomGrowl != null)
            {
                audioSource.PlayOneShot(randomGrowl);
            }
            // Reset timer with slight randomness for natural feel
            growlTimer = growlInterval + Random.Range(-1f, 1f);
        }
        // -----------------------------------
    }

    void AttackBehavior()
    {
        agent.isStopped = true;
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPos);

        if (anim != null) anim.SetBool("isChasing", false);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            if (anim != null) anim.SetTrigger("doAttack");

            // --- Play attack sound ---
            if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            if (player != null)
            {
                // Get player health component
                PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
                if (pHealth != null)
                {
                    pHealth.TakeDamage(15f); // Bite damage per attack
                }
            }
        }
    }
}