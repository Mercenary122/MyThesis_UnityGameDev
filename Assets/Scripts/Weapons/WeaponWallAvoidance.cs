using UnityEngine;

public class WeaponWallAvoidance : MonoBehaviour
{
    [Header("Avoidance Settings")]
    public float checkDistance = 1.2f;      // Forward check distance (approximately gun length)
    public Vector3 retractedPosition = new Vector3(0, -0.2f, -0.5f); // Retracted weapon position when near a wall
    public float smoothSpeed = 10f;         // Smooth speed for retracting and extending
    public LayerMask wallMask = ~0;         // Layer mask for wall detection (default: all layers)

    private Vector3 originalPosition;       // Original weapon position in hand
    private Camera mainCam;

    void Start()
    {
        // Store original weapon position
        originalPosition = transform.localPosition;
        mainCam = Camera.main;
    }

    void Update()
    {
        // Cast a ray forward from the camera center
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        RaycastHit hit;

        // If a wall is detected within check distance
        if (Physics.Raycast(ray, out hit, checkDistance, wallMask))
        {
            // Calculate proximity factor (0 = far, 1 = touching)
            float distancePercent = 1f - (hit.distance / checkDistance);

            // Calculate retraction amount based on proximity
            Vector3 targetPos = Vector3.Lerp(originalPosition, originalPosition + retractedPosition, distancePercent);

            // Smoothly move to retracted position
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // No wall ahead, smoothly restore to original position
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * smoothSpeed);
        }
    }
}