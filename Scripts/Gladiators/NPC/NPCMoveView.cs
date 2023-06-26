using Gladiators.NPC;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class NPCMoveView : MonoBehaviour, INPCModule
{
    [SerializeField] private NavMeshAgent _navAgent;
    private Transform _targetPos;

    public void Init()
    {
        _navAgent.speed -= Random.Range(0, _navAgent.speed / 4);
    }

    public void SetTargetAndMove(Transform targetPos)
    {
        _targetPos = targetPos;
        
        if (_navAgent.navMeshOwner)
            _navAgent.SetDestination(_targetPos.position);
    }

    public void SetAlignOnTarget()
    {
        transform.rotation = _targetPos.rotation;
    }

    public bool EndOfRoad()
    {
        if (!((transform.position - _targetPos.position).magnitude < 0.2)) return false;
        
        _navAgent.CompleteOffMeshLink();

        return true;

    }
}