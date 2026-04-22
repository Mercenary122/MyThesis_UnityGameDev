using UnityEngine;
using TMPro; // TextMeshPro UI namespace
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float distance = 3f;
    public LayerMask mask = ~0;

    public Camera cam;

    [Header("UI Prompt")]
    public TextMeshProUGUI promptText; // Assign the interaction prompt text

    void Start()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>();

        // Clear prompt text on start
        if (promptText != null) promptText.text = "";
    }

    void Update()
    {
        // Clear prompt every frame; it only shows when looking at an interactable
        if (promptText != null) promptText.text = "";

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            PlayerInteractable interactable = hitInfo.collider.GetComponentInParent<PlayerInteractable>();
            WeaponPickup weaponPickup = hitInfo.collider.GetComponent<WeaponPickup>();
            if (weaponPickup == null) weaponPickup = hitInfo.collider.GetComponentInParent<WeaponPickup>();

            // --- UI prompt logic ---
            if (interactable != null && promptText != null)
            {
                promptText.text = "Press [E] to interact"; // Prompt when looking at a switch
            }
            else if (weaponPickup != null && promptText != null)
            {
                // Display the object name in the pickup prompt
                promptText.text = "Press [E] to pick up " + hitInfo.collider.gameObject.name;
            }

            // --- Input handling for interaction ---
            if (interactable != null || weaponPickup != null)
            {
                bool isPressed = false;
#if ENABLE_INPUT_SYSTEM
                if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
                    isPressed = true;
#else
                if (Input.GetKeyDown(KeyCode.E))
                    isPressed = true;
#endif

                if (isPressed)
                {
                    if (interactable != null) interactable.BaseInteract();
                    if (weaponPickup != null) weaponPickup.PickUpWeapon(this.gameObject);
                }
            }
        }
    }
}