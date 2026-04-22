using UnityEngine;

// Extends the PlayerInteractable
public class WallSwitch : PlayerInteractable
{
    [Header("Switch Settings")]
    [Tooltip("Assign the door to control")]
    public SlidingDoor targetDoor;
    public AudioSource switchSound;

    // Override the parent class virtual method
    protected override void Interact()
    {
        if (targetDoor != null)
        {
            Debug.Log("Switch activated£¡");
            targetDoor.OpenDoor();
        }

        // 2. Play switch sound
        if (switchSound != null) switchSound.Play();
    }
}