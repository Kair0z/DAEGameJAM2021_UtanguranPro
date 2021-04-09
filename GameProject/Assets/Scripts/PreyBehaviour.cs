using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PreyBehaviour : MonoBehaviour
{
    private enum PreyState
    {
        Wandering,
        Fleeing
    }

    [Header("Wander")]
    [SerializeField] private float wanderRadius = 5.0f;
    [SerializeField] private float wanderTime = 3.0f;

    [SerializeField] private NavMeshAgent navMeshAgentComp = null;

    private PreyState _state = PreyState.Wandering;
    private Timer _wanderTimer = new Timer();

    private void Start()
    {
        _state = PreyState.Wandering;
        _wanderTimer.Set(wanderTime);
    }

    private void Update()
    {
        _wanderTimer.OnPing(Time.deltaTime, DetermineNewWanderTarget);
    }

    private void DetermineNewWanderTarget()
    {
        Vector2 point = Random.insideUnitCircle.normalized * Random.Range(0, wanderRadius);
        navMeshAgentComp.SetDestination(new Vector3(point.x, 0f, point.y));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    public void RecieveBark(float barkPower, Vector3 barkPosition)
    {

    }
}
