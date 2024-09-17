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
    GameObject mainCam;   
    GameObject blindCam;     
    GameObject audioManager;
    public AudioSource audioSource;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float timer;
    Rigidbody rb;

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

                    // Create the movement vector in local space
            Vector3 localMoveDirection = new Vector3(horizontalInput, 0f, verticalInput);
            
            // Transform the local move direction to world space based on the player's rotation
            moveDirection = transform.TransformDirection(localMoveDirection);
            
            //transform.Translate(moveDirection * speed);
            rb.velocity = moveDirection * speed;
            
            if(horizontalInput != 0.0f || verticalInput != 0.0f){
                timer = MoveDelay + Time.deltaTime;
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
                audioManager.GetComponent<AudioManager>().RecordSound();
                break;
            case "p":
                audioSource.clip = audioManager.GetComponent<AudioManager>().audioClip;
                print(audioSource.clip);
                audioSource.Play();
                audioManager.GetComponent<AudioManager>().isPlaying = true;
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
        return true;
    }
}
