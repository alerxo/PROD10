using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterState
{
    public IMonsterState Execute(Monster monster);
}

public class Monster_Idle : IMonsterState
{
    private float idleTime;
    private const float maxIdleTime = 5f;

    public IMonsterState Execute(Monster monster)
    {
        if (monster.investigatingState.CanInvestigate(monster))
        {
            return monster.investigatingState;
        }

        if ((idleTime += Time.deltaTime) > maxIdleTime)
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
    private Vector3? destination;

    public IMonsterState Execute(Monster monster)
    {
        if (monster.investigatingState.CanInvestigate(monster))
        {
            Clear(monster);

            return monster.investigatingState;
        }

        if (destination == null)
        {
            GetDestination(monster);
        }

        else if (Vector3.Distance(monster.transform.position, destination.Value) < 0.5)
        {
            Clear(monster);

            return monster.idleState;
        }

        return this;
    }

    private void GetDestination(Monster monster)
    {
        Vector3 position = monster.transform.position + new Vector3(GetRandomCoordinate(), 0, GetRandomCoordinate());

        if (monster.TrySetPath(position))
        {
            destination = position;
        }
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

    private void Clear(Monster monster)
    {
        destination = null;
        monster.ResetPath();
    }
}

public class Monster_Investigating : IMonsterState
{
    private Clue currentClue;
    private Vector3? destination;
    private const float destinationMargin = 7f;

    public IMonsterState Execute(Monster monster)
    {
        if (!CanInvestigate(monster))
        {
            Clear(monster);

            return monster.idleState;
        }

        if (monster.attackingState.CanAttack(monster))
        {
            Clear(monster);

            return monster.attackingState;
        }

        if (destination == null)
        {
            GetDestination(monster);
        }

        else if (Vector3.Distance(monster.transform.position, destination.Value) < 1.5f)
        {
            Clear(monster);

            return monster.idleState;
        }

        return monster.investigatingState;
    }

    private void GetDestination(Monster monster)
    {
        for (int i = 0; i < 10; i++)
        {
            float margin = destinationMargin * (1 - ClueSystem.GetClueStrength(monster, currentClue));
            Vector3 position = currentClue.Position + new Vector3(Random.Range(-margin, margin), 0, Random.Range(-margin, margin));

            if (monster.TrySetPath(position))
            {
                destination = position;
                break;
            }
        }
    }

    public bool CanInvestigate(Monster monster)
    {
        if (!ClueSystem.IsClueValid(currentClue))
        {
            currentClue = null;
            destination = null;

            return false;
        }

        return monster.IsValidDestination(currentClue.Position);
    }

    public void SetClue(Monster monster, Clue clue)
    {
        currentClue = ClueSystem.GetLargerClue(currentClue, clue, monster);
    }

    private void Clear(Monster monster)
    {
        destination = null;
        currentClue = null;
        monster.ResetPath();
    }
}

public class Monster_Chase : IMonsterState
{
    public IMonsterState Execute(Monster monster)
    {
        throw new System.NotImplementedException();
    }
}

public class Monster_Attacking : IMonsterState
{
    private const float attackRange = 1.5f;

    public IMonsterState Execute(Monster monster)
    {
        monster.Player.GetComponent<PlayerController>().Death();

        return monster.idleState;
    }

    public bool CanAttack(Monster monster)
    {
        return Vector3.Distance(monster.transform.position, monster.Player.position) < attackRange;
    }
}