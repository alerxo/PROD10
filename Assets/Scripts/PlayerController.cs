using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;


public class PlayerController : MonoBehaviour
{
    public float speed;
    [SerializeField] private float MoveDelay;
    [SerializeField] LayerMask m_LayerMask;
    [SerializeField] AudioClip playerStep;
    [SerializeField] AudioClip recordSound;
    [SerializeField] AudioClip recordingFailedSound;
    [SerializeField] AudioClip recordPlayingSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip[] swooshSounds;
    [SerializeField] AudioClip[] stepSoundsWood;
    [SerializeField] AudioClip[] direction;
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
    public bool isPaused;
    public EventLogger eventLogger;

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

        GameObject loggerObject = GameObject.FindGameObjectWithTag("EventLogger");

        if (loggerObject != null)
        {
            eventLogger = loggerObject.GetComponent<EventLogger>();
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) return;
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
                ClueSystem.TriggerClue(1, transform.position, gameObject);
            }
        }

            if (Input.GetKeyDown(KeyCode.Q)) Controls(KeyCode.Q);
            if (Input.GetKeyDown(KeyCode.E)) Controls(KeyCode.E);
            if (Input.GetKeyDown(KeyCode.R)) Controls(KeyCode.R);
            if (Input.GetKeyDown(KeyCode.P)) Controls(KeyCode.P);
            if (Input.GetKeyDown(KeyCode.L)) Controls(KeyCode.L);
            if (Input.GetKeyDown(KeyCode.H)) Controls(KeyCode.H);
            if (Input.GetKeyDown(KeyCode.C)) Controls(KeyCode.C);
            if (Input.GetKeyDown(KeyCode.Tab)) Controls(KeyCode.Tab);

    }

void Controls(KeyCode input){
    switch(input){
        case KeyCode.Q: 
            if(rb.velocity.magnitude == 0) rb.rotation *= Quaternion.Euler(0, -90, 0);
            audioSource.PlayOneShot(swooshSounds[0]);
            break;
        case KeyCode.E: 
            if(rb.velocity.magnitude == 0) rb.rotation *= Quaternion.Euler(0, 90, 0);
            audioSource.PlayOneShot(swooshSounds[1]);
            break;
        case KeyCode.R:
            audioSource.Stop();
            audioManager.GetComponent<AudioManager>().isPlaying = false;

            if (audioManager.GetComponent<AudioManager>().RecordSound())
            {
                audioSource.PlayOneShot(recordSound);
            } 
            else 
            {
                audioSource.PlayOneShot(recordingFailedSound);
            }
            break;
        case KeyCode.P:
            audioSource.PlayOneShot(recordPlayingSound);
            audioSource.clip = audioManager.GetComponent<AudioManager>().audioClip;
            print(audioSource.clip);
            audioSource.Play();
            audioManager.GetComponent<AudioManager>().isPlaying = true;
            break;
        case KeyCode.L:
            Respawn();
            break;
        case KeyCode.H:
            DaugtherCall();
            break;
        case KeyCode.C: // Debug purpose, should not be available in shipping (Rufus)
            if(mainCam.activeInHierarchy){
                mainCam.SetActive(false);
                blindCam.SetActive(true);
            }
            else {
                mainCam.SetActive(true);
                blindCam.SetActive(false);
            }
            break;
        case KeyCode.Tab:
            if(rb.rotation.eulerAngles == new Vector3(0,0,0)){
                audioSource.Stop();
                audioSource.clip = direction[0];
                audioSource.Play();
            }
            if(rb.rotation.eulerAngles == new Vector3(0,90,0)){
                audioSource.Stop();
                audioSource.clip = direction[1];
                audioSource.Play();
            }
            else if(rb.rotation.eulerAngles == new Vector3(0,180,0)){
                audioSource.Stop();
                audioSource.clip = direction[2];
                audioSource.Play();
            }
            else if(rb.rotation.eulerAngles == new Vector3(0,270,0)){
                audioSource.Stop();
                audioSource.clip = direction[3];
                audioSource.Play();
            }
            break;
            
        default: 
            break;
    }
}
    public bool Death(){
        audioSource.PlayOneShot(deathSound);
        StartCoroutine(waitForDeath());
        eventLogger.LogEvent("Player died");
        //Respawn();
        return true;
    }

    void Respawn(){
        /*timer = 0;

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

        audioSource.PlayOneShot(deathSound);*/
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
	// Metod för att sätta paus status (adin)
    public void SetPauseState(bool pause)
    {
        isPaused = pause;
        if (pause)
        {
            audioSource.Stop(); // Stoppa ljud från spelaren (adin)
            rb.velocity = Vector3.zero; // Stoppa movement (adin)
        }
    }

    private IEnumerator waitForDeath()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
