using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParameterManager : MonoBehaviour
{
    public GameObject[] walls;
    [SerializeField] private AudioClip wallSound;

    private void Awake()
    {
        //Remove audioclip from walls if false
        if (!ParameterSystem.Get().ActiveSound)
        {
            foreach (GameObject wall in walls)
            {
                if(wall == null) {
                    Debug.Log("PlayerParameter: null");
                    return;
                }

                wall.GetComponent<AudioSource>().clip = null; 
            }
        }

    }
}
