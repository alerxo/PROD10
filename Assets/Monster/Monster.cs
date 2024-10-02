using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject Player { get; private set; }
    public PlayerInput PlayerInput;
    public NavMeshAgent NavMeshAgent { get; private set; }
    public const float WalkSpeed = 3;
    public const float RunSpeed = 6f;

    private IMonsterState state;
    public readonly Monster_Idle idleState = new();
    public readonly Monster_Patrolling patrollingState = new();
    public readonly Monster_Investigating investigatingState = new();
    public readonly Monster_Chasing chasingState = new();
    public readonly Monster_Attacking attackingState = new();
    public readonly Monster_Stunned stunnedState = new();
    public readonly Monster_Killing killingState = new();

    public AudioSource AmbienceAudio;
    public AudioSource StateAudio;
    public AudioSource ActionAudio;

    public AudioClip WalkingClip;
    public AudioClip RunningClip;
    public AudioClip ChaseTriggeredClip;
    public AudioClip AttackWindupClip;
    public AudioClip BlockClip;
    public AudioClip KillClip;

    public Clue CurrentClue { get; private set; }
    [field: SerializeField] public float PlayerNoiseValue { get; private set; }
    public const float PlayerNoiseValueFalloff = 0.7f;

    [SerializeField] private string stateName;

    private void Awake()
    {
        PlayerInput = new();
        PlayerInput.Enable();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Player = FindObjectOfType<PlayerController>().gameObject;
        ClueSystem.OnClueTriggered += ClueTriggered;
        PlayerInput.Player.Block.performed += Block_performed;
    }

    private void OnDestroy()
    {
        ClueSystem.OnClueTriggered -= ClueTriggered;
        PlayerInput.Player.Block.performed -= Block_performed;
    }

    private void Block_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        attackingState.Block();
    }

    private void Start()
    {
        state = idleState;
        StopPath();
    }

    private void Update()
    {
        if (Player == null) return;

        ManageAudio();
        UpdateNoise();
        UpdateState();
    }

    private void ManageAudio()
    {
        switch (state)
        {
            case Monster_Patrolling:
                SetAudioClip(WalkingClip, true);
                break;

            case Monster_Investigating:
                SetAudioClip(WalkingClip, true);
                break;

            case Monster_Chasing:
                SetAudioClip(RunningClip, true);
                break;

            default:
                SetAudioClip(null);
                break;
        }
    }

    private void SetAudioClip(AudioClip clip, bool shouldLoop = false)
    {
        if (ActionAudio.clip == clip) return;

        StateAudio.Stop();
        ActionAudio.clip = clip;

        if (clip == null) return;

        ActionAudio.loop = shouldLoop;
        ActionAudio.Play();
    }

    public void PlayerActionSound(AudioClip clip)
    {
        if (ActionAudio.isPlaying)
        {
            ActionAudio.Stop();
        }

        if (ActionAudio.clip == clip) return;

        ActionAudio.PlayOneShot(clip);
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
        state = state.Execute(this);
        stateName = state.ToString();
    }

    private void ClueTriggered(Clue second)
    {
        if (second.Parent == Player)
        {
            PlayerNoiseValue += second.Strength;
        }

        CurrentClue = ClueSystem.GetLargerClue(CurrentClue, second, this);
    }

    public bool TrySetPath(Vector3 position, float speed)
    {
        NavMeshPath path = new();
        NavMeshAgent.CalculatePath(position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            NavMeshAgent.SetPath(path);
            NavMeshAgent.isStopped = false;
            NavMeshAgent.speed = speed;

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

    public void SetDefaultValues()
    {
        state = idleState;
        PlayerNoiseValue = 0;
        CurrentClue = null;
    }
}