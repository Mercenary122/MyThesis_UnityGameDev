using UnityEngine;

public class GunSoundBroadcaster : MonoBehaviour
{
    public float soundRange = 60f; // How far the gunshot sound travels

    // Called when the player fires a weapon
    public static void BroadcastGunshot(Vector3 position, float range)
    {
        // Find all enemies with EnemyVisibility in the scene
        EnemyVisibility[] allEnemies = FindObjectsByType<EnemyVisibility>(FindObjectsSortMode.None);
        foreach (EnemyVisibility enemy in allEnemies)
        {
            enemy.HearSound(position, range);
        }
    }
}