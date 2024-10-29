using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParameterSystem
{
    private static Parameters Parameters;
    private static readonly string ParameterFile = $"{Application.dataPath}/ParameterFile.Json";

    public static Parameters Get()
    {
        if (Parameters != null) return Parameters;

        if (!File.Exists(ParameterFile))
        {
            Parameters = new Parameters();
            File.WriteAllText(ParameterFile, JsonUtility.ToJson(Parameters));
        }

        else
        {
            Parameters = JsonUtility.FromJson<Parameters>(File.ReadAllText(ParameterFile));
        }

        return Parameters;
    }
}

public class Parameters
{
    public float MonsterWalkSpeed = 3f;
    public float MonsterRunSpeed = 7f;
    public float PlayerNoiseFalloff = 0.7f;
    public float PlayerNoiseFastFalloff = 2f;

    public int ActiveIntroClip = 0;
}