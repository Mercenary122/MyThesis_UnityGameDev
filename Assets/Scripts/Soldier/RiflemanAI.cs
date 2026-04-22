using UnityEngine;
using UnityEngine.AI;

public class RiflemanAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;

    [Header("Combat Stats")]
    public float detectionRange = 25f;
    public float attackRange = 15f;
    public float fireRate = 1.5f;
    public float attackDamage = 15f;
    public float zombieDamage = 50f;

    [Header("Component References")]
    public Transform gunBarrel;
    public ParticleSystem muzzleFlash;
    public AudioSource gunSound;

    [Header("Target Switch Settings")]
    public float targetSwitchInterval = 2f;
    public string zombieTag = "Zombie";

    private NavMeshAgent agent;
    private Animator anim;
    private float nextFireTime = 0f;
    private EnemyVisibility visibility;

    // Target system
    private Transform currentTarget;
    private float nextTargetScanTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        visibility = GetComponent<EnemyVisibility>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
        if (player == null) return;

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph == null) ph = player.GetComponentInChildren<PlayerHealth>();
        if (ph != null && ph.isDead)
        {
            if (agent != null) agent.isStopped = true;
            return;
        }

        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // Periodically scan for closest target
        if (Time.time >= nextTargetScanTime)
        {
            currentTarget = FindClosestTarget();
            nextTargetScanTime = Time.time + targetSwitchInterval;
        }

        if (currentTarget == null)
        {
            agent.isStopped = true;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        // Determine whether to engage
        bool shouldEngage = false;
        bool targetIsZombie = currentTarget.CompareTag(zombieTag);

        if (visibility != null)
        {
            if (targetIsZombie)
            {
                // Use distance check for zombies
                shouldEngage = distanceToTarget <= detectionRange;
            }
            else
            {
                // Use visibility system for player
                shouldEngage = visibility.canSeePlayer || visibility.isAlerted;
            }
        }
        else
        {
            shouldEngage = distanceToTarget <= detectionRange;
        }

        if (shouldEngage)
        {
            if (distanceToTarget <= attackRange)
            {
                EngageTarget();
            }
            else
            {
                ChaseTarget();
            }
        }
        else if (visibility != null && visibility.isAlerted)
        {
            // Heard a sound, investigate
            agent.isStopped = false;
            agent.SetDestination(visibility.lastKnownPosition);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    Transform FindClosestTarget()
    {
        float searchRange = detectionRange;
        if (visibility != null && visibility.viewDistance > searchRange)
        {
            searchRange = visibility.viewDistance;
        }

        Transform closest = null;
        float closestDist = searchRange;

        // Check player distance
        if (player != null)
        {
            float playerDist = Vector3.Distance(transform.position, player.position);
            if (playerDist < closestDist)
            {
                closestDist = playerDist;
                closest = player;
            }
        }

        // Check all zombies
        GameObject[] zombies = GameObject.FindGameObjectsWithTag(zombieTag);
        foreach (GameObject z in zombies)
        {
            float dist = Vector3.Distance(transform.position, z.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = z.transform;
            }
        }

        return closest;
    }

    void ChaseTarget()
    {
        agent.isStopped = false;
        agent.SetDestination(currentTarget.position);
    }

    void EngageTarget()
    {
        agent.isStopped = true;

        // Rotate toward current target
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);

        if (Time.time >= nextFireTime)
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        float effectiveRange = attackRange;
        if (visibility != null && visibility.viewDistance > effectiveRange)
        {
            effectiveRange = visibility.viewDistance;
        }

        // Raycast toward current target
        Vector3 shootOrigin = gunBarrel.position;
        Vector3 shootDirection = (currentTarget.position - shootOrigin).normalized;

        Debug.DrawRay(shootOrigin, shootDirection * effectiveRange, Color.red, 2f);

        Collider myCollider = GetComponent<Collider>();
        if (myCollider != null) myCollider.enabled = false;

        RaycastHit hit;
        bool didHit = Physics.Raycast(shootOrigin, shootDirection, out hit, effectiveRange);

        if (myCollider != null) myCollider.enabled = true;

        if (didHit)
        {
            // Hit player
            if (hit.collider.CompareTag("Player"))
            {
                nextFireTime = Time.time + fireRate;
                if (muzzleFlash != null) muzzleFlash.Play();
                if (gunSound != null) gunSound.Play();
                if (anim != null) anim.SetTrigger("Shoot");

                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph == null) ph = player.GetComponentInChildren<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(attackDamage);
                    Debug.Log("Rifleman hit player! Damage: " + attackDamage);
                }
            }
            // Hit zombie
            else if (hit.collider.CompareTag(zombieTag))
            {
                nextFireTime = Time.time + fireRate;
                if (muzzleFlash != null) muzzleFlash.Play();
                if (gunSound != null) gunSound.Play();
                if (anim != null) anim.SetTrigger("Shoot");

                Target target = hit.collider.GetComponent<Target>();
                if (target == null) target = hit.collider.GetComponentInParent<Target>();
                if (target != null)
                {
                    target.TakeDamage(zombieDamage);
                    Debug.Log("Rifleman hit zombie! Damage: " + attackDamage);
                }
            }
            // Hit friendly (Soldier tag) -> do not fire
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}