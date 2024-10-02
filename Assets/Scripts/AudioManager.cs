using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AudioManager : MonoBehaviour
{
    bool m_Started;
    [SerializeField] LayerMask m_LayerMask;
    [SerializeField] LayerMask m_ObsLayerMask;
    [SerializeField] AudioClip recorderEmptySound;
    private Collider[] hitColliders;
    private Collider[] obstacleColliders;
    private List<GameObject> wallSounds;

    public bool isPlaying = false;
    // Potential to include a default recording (Rufus)
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        m_Started = true;
        wallSounds = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        CollisionDetection();

        if(isPlaying && hitColliders.Length > 0){
            AdversarySounds();
        } 

        if(obstacleColliders.Length > 0){
            WallSound();
        }
    }

    void CollisionDetection(){
        // Potential for several detections (Rufus)
        hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
            for (int i = 0; i < hitColliders.Length; i++){
                    //print(hitColliders[i].name + " " + audioClip.name);
                }

        obstacleColliders = Physics.OverlapBox(transform.parent.transform.position, transform.parent.transform.localScale*4, Quaternion.identity,m_ObsLayerMask | m_LayerMask);
            for (int i = 0; i < obstacleColliders.Length; i++){
                    //print(hitColliders[i].name + " " + audioClip.name);
                }
    }

    public bool RecordSound(){
        for (int i = 0; i < hitColliders.Length; i++){
            audioClip = hitColliders[i].gameObject.GetComponent<AudioSource>().clip;
            print(audioClip);
            return true;
        }
        return false;
    }

    void AdversarySounds(){
        for (int i = 0; i < hitColliders.Length; i++){
            if(hitColliders[i].GetComponent<PuzzleElement>()){
                PuzzleElement puzzleElement = hitColliders[i].GetComponent<PuzzleElement>();
                AudioSource puzzleSource = puzzleElement.GetComponent<AudioSource>();
               if(puzzleElement.solutionClip == audioClip && audioClip != null){
                    puzzleSource.Stop();
                    puzzleSource.clip = puzzleElement.responseClip;
                    GetComponentsInParent<PlayerController>()[0].audioSource.Stop();
                    puzzleSource.Play();
                    puzzleSource.loop = false;
                    audioClip = null;
                    isPlaying = false;
                    hitColliders[i].enabled = false;

                    //Indikator för ljudfilen som förstörs
                    RecorderEmptyIndicator();
                    audioClip = null;
               }  
            }
        }
    }

    void WallSound(){
        for (int i = 0; i < obstacleColliders.Length; i++){
            //GameObject obstacle = obstacleColliders[i].GetComponent<GameObject>();

            if(wallSounds.Count < obstacleColliders.Length){
                GameObject objToSpawn = new GameObject("WallSound");
                objToSpawn.transform.SetParent(transform.parent.transform);
                objToSpawn.AddComponent<AudioSource>();
                objToSpawn.AddComponent<WallScript>();
                objToSpawn.GetComponent<WallScript>().vInput = GetComponentInParent<PlayerController>().verticalInput;
                objToSpawn.GetComponent<WallScript>().hInput = GetComponentInParent<PlayerController>().horizontalInput;
                wallSounds.Add(objToSpawn);
               
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started){
            Gizmos.DrawWireCube(transform.position, transform.localScale);
            Gizmos.DrawWireCube(transform.parent.transform.position, transform.parent.transform.localScale*4);
        }    
    }

    void RecorderEmptyIndicator() {
        AudioSource audioSource = GetComponentsInParent<PlayerController>()[0].audioSource;
        audioClip = recorderEmptySound;
        audioSource.PlayOneShot(audioClip);

    }
}
