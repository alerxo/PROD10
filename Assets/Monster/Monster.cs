using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public readonly Monster_Scared scaredState = new();

    public AudioSource AmbienceAudio;
    public AudioSource StateAudio;
    public AudioSource ActionAudio;
    public AudioSource NormalMusic;
    public AudioSource IntenseMusic;

    public AudioClip[] AmbienceClips;

    public AudioClip WalkingClip;
    public AudioClip RunningClip;
    public AudioClip ChaseTriggeredClip;
    public AudioClip AttackWindupClip;
    public AudioClip BlockClip;
    public AudioClip KillClip;

    public Clue CurrentClue { get; private set; }
    [field: SerializeField] public float PlayerNoiseValue { get; private set; }
    public const float PlayerNoiseFalloff = 0.7f;
    public const float PlayerNoiseFastFalloff = 2f;

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
        ManageMusic();
        UpdateNoise();
        UpdateState();
    }

    private void ManageAudio()
    {
        switch (state)
        {
            case Monster_Patrolling:
                SetStateAudio(WalkingClip);
                ManageAmbience(true);
                break;

            case Monster_Investigating:
                SetStateAudio(WalkingClip);
                ManageAmbience(true);
                break;

            case Monster_Chasing:
                SetStateAudio(RunningClip);
                ManageAmbience(false);
                break;

            case Monster_Attacking:
                SetStateAudio(null);
                ManageAmbience(false);
                break;

            case Monster_Stunned:
                SetStateAudio(null);
                ManageAmbience(false);
                break;

            case Monster_Killing:
                SetStateAudio(null);
                ManageAmbience(false);
                break;

            case Monster_Scared:
                SetStateAudio(RunningClip);
                ManageAmbience(false);
                break;

            default:
                SetStateAudio(null);
                ManageAmbience(true);
                break;
        }
    }

    private void ManageAmbience(bool shouldPlay)
    {
        if (!shouldPlay)
        {
            AmbienceAudio.Stop();
            return;
        }

        if (AmbienceAudio.isPlaying) return;

        AmbienceAudio.Stop();
        List<AudioClip> temp = AmbienceClips.ToList();
        temp.Remove(AmbienceAudio.clip);
        AmbienceAudio.clip = temp[Random.Range(0, temp.Count)];
        AmbienceAudio.pitch = Random.Range(0.9f, 1.1f);
        AmbienceAudio.Play();
    }

    private void SetStateAudio(AudioClip clip)
    {
        if (StateAudio.clip == clip) return;

        StateAudio.Stop();
        StateAudio.clip = clip;

        if (clip == null) return;

        StateAudio.Play();
    }

    public void SetActionAudio(AudioClip clip)
    {
        ActionAudio.Stop();
        ActionAudio.PlayOneShot(clip);
    }

    private void ManageMusic()
    {
        float volume = PlayerNoiseValue - Monster_Chasing.PlayerNoiseValueExitValue;
        float max = Monster_Chasing.PlayerNoiseValueEnterValue - Monster_Chasing.PlayerNoiseValueExitValue;
        volume = Mathf.Min(volume, max) / max;
        volume = Mathf.Pow(volume, 2);
        volume = Mathf.Clamp01(volume);

        IntenseMusic.volume = volume;
        NormalMusic.volume = 1 - volume;
    }

    private void UpdateNoise()
    {
        if (CurrentClue != null && !ClueSystem.IsClueValid(CurrentClue))
        {
            CurrentClue = null;
        }

        if (PlayerNoiseValue > 0)
        {
            PlayerNoiseValue -= (state == scaredState || PlayerNoiseValue > Monster_Chasing.PlayerNoiseValueEnterValue + PlayerNoiseFalloff ?
                PlayerNoiseFastFalloff : PlayerNoiseFalloff) * Time.deltaTime;
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