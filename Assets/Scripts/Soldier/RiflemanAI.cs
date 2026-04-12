using UnityEngine;
using UnityEngine.AI;

public class RiflemanAI : MonoBehaviour
{
    [Header("目标设置")]
    public Transform player;

    [Header("战斗属性")]
    public float detectionRange = 25f;
    public float attackRange = 15f;
    public float fireRate = 1.5f;
    public float attackDamage = 15f;
    public float zombieDamage = 50f;

    [Header("组件引用")]
    public Transform gunBarrel;
    public ParticleSystem muzzleFlash;
    public AudioSource gunSound;

    [Header("目标切换设置")]
    public float targetSwitchInterval = 2f;
    public string zombieTag = "Zombie";

    private NavMeshAgent agent;
    private Animator anim;
    private float nextFireTime = 0f;
    private EnemyVisibility visibility;

    // 目标系统
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

        // 定期扫描最近目标
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

        // 判断是否应该行动
        bool shouldEngage = false;
        bool targetIsZombie = currentTarget.CompareTag(zombieTag);

        if (visibility != null)
        {
            if (targetIsZombie)
            {
                // 对僵尸用距离检测就行
                shouldEngage = distanceToTarget <= detectionRange;
            }
            else
            {
                // 对玩家用视野系统
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
            // 听到动静，去调查
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

        // 检查玩家
        if (player != null)
        {
            float playerDist = Vector3.Distance(transform.position, player.position);
            if (playerDist < closestDist)
            {
                closestDist = playerDist;
                closest = player;
            }
        }

        // 检查所有僵尸
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

        // 转向当前目标（不只是玩家）
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

        // 射线朝向当前目标（不只是玩家）
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
            // 打中玩家
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
                    Debug.Log("【步枪兵】命中玩家！扣血：" + attackDamage);
                }
            }
            // 打中僵尸
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
                    Debug.Log("【步枪兵】命中僵尸！伤害：" + attackDamage);
                }
            }
            // 打中队友（Soldier Tag）→ 不开枪
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