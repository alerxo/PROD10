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
    private const float maxIdleTime = 5;

    public IAlienState Execute(Alien alien)
    {
        if (alien.investigatingState.HasTrail())
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
    private const float patrolDistance = 5;
    private Vector3? destination;

    public IAlienState Execute(Alien alien)
    {
        if (destination == null)
        {
            GetDestination(alien);
        }

        else if (alien.investigatingState.HasTrail())
        {
            destination = null;

            return alien.investigatingState;
        }

        else if (Vector3.Distance(alien.transform.position, alien.NavMeshAgent.destination) > 0.5)
        {
            destination = null;

            return alien.idleState;
        }

        return this;
    }

    private void GetDestination(Alien alien)
    {
        destination = alien.transform.position + new Vector3(Random.Range(-patrolDistance, patrolDistance), 0, Random.Range(-patrolDistance, patrolDistance));
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
}

public class Alien_Investigating : IAlienState
{
    private Vector3? trail;

    public IAlienState Execute(Alien alien)
    {
        if (!HasTrail())
        {
            return alien.idleState;
        }

        if (alien.attackingState.CanAttack(alien))
        {
            return alien.attackingState;
        }

        alien.NavMeshAgent.SetDestination(trail.Value);

        return alien.investigatingState;
    }

    public bool HasTrail()
    {
        return trail != null;
    }

    public void SetTrail(Vector3 source)
    {
        trail = source;
    }
}

public class Alien_Attacking : IAlienState
{
    private const float attackRange = 1f;

    public IAlienState Execute(Alien alien)
    {
        alien.HellYeah();

        return this;
    }

    public bool CanAttack(Alien alien)
    {
        return Vector3.Distance(alien.transform.position, alien.Player.position) < attackRange;
    }
}
