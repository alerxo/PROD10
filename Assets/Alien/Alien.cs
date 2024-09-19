using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Alien : MonoBehaviour
{
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Transform Player { get; private set; }
    public AudioSource AudioSource { get; private set; }

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
        AudioSource = GetComponent<AudioSource>();
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

        AudioSource.volume = 1 - (Mathf.Min(Vector3.Distance(transform.position, Player.position), 20) / 20);
        AudioSource.pitch = state == investigatingState ? 1.5f : 0.5f;
    }

    private void ClueTriggered(Clue clue)
    {
        investigatingState.SetClue(this, clue);
    }

    public NavMeshPath TryGetPath(Vector3 position)
    {
        NavMeshPath path = new();
        NavMeshAgent.CalculatePath(position, path);

        return path.status == NavMeshPathStatus.PathComplete ? path : null;
    }
}
