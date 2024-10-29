using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject Player { get; private set; }
    public NavMeshObstacle PlayerObstacle { get; private set; }
    public PlayerInput PlayerInput;
    public NavMeshAgent NavMeshAgent { get; private set; }
    public const float WalkSpeed = 3f;
    public const float RunSpeed = 7f;

    public IMonsterState State {  get; private set; }
    public readonly Monster_Idle idleState = new();
    public readonly Monster_Patrolling patrollingState = new();
    public readonly Monster_Investigating investigatingState = new();
    public readonly Monster_Chasing chasingState = new();
    public readonly Monster_Attacking attackingState = new();
    public readonly Monster_Stunned stunnedState = new();
    public readonly Monster_Killing killingState = new();
    public readonly Monster_Scared scaredState = new();

    public AudioSource AmbienceAudio;
    public AudioSource FootStepsAudio;
    public AudioSource ActionAudio;
    public AudioSource ActionAudio3D;
    public AudioSource NormalMusic;
    public AudioSource IntenseMusic;

    public AudioClip[] AmbienceClips;

    public AudioClip[] FootStepClips;
    public AudioClip ChaseTriggeredClip;
    public AudioClip AttackWindupClip;
    public AudioClip BlockClip;
    public AudioClip BlockFailClip;
    public AudioClip KillClip;

    public Clue CurrentClue { get; private set; }
    [field: SerializeField] public float PlayerNoiseValue { get; private set; }
    public const float PlayerNoiseFalloff = 0.7f;
    public const float PlayerNoiseFastFalloff = 2f;

    private float distance = 0;
    private Vector3 last = Vector3.zero;
    private const float step = 1.5f;

    private bool canBlock = true;
    private const float blockCooldown = 1.5f;

    [SerializeField] private string stateName;

    private void Awake()
    {
        PlayerInput = new();
        PlayerInput.Enable();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Player = FindObjectOfType<PlayerController>().gameObject;
        PlayerObstacle = Player.GetComponent<NavMeshObstacle>();
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
        if (canBlock)
        {
            attackingState.Block(this);
            canBlock = false;
            StartCoroutine(BlockTimer());
        }
    }

    private IEnumerator BlockTimer()
    {
        yield return new WaitForSeconds(blockCooldown);
        canBlock = true;
    }

    private void Start()
    {
        State = idleState;
        StopPath();
    }

    private void Update()
    {
        if (Player == null) return;

        PlayFootSteps();
        ManageAudio();
        ManageMusic();
        UpdateNoise();
        UpdateState();
    }

    private void PlayFootSteps()
    {
        if (State is Monster_Idle) return;

        distance += Vector3.Distance(transform.position, last);

        if (distance >= step)
        {
            distance -= step;
            FootStepsAudio.PlayOneShot(FootStepClips[Random.Range(0, FootStepClips.Length)]);
        }

        last = transform.position;
    }

    private void ManageAudio()
    {
        switch (State)
        {
            case Monster_Idle:
            case Monster_Patrolling:
            case Monster_Investigating:
                ManageAmbience(true);
                break;

            case Monster_Chasing:
            case Monster_Attacking:
            case Monster_Stunned:
            case Monster_Killing:
            case Monster_Scared:
                ManageAmbience(false);
                break;

            default:
                Debug.LogError("Defaulted");
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

    public void SetActionAudio(AudioClip clip)
    {
        ActionAudio.Stop();
        ActionAudio.PlayOneShot(clip);
    }

    public void SetActionAudio3D(AudioClip clip)
    {
        ActionAudio3D.Stop();
        ActionAudio3D.PlayOneShot(clip);
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
            PlayerNoiseValue -= (State == scaredState || PlayerNoiseValue > Monster_Chasing.PlayerNoiseValueEnterValue + PlayerNoiseFalloff ?
                PlayerNoiseFastFalloff : PlayerNoiseFalloff) * Time.deltaTime;
        }
    }

    private void UpdateState()
    {
        State = State.Execute(this);
        stateName = State.ToString();
    }

    private void ClueTriggered(Clue second)
    {
        if (!CanGetToDestination(second.Position)) return;

        if (second.Parent == Player)
        {
            PlayerNoiseValue += second.Strength;
        }

        CurrentClue = ClueSystem.GetLargerClue(CurrentClue, second, this);
    }

    public bool TrySetPath(Vector3 position, float speed)
    {
        if (!CanGetToDestination(position) && NavMesh.SamplePosition(position, out NavMeshHit hit, 6, -1))
        {
            position = hit.position;
        }

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
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 6, -1))
        {
            position = hit.position;
        }

        return CanGetToDestination(position);
    }

    private bool CanGetToDestination(Vector3 position)
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
        PlayerNoiseValue = 0;
        CurrentClue = null;
    }
}