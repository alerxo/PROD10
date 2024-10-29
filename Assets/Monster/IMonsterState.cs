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
    private const float idleCooldown = 3f;

    public IMonsterState Execute(Monster monster)
    {
        if (monster.investigatingState.CanInvestigate(monster))
        {
            idleTime = 0;

            return monster.investigatingState;
        }

        if ((idleTime += Time.deltaTime) >= idleCooldown)
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
    private const float patrolDistanceMax = 10;
    private const float stopDistance = 0.5f;

    public IMonsterState Execute(Monster monster)
    {
        if (monster.investigatingState.CanInvestigate(monster))
        {
            monster.StopPath();

            return monster.investigatingState;
        }

        if (!monster.HasDestination())
        {
            GetDestination(monster);
        }

        else if (Vector3.Distance(monster.transform.position, monster.NavMeshAgent.destination) < stopDistance)
        {
            monster.StopPath();

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
        if (!CanInvestigate(monster))
        {
            monster.StopPath();

            return monster.idleState;
        }

        if (monster.chasingState.CanChase(monster))
        {
            monster.ActionAudio3D.PlayOneShot(monster.ChaseTriggeredClip);

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
    public const float PlayerNoiseValueEnterValue = 4.5f;
    public const float PlayerNoiseValueExitValue = 1f;

    public IMonsterState Execute(Monster monster)
    {
        if (monster.PlayerObstacle.enabled)
        {
            monster.PlayerObstacle.enabled = false;
        }

        if (monster.PlayerNoiseValue <= PlayerNoiseValueExitValue)
        {
            monster.StopPath();
            monster.PlayerObstacle.enabled = true;

            return monster.idleState;
        }

        if (monster.attackingState.IsInRangeToAttack(monster))
        {
            monster.StopPath();
            monster.SetActionAudio3D(monster.AttackWindupClip);
            monster.attackingState.Reset();

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
    private const float range = 0.3f;
    private const float windupTimeInSeconds = 0.595f;
    private float timer = 0f;
    private bool isBlocked = false;

    public IMonsterState Execute(Monster monster)
    {
        if (isBlocked)
        {
            timer = 0f;

            monster.SetDefaultValues();

            return monster.stunnedState;
        }

        if ((timer += Time.deltaTime) >= windupTimeInSeconds)
        {
            timer = 0f;

            return monster.killingState;
        }

        return this;
    }

    public void Reset()
    {
        isBlocked = false;
    }

    public void Block(Monster monster)
    {
        if (monster.State == this)
        {
            monster.SetActionAudio3D(monster.BlockClip);
            isBlocked = true;
        }

        else
        {
            monster.SetActionAudio(monster.BlockFailClip);
        }
    }

    public bool IsInRangeToAttack(Monster monster)
    {
        return Vector2.Distance(
            new Vector2(monster.transform.position.x, monster.transform.position.z),
            new Vector2(monster.Player.transform.position.x, monster.Player.transform.position.z)) < range;
    }
}

public class Monster_Killing : IMonsterState
{
    public IMonsterState Execute(Monster monster)
    {
        monster.SetActionAudio3D(monster.KillClip);
        monster.Player.GetComponent<PlayerController>().Death();
        monster.SetDefaultValues();
        monster.PlayerObstacle.enabled = true;

        return monster.idleState;
    }
}

public class Monster_Stunned : IMonsterState
{
    private float timer = 0;
    private const float stunTume = 0.3f;

    public IMonsterState Execute(Monster monster)
    {
        if ((timer += Time.deltaTime) > stunTume)
        {
            timer = 0;

            return monster.scaredState;
        }

        return this;
    }
}

public class Monster_Scared : IMonsterState
{
    private const float fleeDistanceMargin = 5;
    private const float stopDistance = 0.5f;

    public IMonsterState Execute(Monster monster)
    {
        if (!monster.HasDestination())
        {
            GetDestination(monster);
        }

        else if (Vector3.Distance(monster.transform.position, monster.NavMeshAgent.destination) < stopDistance)
        {
            monster.StopPath();
            monster.PlayerObstacle.enabled = true;

            return monster.idleState;
        }

        return this;
    }

    private void GetDestination(Monster monster)
    {
        Vector3 position = monster.Player.transform.position + new Vector3(GetRandomCoordinate(monster), 0, GetRandomCoordinate(monster));
        monster.TrySetPath(position, Monster.RunSpeed);
    }

    private float GetRandomCoordinate(Monster monster)
    {
        float value = Random.Range(monster.PlayerObstacle.radius + fleeDistanceMargin, fleeDistanceMargin * 2);

        if (Random.Range(0, 2) == 0)
        {
            value = -value;
        }

        return value;
    }
}