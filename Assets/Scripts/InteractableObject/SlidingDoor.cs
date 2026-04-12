using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("门滑动的偏移量 (X, Y, Z)")]
    public Vector3 slideOffset = new Vector3(2f, 0, 0);
    public float speed = 5f;
    public float autoCloseDistance = 6f; // 走多远自动关

    private Vector3 _closedPos;
    private Vector3 _openPos;
    private bool _isOpen = false;
    private Transform _player;

    void Start()
    {
        _closedPos = transform.position;
        _openPos = _closedPos + slideOffset;

        // 自动找玩家
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _player = p.transform;
    }

    void Update()
    {
        // 1. 平滑移动逻辑
        Vector3 target = _isOpen ? _openPos : _closedPos;
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);

        // 2. 自动关闭逻辑
        if (_isOpen && _player != null)
        {
            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist > autoCloseDistance)
            {
                CloseDoor();
            }
        }
    }

    // 公开方法：开门
    public void OpenDoor()
    {
        _isOpen = true;
    }

    // 公开方法：关门
    public void CloseDoor()
    {
        _isOpen = false;
    }
}