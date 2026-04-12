using UnityEngine;
using UnityEngine.AI;

public class SoldierHealth : MonoBehaviour
{
    public float health = 100f;
    private bool isDead = false;
    private Target target; // 新增：引用Target组件

    void Start()
    {
        // 跟EnemyHealth一样，确保身上有Target组件
        target = GetComponent<Target>();
        if (target == null)
            target = gameObject.AddComponent<Target>();

        target.health = health; // 同步血量
    }

    void Update()
    {
        // 通过Target来检测是否被打死（因为枪是对Target扣血的）
        if (!isDead && target != null && target.health <= 0)
        {
            Die();
        }
    }

    // 这个方法保留，以防有其他脚本直接调用
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        health -= damage;
        if (target != null) target.health = health;
        Debug.Log("士兵受到伤害，剩余血量：" + health);
        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        // 关闭所有可能的AI脚本
        if (TryGetComponent(out RiflemanAI ai)) ai.enabled = false;
        if (TryGetComponent(out SniperAI sniper)) sniper.enabled = false;

        if (TryGetComponent(out NavMeshAgent agent)) agent.enabled = false;
        if (TryGetComponent(out Animator anim)) anim.enabled = false;
        if (TryGetComponent(out CapsuleCollider col)) col.enabled = false;

        // 关闭激光（如果有LineRenderer）
        if (TryGetComponent(out LineRenderer laser)) laser.enabled = false;

        // 原地倒下，不改变Y坐标（防止楼顶的敌人掉到地面）
        transform.eulerAngles = new Vector3(-90f, transform.eulerAngles.y, transform.eulerAngles.z);

        Destroy(gameObject, 30f);
    }
}