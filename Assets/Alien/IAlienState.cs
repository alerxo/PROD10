using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IAlienState
{
    public IAlienState Execute(Alien alien);
}

public class Alien_Idle : IAlienState
{
    private float idleTime;
    private const float maxIdleTime = 5f;

    public IAlienState Execute(Alien alien)
    {
        if (alien.investigatingState.CanInvestigate(alien))
        {
            return alien.investigatingState;
        }

        if ((idleTime += Time.deltaTime) > maxIdleTime)
        {
            idleTime = 0;

            return alien.patrollingState;
        }

        return this;
    }
}

public class Alien_Patrolling : IAlienState
{
    private const float patrolDistanceMin = 5;
    private const float patrolDistanceMax = 15;
    private Vector3? destination;

    public IAlienState Execute(Alien alien)
    {
        if (alien.investigatingState.CanInvestigate(alien))
        {
            Clear(alien);

            return alien.investigatingState;
        }

        if (destination == null)
        {
            GetDestination(alien);
        }

        else if (Vector3.Distance(alien.transform.position, alien.NavMeshAgent.destination) < 0.5)
        {
            Clear(alien);

            return alien.idleState;
        }

        return this;
    }

    private void GetDestination(Alien alien)
    {
        destination = alien.transform.position + new Vector3(GetRandomCoordinate(), 0, GetRandomCoordinate());
        NavMeshPath path = alien.TryGetPath(destination.Value);

        if (path != null)
        {
            alien.NavMeshAgent.SetPath(path);
        }

        else
        {
            destination = null;
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

    private void Clear(Alien alien)
    {
        destination = null;
        alien.NavMeshAgent.SetDestination(alien.transform.position);
    }
}

public class Alien_Investigating : IAlienState
{
    private Clue currentClue;
    private Vector3? destination;
    private const float destinationMargin = 5;

    public IAlienState Execute(Alien alien)
    {
        if (!CanInvestigate(alien))
        {
            Clear(alien);

            return alien.idleState;
        }

        if (alien.attackingState.CanAttack(alien))
        {
            Clear(alien);

            return alien.attackingState;
        }

        if (destination == null)
        {
            GetDestination(alien);
        }

        else if (Vector3.Distance(alien.transform.position, destination.Value) < 1.5f)
        {
            Clear(alien);

            return alien.idleState;
        }

        return alien.investigatingState;
    }

    private void GetDestination(Alien alien)
    {
        float margin = destinationMargin * ClueSystem.GetClueStrength(alien, currentClue);
        destination = currentClue.Position + new Vector3(Random.Range(-margin, margin), 0, Random.Range(-margin, margin));

        NavMeshPath path = new();

        if (alien.NavMeshAgent.CalculatePath(destination.Value, path))
        {
            alien.NavMeshAgent.SetPath(path);
        }

        else
        {
            destination = null;
        }
    }

    public bool CanInvestigate(Alien alien)
    {
        if (!ClueSystem.IsClueValid(currentClue))
        {
            currentClue = null;

            return false;
        }

        return alien.TryGetPath(currentClue.Position) != null;
    }

    public void SetClue(Alien alien, Clue newClue)
    {
        if (currentClue == null)
        {
            currentClue = newClue;
            destination = null;
        }

        else
        {
            currentClue = ClueSystem.GetLargerClue(currentClue, newClue, alien);
        }
    }

    private void Clear(Alien alien)
    {
        destination = null;
        currentClue = null;
        alien.NavMeshAgent.SetDestination(alien.transform.position);
    }
}

public class Alien_Attacking : IAlienState
{
    private const float attackRange = 1.5f;

    public IAlienState Execute(Alien alien)
    {
        alien.Player.GetComponent<PlayerController>().Death();

        return alien.idleState;
    }

    public bool CanAttack(Alien alien)
    {
        return Vector3.Distance(alien.transform.position, alien.Player.position) < attackRange;
    }
}