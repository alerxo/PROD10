using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClueSystem
{
    public static UnityAction<Clue> OnClueTriggered;

    public const float ClueAliveTimeInSeconds = 10;
    public const float ClueRange = 20;

    public static void TriggerClue(Clue clue)
    {
        OnClueTriggered?.Invoke(clue);
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
