using UnityEngine;
using UnityEngine.AI;
public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Transform _playerTarget;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Automatically finds the player
        // We tag the GameObject(PlayerCapsule) -> Player 
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _playerTarget = playerObj.transform;
        }
        else
        {
            Debug.LogError("Zombie can't find player,please check the Tag of PlayerCapsule if it is set as 'Player'");
        }
    }

    void Update()
    {
        if (_playerTarget != null)
        {
            _agent.SetDestination(_playerTarget.position);
        }
    }
}