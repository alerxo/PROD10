using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    GameObject mainCam;   
    GameObject blindCam;     
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        blindCam = GameObject.FindGameObjectWithTag("BlindCamera");

        blindCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        transform.Translate(moveDirection * speed * Time.deltaTime);

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
}
