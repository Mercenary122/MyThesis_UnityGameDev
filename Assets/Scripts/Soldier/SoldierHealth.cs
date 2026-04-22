using UnityEngine;
using UnityEngine.AI;

public class SoldierHealth : MonoBehaviour
{
    public float health = 100f;
    private bool isDead = false;
    private Target target; // Reference to Target component

    void Start()
    {
        // Ensure Target component exists (same as EnemyHealth)
        target = GetComponent<Target>();
        if (target == null)
            target = gameObject.AddComponent<Target>();

        target.health = health; // Sync health value
    }

    void Update()
    {
        // Check death via Target (guns deal damage to Target component)
        if (!isDead && target != null && target.health <= 0)
        {
            Die();
        }
    }

    // Kept for backward compatibility with other scripts
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        health -= damage;
        if (target != null) target.health = health;
        Debug.Log("Soldier took damage, remaining health: " + health);
        if (health <= 0) Die();
    }

    void Die()
    {
        isDead = true;

        // Disable all AI scripts
        if (TryGetComponent(out RiflemanAI ai)) ai.enabled = false;
        if (TryGetComponent(out SniperAI sniper)) sniper.enabled = false;

        if (TryGetComponent(out NavMeshAgent agent)) agent.enabled = false;
        if (TryGetComponent(out Animator anim)) anim.enabled = false;
        if (TryGetComponent(out CapsuleCollider col)) col.enabled = false;

        // Disable laser (if LineRenderer exists)
        if (TryGetComponent(out LineRenderer laser)) laser.enabled = false;

        // Fall in place without changing Y position (prevents rooftop enemies from falling)
        transform.eulerAngles = new Vector3(-90f, transform.eulerAngles.y, transform.eulerAngles.z);

        Destroy(gameObject, 30f);
    }
}