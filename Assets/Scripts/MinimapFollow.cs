using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // Player transform reference

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPos = player.position;
            // Keep camera Y position, only update X and Z
            newPos.y = transform.position.y;
            transform.position = newPos;
        }
    }
}