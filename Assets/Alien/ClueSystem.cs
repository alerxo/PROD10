using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClueSystem
{
    public static UnityAction<Clue> OnClueTriggered;

    public const float ClueAliveTimeInSeconds = 3;
    public const float ClueRange = 30;

    public static void TriggerClue(float loudness, Vector3 position)
    {
        OnClueTriggered?.Invoke(new Clue(loudness, position));
    }

    public static float GetClueStrength(Alien alien, Clue clue)
    {
        float loudness = clue.Loudness;
        float time = 1 - (Mathf.Min(Time.time - clue.time, ClueAliveTimeInSeconds) / ClueAliveTimeInSeconds);
        float distance = 1 - (Mathf.Min(Vector3.Distance(alien.transform.position, clue.Position), ClueRange) / ClueRange);

        return loudness * time * distance;
    }

    public static bool IsClueValid(Clue clue)
    {
        return clue != null && Time.time - clue.time > ClueAliveTimeInSeconds;
    }

    public static Clue GetLargerClue(Clue first, Clue second, Alien alien)
    {
        return GetClueStrength(alien, first) > GetClueStrength(alien, second) ? first : second;
    }
}

public class Clue
{
    public float Loudness { get; private set; }
    public Vector3 Position { get; private set; }
    public readonly float time;

    public Clue(float loudness, Vector3 position)
    {
        Loudness = loudness;
        Position = position;
        time = Time.time;
    }
}
