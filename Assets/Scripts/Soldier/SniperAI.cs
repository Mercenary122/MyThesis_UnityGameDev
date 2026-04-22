using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SniperAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;

    [Header("Combat Stats")]
    public float detectionRange = 60f;
    public float fireRate = 3f;
    public float attackDamage = 40f;
    public float zombieDamage = 100f;
    public float aimDelay = 1.5f;

    [Header("Component References")]
    public Transform gunBarrel;
    public ParticleSystem muzzleFlash;

    [Header("Audio Settings")]
    public AudioSource gunSound;
    [Range(0f, 1f)]
    public float minVolume = 0.3f;
    [Range(0f, 1f)]
    public float maxVolume = 1.0f;

    [Header("Laser Settings")]
    public float laserWidth = 0.03f;
    public float laserNoise = 0.02f;
    public Color laserColor = Color.red;
    public Material laserMaterial;
    public ParticleSystem laserHitEffect;

    [Header("Target Switch Settings")]
    public float targetSwitchInterval = 2f;
    public string zombieTag = "Zombie";

    private Animator anim;
    private float nextFireTime = 0f;
    private Collider myCollider;
    private LineRenderer laserLine;
    private bool isAiming = false;
    private float aimTimer = 0f;

    private Vector3[] laserPositions;
    private int laserSegments = 20;
    private Transform hitEffectTransform;

    private Transform currentTarget;
    private float nextTargetScanTime = 0f;

    // Cache visibility component to avoid per-frame GetComponent
    private EnemyVisibility visibility;

    void Start()
    {
        anim = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
        visibility = GetComponent<EnemyVisibility>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        laserLine = GetComponent<LineRenderer>();
        laserLine.startWidth = laserWidth;
        laserLine.endWidth = laserWidth * 0.5f;
        laserLine.positionCount = laserSegments;
        laserLine.enabled = false;


        if (laserMaterial != null)
        {
            laserLine.material = laserMaterial;
        }
        else
        {
            Material laserMat = new Material(Shader.Find("Unlit/Color"));
            laserMat.color = laserColor;
            laserLine.material = laserMat;
        }

        laserPositions = new Vector3[laserSegments];

        if (laserHitEffect != null)
        {
            hitEffectTransform = laserHitEffect.transform;
            laserHitEffect.Stop();
        }
    }

    // Get effective range (max of visibility range and detection range)
    float GetEffectiveRange()
    {
        float range = detectionRange;
        if (visibility != null && visibility.viewDistance > range)
        {
            range = visibility.viewDistance;
        }
        return range;
    }

    void Update()
    {
        // Periodically scan for closest target
        if (Time.time >= nextTargetScanTime)
        {
            currentTarget = FindClosestTarget();
            nextTargetScanTime = Time.time + targetSwitchInterval;
        }

        if (currentTarget == null)
        {
            laserLine.enabled = false;
            if (laserHitEffect != null && laserHitEffect.isPlaying)
                laserHitEffect.Stop();
            return;
        }

        if (anim != null) anim.SetFloat("Speed", 0f);

        if (player == null) return;

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph == null) ph = player.GetComponentInChildren<PlayerHealth>();
        if (ph != null && ph.isDead)
        {
            laserLine.enabled = false;
            if (laserHitEffect != null && laserHitEffect.isPlaying)
                laserHitEffect.Stop();
            return;
        }

        // Determine whether to engage current target
        bool shouldEngage = false;

        if (visibility != null)
        {
            bool targetIsZombie = currentTarget.CompareTag(zombieTag);
            if (targetIsZombie)
            {
                // Use distance check only for zombies
                float dist = Vector3.Distance(transform.position, currentTarget.position);
                shouldEngage = dist <= detectionRange;
            }
            else
            {
                // Use visibility system for player
                shouldEngage = visibility.canSeePlayer || visibility.isAlerted;
            }
        }
        else
        {
            // No visibility system, use distance check only
            float dist = Vector3.Distance(transform.position, currentTarget.position);
            shouldEngage = dist <= detectionRange;
        }

        if (shouldEngage)
        {
            // Smoothly rotate toward target
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                5f * Time.deltaTime
            );

            UpdateLaser();

            if (Time.time >= nextFireTime)
            {
                if (!isAiming)
                {
                    isAiming = true;
                    aimTimer = aimDelay;
                }
                else
                {
                    aimTimer -= Time.deltaTime;
                    if (aimTimer <= 0f)
                    {
                        TryShoot();
                        isAiming = false;
                    }
                }
            }
        }
        else
        {
            laserLine.enabled = false;
            if (laserHitEffect != null && laserHitEffect.isPlaying)
                laserHitEffect.Stop();
            isAiming = false;
        }
    }

    Transform FindClosestTarget()
    {
        float searchRange = GetEffectiveRange();

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

    void UpdateLaser()
    {
        laserLine.enabled = true;

        float effectiveRange = GetEffectiveRange();

        Vector3 laserStart = gunBarrel.position;
        Vector3 laserDirection = (currentTarget.position - laserStart).normalized;

        if (myCollider != null) myCollider.enabled = false;
        RaycastHit hit;
        bool didHit = Physics.Raycast(laserStart, laserDirection, out hit, effectiveRange);
        if (myCollider != null) myCollider.enabled = true;

        Vector3 laserEnd;
        if (didHit)
            laserEnd = hit.point;
        else
            laserEnd = laserStart + laserDirection * effectiveRange;

        float currentNoise = laserNoise;
        if (isAiming && aimTimer < 0.5f)
        {
            currentNoise = laserNoise * 3f;
            laserLine.enabled = (Time.time % 0.1f > 0.05f);
        }

        for (int i = 0; i < laserSegments; i++)
        {
            float t = (float)i / (laserSegments - 1);
            Vector3 basePos = Vector3.Lerp(laserStart, laserEnd, t);

            if (i == 0 || i == laserSegments - 1)
            {
                laserPositions[i] = basePos;
            }
            else
            {
                Vector3 noiseOffset = new Vector3(
                    Random.Range(-currentNoise, currentNoise),
                    Random.Range(-currentNoise, currentNoise),
                    Random.Range(-currentNoise, currentNoise)
                );
                laserPositions[i] = basePos + noiseOffset;
            }
        }

        laserLine.positionCount = laserSegments;
        laserLine.SetPositions(laserPositions);

        if (laserHitEffect != null && didHit)
        {
            hitEffectTransform.position = hit.point;
            if (!laserHitEffect.isPlaying)
                laserHitEffect.Play();
        }
        else if (laserHitEffect != null)
        {
            if (laserHitEffect.isPlaying)
                laserHitEffect.Stop();
        }
    }

    void TryShoot()
    {
        float effectiveRange = GetEffectiveRange();

        Vector3 shootOrigin = gunBarrel.position;
        Vector3 shootDirection = (currentTarget.position - shootOrigin).normalized;

        if (myCollider != null) myCollider.enabled = false;
        RaycastHit hit;
        bool didHit = Physics.Raycast(shootOrigin, shootDirection, out hit, effectiveRange);
        if (myCollider != null) myCollider.enabled = true;

        if (gunSound != null)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);
            float volumePercent = 1f - (distance / effectiveRange);
            gunSound.volume = Mathf.Lerp(minVolume, maxVolume, volumePercent);
            gunSound.Play();
        }

        if (muzzleFlash != null) muzzleFlash.Play();
        if (anim != null) anim.SetTrigger("Shoot");
        laserLine.enabled = false;

        if (didHit)
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph == null) ph = player.GetComponentInChildren<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(attackDamage);
                    Debug.Log("Sniper hit player! Damage: " + attackDamage);
                }
            }
            else if (hit.collider.CompareTag(zombieTag))
            {
                Target target = hit.collider.GetComponent<Target>();
                if (target == null) target = hit.collider.GetComponentInParent<Target>();
                if (target != null)
                {
                    target.TakeDamage(zombieDamage);
                    Debug.Log("Sniper hit zombie! Damage: " + zombieDamage);
                }
            }
        }

        nextFireTime = Time.time + fireRate;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw additional visibility range gizmo if available
        EnemyVisibility vis = GetComponent<EnemyVisibility>();
        if (vis != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, vis.viewDistance);
        }
    }
}