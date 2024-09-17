using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject[] puzzleList;
    public void AddPuzzle(GameObject puzzle){
        for(int i = 0; i <= puzzleList.Length; i++){
            puzzleList[i] = puzzle;
        }
    }

    public void RespawnPuzzle(){
        //GameObject[] existingPieces = GameObject.FindGameObjectsWithTag("");

        
        for(int i = 0; i < puzzleList.Length; i++){

        }
    }
}
