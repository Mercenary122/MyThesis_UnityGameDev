using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Door slide offset (X, Y, Z)")]
    public Vector3 slideOffset = new Vector3(2f, 0, 0);
    public float speed = 5f;
    public float autoCloseDistance = 6f; // Auto-close distance from player

    private Vector3 _closedPos;
    private Vector3 _openPos;
    private bool _isOpen = false;
    private Transform _player;

    void Start()
    {
        _closedPos = transform.position;
        _openPos = _closedPos + slideOffset;

        // Auto-find player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;
    }

    void Update()
    {
        // 1. Smooth movement logic
        Vector3 target = _isOpen ? _openPos : _closedPos;
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

        // 2. Auto-close logic
        if (_isOpen && _player != null)
        {
            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist > autoCloseDistance)
            {
                CloseDoor();
            }
        }
    }

    // Public method: open door
    public void OpenDoor()
    {
        _isOpen = true;
    }

    // Public method: close door
    public void CloseDoor()
    {
        _isOpen = false;
    }
}