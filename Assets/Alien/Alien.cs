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

    float timer = 0;

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Player = FindObjectOfType<PlayerController>().transform;
    }

    private void Start()
    {
        state = idleState;
        ClueSystem.OnClueTriggered += ClueTriggered;
    }

    private void OnDestroy()
    {
        ClueSystem.OnClueTriggered -= ClueTriggered;
    }

    private void Update()
    {
        if (Player == null) return;

        state = state.Execute(this);
        stateName = state.ToString();
    }

    private void ClueTriggered(Clue clue)
    {
        investigatingState.SetClue(this, clue);
    }

    public void HellYeah()
    {
        Destroy(Player.gameObject);
    }
}
