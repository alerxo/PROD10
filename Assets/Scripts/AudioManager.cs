using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class AudioManager : MonoBehaviour
{
    bool m_Started;
    [SerializeField] LayerMask m_LayerMask;
    [SerializeField] LayerMask m_LayerSourceMask;
    [SerializeField] LayerMask m_ObsLayerMask;
    [SerializeField] LayerMask m_BlockLayer;
    [SerializeField] AudioClip recorderEmptySound;
    [SerializeField] AudioClip interactionDistance;
    [SerializeField] int wallAmount;
    public Collider[] hitColliders;
    public Collider[] obstacleColliders;
    private List<Collider> foundObstacleColliders;
    private List<GameObject> wallSounds;

    public bool isPlaying = false;
    public bool isInteracting = false;
    // Potential to include a default recording (Rufus)
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        m_Started = true;
        wallSounds = new List<GameObject>();
        foundObstacleColliders = new List<Collider>();

        /*for(int i = 0; i < wallAmount; i++){
                GameObject objToSpawn = new GameObject("WallSound"+i);

                objToSpawn.SetActive(false);

                objToSpawn.AddComponent<AudioSource>();
                objToSpawn.GetComponent<AudioSource>().playOnAwake = true;
                objToSpawn.GetComponent<AudioSource>().loop = true;
                objToSpawn.GetComponent<AudioSource>().spatialBlend = 1;
                objToSpawn.GetComponent<AudioSource>().enabled = false;

                objToSpawn.AddComponent<Rigidbody>();

                objToSpawn.AddComponent<WallScript>();

                objToSpawn.AddComponent<CapsuleCollider>();
                //objToSpawn.GetComponent<CapsuleCollider>().isTrigger = true;
                objToSpawn.GetComponent<WallScript>().m_BlockLayer = m_BlockLayer;
                
                wallSounds.Add(objToSpawn);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        CollisionDetection();

        if(isPlaying && hitColliders.Length > 0){
            AdversarySounds();
        } 

        /*if(obstacleColliders.Length > 0){
            WallSound();
        }*/
    }

void CollisionDetection()
{
    RaycastHit hit;

    if (Physics.Raycast(transform.parent.transform.position, transform.parent.transform.forward, out hit, 2f, m_LayerSourceMask | m_LayerMask))
    {
        if(!GetComponent<AudioSource>().isPlaying && !isInteracting){
            GetComponent<AudioSource>().Play();
            isInteracting = true;
        }
 
        hitColliders = new Collider[1];
        hitColliders[0] = hit.collider;

    }
    else
    {
        hitColliders = new Collider[0];
        isInteracting = false;
    }

    float detectionDistance = 5f;
    List<Collider> foundObstacles = new List<Collider>();

    Vector3[] directions = {
        transform.parent.transform.forward,    
        -transform.parent.transform.forward,   
        transform.parent.transform.right,      
        -transform.parent.transform.right      
    };

    foreach (Vector3 dir in directions)
    {
        if (Physics.Raycast(transform.parent.transform.position, dir, out hit, detectionDistance, m_ObsLayerMask | m_LayerMask | m_BlockLayer))
        {
            foundObstacles.Add(hit.collider);
        }
    }

    obstacleColliders = foundObstacles.ToArray();
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
                    //puzzleSource.Play();
                    puzzleSource.loop = false;
                    audioClip = null;
                    isPlaying = false;
                    //hitColliders[i].isTrigger = true;
                    puzzleElement.solved = true;

                    //Indikator för ljudfilen som förstörs
                    RecorderEmptyIndicator();
                    audioClip = null;
               }  
            }
        }
    }

/*void WallSound()
{
    for (int i = 0; i < obstacleColliders.Length; i++)
    {
        bool alreadyExists = false;
        
        for (int j = 0; j < foundObstacleColliders.Count; j++)
        {
            if (obstacleColliders[i] == foundObstacleColliders[j] || obstacleColliders[i].CompareTag("Pillar"))
            {
                alreadyExists = true;
                break;
            }
        }

        if (!alreadyExists)
        {
            foundObstacleColliders.Add(obstacleColliders[i]);

            wallSounds[i].transform.SetParent(transform.parent.transform);
            wallSounds[i].transform.position = transform.parent.transform.position;
            
            wallSounds[i].GetComponent<AudioSource>().clip = obstacleColliders[i].GetComponent<AudioSource>().clip;
            wallSounds[i].GetComponent<AudioSource>().volume = obstacleColliders[i].GetComponent<AudioSource>().volume;
            wallSounds[i].GetComponent<AudioSource>().rolloffMode =  obstacleColliders[i].GetComponent<AudioSource>().rolloffMode;
            wallSounds[i].GetComponent<AudioSource>().maxDistance =  obstacleColliders[i].GetComponent<AudioSource>().maxDistance;
            wallSounds[i].GetComponent<AudioSource>().SetCustomCurve(AudioSourceCurveType.CustomRolloff, obstacleColliders[i].GetComponent<AudioSource>().GetCustomCurve(AudioSourceCurveType.CustomRolloff));
            wallSounds[i].SetActive(true);
            
            wallSounds[i].GetComponent<WallScript>().vInput = GetComponentInParent<PlayerController>().verticalInput;
            wallSounds[i].GetComponent<WallScript>().hInput = GetComponentInParent<PlayerController>().horizontalInput;
            wallSounds[i].GetComponent<WallScript>().assignedObject = obstacleColliders[i].gameObject;

            wallSounds.RemoveAt(i); 
        }
    }
}*/

void OnDrawGizmos()
{
    Gizmos.color = Color.red;
    
    if (m_Started)
    {
        Gizmos.DrawWireSphere(transform.parent.transform.position + transform.parent.transform.forward, 0.5f);

        float detectionDistance = 5f; 
        Vector3[] directions = {
            transform.parent.transform.forward,   
            -transform.parent.transform.forward,  
            transform.parent.transform.right,      
            -transform.parent.transform.right      
        };

        Gizmos.color = Color.green; 
        foreach (Vector3 dir in directions)
        {
            Gizmos.DrawRay(transform.parent.transform.position, dir * detectionDistance);
            Gizmos.DrawSphere(transform.parent.transform.position + dir * detectionDistance, 0.2f); 
        }
    }
}

    void RecorderEmptyIndicator() {
        AudioSource audioSource = GetComponentsInParent<PlayerController>()[0].audioSource;
        audioClip = recorderEmptySound;
        audioSource.PlayOneShot(audioClip);

    }
}
