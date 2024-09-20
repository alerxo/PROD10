using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public Transform Player { get; private set; }
    public AudioSource AudioSource { get; private set; }
    private NavMeshAgent NavMeshAgent;

    private IMonsterState state;
    public readonly Monster_Idle idleState = new();
    public readonly Monster_Patrolling patrollingState = new();
    public readonly Monster_Investigating investigatingState = new();
    public readonly Monster_Attacking attackingState = new();

    [SerializeField] private string stateName;
    [SerializeField] private float stateUpdateTimer = 0;
    private const float stateUpdateCooldownInSeconds = 0.1f;

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

        if ((stateUpdateTimer += Time.deltaTime) >= stateUpdateCooldownInSeconds)
        {
            state = state.Execute(this);
            stateName = state.ToString();
            stateUpdateTimer = 0;
        }
    }

    private void ClueTriggered(Clue clue)
    {
        investigatingState.SetClue(this, clue);
    }

    public bool IsValidDestination(Vector3 position)
    {
        NavMeshPath path = new();
        NavMeshAgent.CalculatePath(position, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }

    public bool TrySetPath(Vector3 position)
    {
        NavMeshPath path = new();
        NavMeshAgent.CalculatePath(position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            NavMeshAgent.SetPath(path);

            return true;
        }

        return false;
    }

    public void ResetPath()
    {
        NavMeshAgent.ResetPath();
    }
}
