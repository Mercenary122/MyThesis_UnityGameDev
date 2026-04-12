
using UnityEngine;
using UnityEngine.AI;

public class NavMeshZombie : MonoBehaviour
{
    [Header("目标与导航")]
    public Transform player;          // 玩家
    private NavMeshAgent agent;       // 寻路组件

    [Header("攻击设置")]
    public float attackDistance = 2f; // 多近开始咬人
    public float attackCooldown = 1.5f;// 攻击间隔
    private float lastAttackTime = 0f;

    [Header("音效控制")]
    public AudioSource audioSource;
    public AudioClip attackSound;      // 攻击时的咬人声
    public AudioClip[] chaseGrowls;    // 追击时的低吼声 (数组，可以放好几种不同的叫声)
    public float growlInterval = 4f;   // 大概每隔几秒叫一次
    private float growlTimer = 0f;

    [Header("动画控制")]
    // 【1. 动画器变量】
    public Animator anim;

    void Start()
    {
        // 获取寻路组件
        agent = GetComponent<NavMeshAgent>();

        // 【2. 自动寻找动画器】去子物体(你的僵尸模型)身上找 Animator
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // 如果玩家死了或者没找到玩家，就不动
        if (player == null) return;

        // 计算僵尸和玩家的距离
        float distance = Vector3.Distance(transform.position, player.position);

        // 如果距离小于攻击距离，就攻击；否则就追击
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

        // --- [新增：追击时随机发出低吼声] ---
        growlTimer -= Time.deltaTime; // 倒计时
        if (growlTimer <= 0f && chaseGrowls.Length > 0)
        {
            // 从数组里随机挑一个声音播放
            AudioClip randomGrowl = chaseGrowls[Random.Range(0, chaseGrowls.Length)];
            if (audioSource != null && randomGrowl != null)
            {
                audioSource.PlayOneShot(randomGrowl);
            }
            // 重置倒计时，加一点随机性(比如 3~5秒叫一次)，听起来更自然
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

            // --- [新增：播放攻击音效] ---
            if (audioSource != null && attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            if (player != null)
            {
                // 获取玩家身上的血量脚本 (注意：如果你玩家的血量脚本不叫 PlayerHealth，请改成你实际的名字)
                PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
                if (pHealth != null)
                {
                    pHealth.TakeDamage(15f); // 咬一口扣15滴血，数值你可以自己改
                }
            }
        }
    }
}