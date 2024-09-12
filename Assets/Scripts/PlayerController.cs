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
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        blindCam = GameObject.FindGameObjectWithTag("BlindCamera");

        blindCam.SetActive(false);

        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(moveDirection * speed * Time.deltaTime);

        timer -= Time.deltaTime;

        if(timer <= 0){
            print(horizontalInput + " " + verticalInput);

            float rawHorizontalInput = Input.GetAxis("Horizontal");
            float rawVerticalInput = Input.GetAxis("Vertical");

            horizontalInput = (float)Math.Ceiling(Math.Round(Input.GetAxis("Horizontal"), 1));
            verticalInput = (float)Math.Ceiling(Math.Round(Input.GetAxis("Vertical"), 1));

            if(rawHorizontalInput < 0.0f){
                horizontalInput = -1;
            }
            if(rawVerticalInput < 0.0f){
               verticalInput = -1;
            }

            moveDirection = new Vector3(horizontalInput, 0, verticalInput);
            
            transform.Translate(moveDirection * speed);

            if(horizontalInput != 0.0f || verticalInput != 0.0f){
                timer = MoveDelay + Time.deltaTime;
            }
            
        }

        

        var input = Input.inputString;
        switch(input){
            case "q": 
                transform.rotation *= Quaternion.Euler(0,-90,0);
                break;
            case "e": 
                transform.rotation *= Quaternion.Euler(0,90,0);
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
