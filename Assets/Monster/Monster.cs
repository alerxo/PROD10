using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject Player { get; private set; }
    public AudioSource AudioSource { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }

    private IMonsterState state;
    public readonly Monster_Idle idleState = new();
    public readonly Monster_Patrolling patrollingState = new();
    public readonly Monster_Investigating investigatingState = new();
    public readonly Monster_Chasing chasingState = new();
    public readonly Monster_Attacking attackingState = new();

    public Clue CurrentClue { get; private set; }
    [field: SerializeField] public float PlayerNoiseValue { get; private set; }
    public const float PlayerNoiseValueFalloff = 0.7f;

    [SerializeField] private string stateName;
    [SerializeField] private float stateUpdateTimer = 0;
    public const float stateUpdateCooldownInSeconds = 0.1f;

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Player = FindObjectOfType<PlayerController>().gameObject;
        AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        state = idleState;
        StopPath();
        ClueSystem.OnClueTriggered += ClueTriggered;
    }

    private void OnDestroy()
    {
        ClueSystem.OnClueTriggered -= ClueTriggered;
    }

    private void Update()
    {
        if (Player == null) return;

        UpdateNoise();
        UpdateState();
    }

    private void UpdateNoise()
    {
        if (CurrentClue != null && !ClueSystem.IsClueValid(CurrentClue))
        {
            CurrentClue = null;
        }

        if (PlayerNoiseValue > 0)
        {
            PlayerNoiseValue -= PlayerNoiseValueFalloff * Time.deltaTime;
        }
    }

    private void UpdateState()
    {
        if (state != null && (stateUpdateTimer += Time.deltaTime) >= stateUpdateCooldownInSeconds)
        {
            state = state.Execute(this);
            stateName = state != null ? state.ToString() : "State Update Paused";
            stateUpdateTimer = 0;
        }
    }

    private void ClueTriggered(Clue second)
    {
        if (second.Parent == Player)
        {
            PlayerNoiseValue += second.Strength;
        }

        CurrentClue = ClueSystem.GetLargerClue(CurrentClue, second, this);
    }

    public bool TrySetPath(Vector3 position)
    {
        NavMeshPath path = new();
        NavMeshAgent.CalculatePath(position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            NavMeshAgent.SetPath(path);
            NavMeshAgent.isStopped = false;

            return true;
        }

        return false;
    }

    public bool IsValidDestination(Vector3 position)
    {
        NavMeshPath path = new();
        NavMeshAgent.CalculatePath(position, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }

    public void StopPath()
    {
        NavMeshAgent.ResetPath();
        NavMeshAgent.isStopped = true;
    }

    public bool HasDestination()
    {
        return NavMeshAgent.isStopped == false;
    }

    public IEnumerator Attack()
    {
        float timer = 0;

        while ((timer += Time.deltaTime) < 1f)
        {
            yield return null;
        }

        Player.GetComponent<PlayerController>().Death();

        state = idleState;
    }
}