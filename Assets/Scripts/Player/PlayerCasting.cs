using UnityEngine;

public class PlayerCasting : MonoBehaviour
{
    public static float distanceFromTarget;
    [SerializeField] float toTarget;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit)) { }
        distanceFromTarget = hit.distance;
        toTarget = hit.distance;
    }
}
