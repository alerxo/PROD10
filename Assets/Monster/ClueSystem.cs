using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ClueSystem
{
    public static UnityAction<Clue> OnClueTriggered;

    public const float ClueAliveTimeInSeconds = 3f;
    public const float ClueRange = 30f;

    public static void TriggerClue(float strength, Vector3 position, GameObject parent)
    {
        OnClueTriggered?.Invoke(new Clue(strength, position, parent));
    }

    public static bool IsClueValid(Clue clue)
    {
        return Time.time - clue.TriggerTime < ClueAliveTimeInSeconds;
    }

    public static Clue GetLargerClue(Clue first, Clue second, Monster monster)
    {
        if (first == null)
        {
            return second;
        }

        else if (second == null)
        {
            return first;
        }

        return GetClueStrength(monster, first) > GetClueStrength(monster, second) ? first : second;
    }

    public static float GetClueStrength(Monster monster, Clue clue)
    {
        float time = 1 - (Mathf.Min(Time.time - clue.TriggerTime, ClueAliveTimeInSeconds) / ClueAliveTimeInSeconds);
        float distance = 1 - (Mathf.Min(Vector3.Distance(monster.transform.position, clue.Position), ClueRange) / ClueRange);

        return clue.Strength * time * distance;
    }
}

public class Clue
{
    public float Strength { get; private set; }
    public Vector3 Position { get; private set; }
    public GameObject Parent { get; private set; }
    public float TriggerTime { get; private set; }

    public Clue(float strength, Vector3 position, GameObject parent)
    {
        Assert.IsTrue(strength > 0 && strength <= 1);
        Assert.IsTrue(parent != null);
        Strength = strength;
        Position = position;
        Parent = parent;
        TriggerTime = Time.time;
    }

    public void Consume()
    {
        TriggerTime = float.MinValue;
    }
}
