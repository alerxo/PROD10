using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alien : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Transform Player { get; private set; }

    private IAlienState state;
    public readonly Alien_Idle idleState = new();
    public readonly Alien_Patrolling patrollingState = new();
    public readonly Alien_Investigating investigatingState = new();
    public readonly Alien_Attacking attackingState = new();
    [SerializeField] private string stateName;

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Player = FindObjectOfType<PlayerController>().transform;
    }

    private void Start()
    {
        state = idleState;
    }

    private void Update()
    {
        if (Player == null) return;

        state = state.Execute(this);
        stateName = state.ToString();

        if (Vector3.Distance(transform.position, Player.position) < 12)
        {
            investigatingState.SetTrail(Player.position);
        }
    }

    public void HellYeah()
    {
        Destroy(Player.gameObject);
    }
}
