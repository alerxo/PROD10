using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float MoveDelay;
    [SerializeField] LayerMask m_LayerMask;
    [SerializeField] AudioClip playerStep;
    [SerializeField] AudioClip recordSound;
    [SerializeField] AudioClip deathSound;
    GameObject mainCam;   
    GameObject blindCam;     
    GameObject audioManager;
    public AudioSource audioSource;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float timer;
    Rigidbody rb;
    private Collider[] ventCollider;
    private bool isMoving = false;

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

            Vector3 localMoveDirection = new Vector3(horizontalInput, 0f, verticalInput);
            
            moveDirection = transform.TransformDirection(localMoveDirection);
            
            //transform.Translate(moveDirection * speed); 
            rb.velocity = moveDirection * speed; //Can also move diagonally but scuffed (Rufus)

            if(horizontalInput != 0.0f || verticalInput != 0.0f){
                timer = MoveDelay + Time.deltaTime;
                isMoving = true;
                if(isMoving){
                    audioSource.clip = playerStep; 
                    audioSource.Play();
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
                rb.rotation *= Quaternion.Euler(0,-90,0);
                break;
            case "e": 
                rb.rotation *= Quaternion.Euler(0,90,0);
                break;
            case "r":
                audioSource.Stop();
                audioManager.GetComponent<AudioManager>().isPlaying = false;
                if (audioManager.GetComponent<AudioManager>().RecordSound())
                {
                    audioSource.PlayOneShot(recordSound);
                }
                break;
            case "p":
                audioSource.clip = audioManager.GetComponent<AudioManager>().audioClip;
                print(audioSource.clip);
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
}
