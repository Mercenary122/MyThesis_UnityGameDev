using UnityEngine;

public class EnemyVisibility : MonoBehaviour
{
    [Header("Visual Perception")]
    public float viewDistance = 25f;        // Maximum view distance
    public float viewAngle = 120f;          // Field of view angle
    public float eyeHeight = 1.6f;         // Eye height from ground
    public LayerMask obstacleMask;          // Layers that block line of sight (walls, buildings, etc.)

    [Header("Audio Perception")]
    public float hearingRange = 60f;        // Maximum distance to hear gunshots
    public float alertDuration = 5f;        // How long the alert state lasts after hearing a sound

    [Header("Debug")]
    public bool showDebugGizmos = true;

    // Public state flags for other scripts to read
    [HideInInspector] public bool canSeePlayer = false;
    [HideInInspector] public bool isAlerted = false;    // Heard a sound, entering alert state
    [HideInInspector] public Vector3 lastKnownPosition; // Last known player position (seen or heard)

    private Transform player;
    private float alertTimer = 0f;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Check line of sight every frame
        canSeePlayer = CheckLineOfSight();

        if (canSeePlayer)
        {
            lastKnownPosition = player.position;
            isAlerted = true;
            alertTimer = alertDuration; // Refresh alert timer when player is visible
        }

        // Alert state countdown
        if (isAlerted && !canSeePlayer)
        {
            alertTimer -= Time.deltaTime;
            if (alertTimer <= 0f)
            {
                isAlerted = false;
            }
        }
    }

    bool CheckLineOfSight()
    {
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 directionToPlayer = (player.position - eyePos).normalized;
        float distanceToPlayer = Vector3.Distance(eyePos, player.position);

        // 1. Distance check: too far to see
        if (distanceToPlayer > viewDistance)
            return false;

        // 2. Angle check: outside field of view cone
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > viewAngle / 2f)
            return false;

        // 3. Obstruction check: blocked by walls
        if (Physics.Raycast(eyePos, directionToPlayer, out RaycastHit hit, distanceToPlayer, obstacleMask))
        {
            // Raycast hit an obstacle, line of sight is blocked
            if (!hit.collider.CompareTag("Player"))
                return false;
        }

        return true;
    }

    // Called externally when a gunshot is fired nearby
    public void HearSound(Vector3 soundPosition, float soundRange)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        if (distance <= hearingRange && distance <= soundRange)
        {
            isAlerted = true;
            alertTimer = alertDuration;
            lastKnownPosition = soundPosition;
            Debug.Log("¡¾" + gameObject.name + "¡¿heard a sound!£¡");
        }
    }

    // Draw vision range gizmos in editor
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;

        // Draw view distance sphere
        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // Draw the field of view cone
        Vector3 leftBound = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward * viewDistance;
        Vector3 rightBound = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward * viewDistance;
        Gizmos.color = Color.cyan; 
        Gizmos.DrawLine(eyePos, eyePos + leftBound);
        Gizmos.DrawLine(eyePos, eyePos + rightBound);

        // Draw the hearing range
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Semi-transparent orange
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}