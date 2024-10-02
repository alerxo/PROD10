using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterState
{
    public IMonsterState Execute(Monster monster);
}

public class Monster_Idle : IMonsterState
{
    private float idleTime = 0f;
    private const float maxIdleTime = 3f;

    public IMonsterState Execute(Monster monster)
    {
        if (monster.investigatingState.CanInvestigate(monster))
        {
            return monster.investigatingState;
        }

        if ((idleTime += Monster.stateUpdateCooldownInSeconds + Time.deltaTime) >= maxIdleTime)
        {
            idleTime = 0;

            return monster.patrollingState;
        }

        return this;
    }
}

public class Monster_Patrolling : IMonsterState
{
    private const float patrolDistanceMin = 5;
    private const float patrolDistanceMax = 15;
    private const float stopDistance = 0.5f;

    public IMonsterState Execute(Monster monster)
    {
        if (!monster.WalkingAudio.isPlaying)
        {
            monster.WalkingAudio.Play();
        }

        if (monster.investigatingState.CanInvestigate(monster))
        {
            monster.StopPath();
            monster.WalkingAudio.Stop();

            return monster.investigatingState;
        }

        if (!monster.HasDestination())
        {
            GetDestination(monster);
        }

        else if (Vector3.Distance(monster.transform.position, monster.NavMeshAgent.destination) < stopDistance)
        {
            monster.StopPath();
            monster.WalkingAudio.Stop();

            return monster.idleState;
        }

        return this;
    }

    private void GetDestination(Monster monster)
    {
        Vector3 position = monster.transform.position + new Vector3(GetRandomCoordinate(), 0, GetRandomCoordinate());
        monster.TrySetPath(position, Monster.WalkSpeed);
    }

    private float GetRandomCoordinate()
    {
        float value = Random.Range(patrolDistanceMin, patrolDistanceMax);

        if (Random.Range(0, 2) == 0)
        {
            value = -value;
        }

        return value;
    }
}

public class Monster_Investigating : IMonsterState
{
    private const float destinationMargin = 7f;
    private const float stopDistance = 1.5f;
    private const int setPathAttemptCount = 10;

    public IMonsterState Execute(Monster monster)
    {
        if (!monster.WalkingAudio.isPlaying)
        {
            monster.WalkingAudio.Play();
        }

        if (!CanInvestigate(monster))
        {
            monster.StopPath();
            monster.WalkingAudio.Stop();

            return monster.idleState;
        }

        if (monster.chasingState.CanChase(monster))
        {
            monster.WalkingAudio.Stop();

            return monster.chasingState;
        }

        if (!monster.HasDestination())
        {
            GetDestination(monster);
        }

        else if (Vector3.Distance(monster.transform.position, monster.NavMeshAgent.destination) < stopDistance)
        {
            monster.CurrentClue.Consume();
        }

        return this;
    }

    private void GetDestination(Monster monster)
    {
        for (int i = 0; i < setPathAttemptCount; i++)
        {
            float margin = destinationMargin * (1 - ClueSystem.GetClueStrength(monster, monster.CurrentClue));
            Vector3 position = monster.CurrentClue.Position + new Vector3(Random.Range(-margin, margin), 0, Random.Range(-margin, margin));

            if (monster.TrySetPath(position, Monster.WalkSpeed))
            {
                break;
            }
        }
    }

    public bool CanInvestigate(Monster monster)
    {
        return monster.CurrentClue != null && monster.IsValidDestination(monster.CurrentClue.Position);
    }
}

public class Monster_Chasing : IMonsterState
{
    private const float PlayerNoiseValueEnterValue = 3.5f;
    private const float PlayerNoiseValueExitValue = 1f;

    public IMonsterState Execute(Monster monster)
    {
        if (!monster.ChasingAudio.isPlaying)
        {
            monster.ChasingAudio.Play();
        }

        if (monster.PlayerNoiseValue <= PlayerNoiseValueExitValue)
        {
            monster.StopPath();
            monster.ChasingAudio.Stop();

            return monster.idleState;
        }

        if (monster.attackingState.CanAttack(monster))
        {
            monster.StopPath();
            monster.ChasingAudio.Stop();

            return monster.attackingState;
        }

        monster.TrySetPath(monster.Player.transform.position, Monster.RunSpeed);

        return this;
    }

    public bool CanChase(Monster monster)
    {
        return monster.PlayerNoiseValue > PlayerNoiseValueEnterValue;
    }
}

public class Monster_Attacking : IMonsterState
{
    private const float attackRange = 1f;
    private const float clueAttackWindow = 0.4f;

    public IMonsterState Execute(Monster monster)
    {
        monster.StartCoroutine(monster.Attack());

        return null;
    }

    public bool CanAttack(Monster monster)
    {
        return monster.CurrentClue != null && (Time.time - monster.CurrentClue.TriggerTime) < clueAttackWindow &&
            Vector3.Distance(monster.transform.position, monster.Player.transform.position) < attackRange;
    }
}