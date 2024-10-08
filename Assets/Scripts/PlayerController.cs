using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float MoveDelay;
    [SerializeField] LayerMask m_LayerMask;
    [SerializeField] AudioClip playerStep;
    [SerializeField] AudioClip recordSound;
    [SerializeField] AudioClip recordingFailedSound;
    [SerializeField] AudioClip recordPlayingSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip[] swooshSounds;
    [SerializeField] AudioClip[] stepSoundsWood;
    GameObject mainCam;   
    GameObject blindCam;     
    GameObject audioManager;
    public AudioSource audioSource;
    public float horizontalInput;
    public float verticalInput;
    public Vector3 moveDirection;
    private float timer;
    public Rigidbody rb;
    private Collider[] ventCollider;
    private bool isMoving = false;
    private int foleyType = 0;
    private int lastStep = -1; //Keep track of last step sound used

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        blindCam = GameObject.FindGameObjectWithTag("BlindCamera");
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        blindCam.SetActive(false);

        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(moveDirection * speed * Time.deltaTime);

        timer -= Time.deltaTime;
        isMoving = false;

        if(timer <= 0){

            float rawHorizontalInput = Input.GetAxis("Horizontal");
            float rawVerticalInput = Input.GetAxis("Vertical");

            horizontalInput = (float)Math.Ceiling(Math.Round(Input.GetAxis("Horizontal"), 1)) * speed;
            verticalInput = (float)Math.Ceiling(Math.Round(Input.GetAxis("Vertical"), 1)) * speed;

            if(rawHorizontalInput < 0.0f){
                horizontalInput = speed * -1;
            }
            if(rawVerticalInput < 0.0f){
               verticalInput = speed * -1;
            }

            if(horizontalInput != 0.0f) verticalInput = 0.0f;
            if(verticalInput != 0.0f) horizontalInput = 0.0f;

            Vector3 localMoveDirection = new Vector3(horizontalInput, 0f, verticalInput);
            
            moveDirection = transform.TransformDirection(localMoveDirection);
            
            //transform.Translate(moveDirection * speed); 
            rb.velocity = moveDirection * speed; //Can also move diagonally but scuffed (Rufus)

            if(horizontalInput != 0.0f || verticalInput != 0.0f){
                timer = MoveDelay + Time.deltaTime;
                isMoving = true;
                if(isMoving){
                    //int index = UnityEngine.Random.Range(0, stepSoundsWood.Length);
                    //audioSource.PlayOneShot(stepSoundsWood[index]);
                    

                    //audioSource.clip = playerStep; 
                    //audioSource.Play();
                    PlayFoleySound();
                }

            }
            
            if(localMoveDirection.magnitude > 0) // Calls clue event for alien investigate behaviour
            {
                ClueSystem.TriggerClue(1, transform.position);
            }
        }

        Controls(Input.inputString);

    }

    void Controls(string input){
        switch(input){
            case "q": 
                if(rb.velocity.magnitude == 0) rb.rotation *= Quaternion.Euler(0,-90,0);
                audioSource.PlayOneShot(swooshSounds[0]);
                break;
            case "e": 
                if(rb.velocity.magnitude == 0) rb.rotation *= Quaternion.Euler(0,90,0);
                audioSource.PlayOneShot(swooshSounds[1]);
                break;
            case "r":
                audioSource.Stop();
                audioManager.GetComponent<AudioManager>().isPlaying = false;
                //Indikatorer för när man lyckas eller failar att spela in ljud
                if (audioManager.GetComponent<AudioManager>().RecordSound())
                {
                    audioSource.PlayOneShot(recordSound);
                } else {
                    audioSource.PlayOneShot(recordingFailedSound);
                }
                break;
            case "p":
                //Feedback för när man spelar upp
                audioSource.PlayOneShot(recordPlayingSound);
                
                audioSource.clip = audioManager.GetComponent<AudioManager>().audioClip;
                print(audioSource.clip);

                //Inspelat ljud spelas upp
                audioSource.Play();
                audioManager.GetComponent<AudioManager>().isPlaying = true;
                break;
            case "l":
                Respawn();
                break;
            case "h":
                DaugtherCall();
                break;
            case "c": //Debug purpose, should not be available in shipping (Rufus)
                if(mainCam.activeInHierarchy){
                    mainCam.SetActive(false);
                    blindCam.SetActive(true);
                }
                else if(!mainCam.activeInHierarchy){
                    mainCam.SetActive(true);
                    blindCam.SetActive(false);
                }
                break;
            default: 
                break;
        }
    }
    public bool Death(){
        Respawn();
        return true;
    }

    void Respawn(){
        timer = 0;

        rb.position = new Vector3(0,0,0);
        rb.rotation = Quaternion.Euler(0,0,0);

        audioManager.GetComponent<AudioManager>().audioClip = null;
        audioManager.GetComponent<AudioManager>().isPlaying = false;
        
        audioSource.Stop();
        audioSource.clip = null;

        GameObject[] puzzleElement = GameObject.FindGameObjectsWithTag("PuzzleElement");

        for(int i = 0; i < puzzleElement.Length; i++){
            puzzleElement[i].GetComponent<PuzzleElement>().Reset();
        }

        audioSource.PlayOneShot(deathSound);
    }

    void DaugtherCall(){
        ventCollider = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
        for (int i = 0; i < ventCollider.Length; i++){
            ventCollider[i].GetComponent<AudioSource>().Play();
        }
    }


    //Play foley sounds based on current zone
    private void OnTriggerEnter(Collider other) {
        ChangeFoleyType(other.gameObject);
    }

    //Change foley type based on tag
    //Consider adding gameobjects to array, and changing based on name at index
    private void ChangeFoleyType(GameObject zone) {
        string zoneTag = zone.tag;
        switch (zoneTag)
        {
            case "ZoneWood":
            foleyType = 1;
            break;
            
            case "ZoneStone":
            foleyType = 2;
            break;

            default:
            foleyType = 0;
            break;
        }
        print(foleyType); 
    }

    private void PlayFoleySound() {
        if(stepSoundsWood.Length <= 0) {
            return;
        }

        int min = 0;
        int max = stepSoundsWood.Length;

        switch (foleyType)
        {
            case 1: 
            min = 6; 
            max = 8;
            break;

            case 2:
            min = 3;
            max = 5;
            break;

            default:
            min = 0; 
            max = 2;
            break;
        }

        int index = UnityEngine.Random.Range(min, max + 1);

        while (lastStep == index) {
            index = UnityEngine.Random.Range(min, max + 1);
        }
        lastStep = index;
        audioSource.PlayOneShot(stepSoundsWood[index]);
        //UnityEngine.Debug.Log(index);
    }
}
