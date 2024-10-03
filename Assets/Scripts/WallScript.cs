using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class WallScript : MonoBehaviour
{
    public float hInput;
    public float vInput;
    private Vector3 spawnPos;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = transform.parent.transform.position + GetComponentInParent<PlayerController>().moveDirection*4;
        spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Detect when player come close to a wall
        //Create an empty object in the direction of the wall
        //Make object move either on X or Y axis (Instanciate them before Play for better performance?)
        //Delete relevant object if it's barrier-bound

        if(vInput != 0)
        {
            VerFollow();
            //HorFollow();
        }

        if (hInput != 0)
        {
            HorFollow();
            //VerFollow();
        }

    }

    private void HorFollow()
    {
        if (transform.parent.transform.eulerAngles.y == 90 || transform.parent.transform.eulerAngles.y == -90 || transform.parent.transform.eulerAngles.y == 270)
        {

            transform.position = new Vector3(transform.parent.transform.position.x, spawnPos.y, spawnPos.z);
        }
        else
        {
            transform.position = new Vector3(spawnPos.x, spawnPos.y, transform.parent.transform.position.z);
            
            
        }

        print(vInput + ", " + hInput + " rot: " + transform.parent.transform.eulerAngles);
    }

    private void VerFollow()
    {
        if (transform.parent.transform.eulerAngles.y == 0 || transform.parent.transform.eulerAngles.y == 180)
        {
            transform.position = new Vector3(transform.parent.transform.position.x, spawnPos.y, spawnPos.z);
        }
        else
        {
            transform.position = new Vector3(spawnPos.x, spawnPos.y, transform.parent.transform.position.z);

        }

        print(vInput + ", " + hInput + " rot: " + transform.parent.transform.eulerAngles);
    }


}
