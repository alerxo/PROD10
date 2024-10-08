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

    bool xLocked = false;
    bool zLocked = false;

    private Vector3 lastKnownPos;
    private Vector3 spawnPos;
    private Rigidbody invRb;
    // Start is called before the first frame update
    void Start()
    {
        invRb = GetComponent<Rigidbody>();

        invRb.position = transform.parent.transform.position + GetComponentInParent<PlayerController>().moveDirection*6;

        spawnPos = invRb.position;

        GetComponent<AudioSource>().enabled = true;

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
            VerLock();
        }
        else if (hInput != 0)
        {
            HorLock();
        }
    }

    private void VerLock()
    {

        invRb.interpolation = RigidbodyInterpolation.Interpolate;

        if((transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,0,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,180,0)) && !xLocked)
        {
            VerFollow();

            lastKnownPos = invRb.position;

            zLocked = true;

        }
        else if((transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,90,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,270,0)) && !zLocked)
        {
            HorFollow();

            lastKnownPos = invRb.position;

            xLocked = true;
            
        }
        else if(!xLocked)
        {   
            invRb.position = lastKnownPos;

            VerFollow();

            zLocked = true;
        }
        else if(!zLocked)
        {   
            invRb.position = lastKnownPos;

            HorFollow();

            xLocked = true;
        }
    }

    private void HorLock()
    {
        invRb.interpolation = RigidbodyInterpolation.Interpolate;

        if((transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,0,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,180,0)) && !zLocked)
        {
            HorFollow();

            lastKnownPos = invRb.position;

            xLocked = true;

        }
        else if((transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,90,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,270,0)) && !xLocked)
        {
            VerFollow();

            lastKnownPos = invRb.position;

            zLocked = true;
            
        }
        else if(!zLocked)
        {   
            invRb.position = lastKnownPos;

            HorFollow();

            xLocked = true;
        }
        else if(!xLocked)
        {   
            invRb.position = lastKnownPos;

            VerFollow();

            zLocked = true;
        }
    }

    private void VerFollow()
    {
        invRb.position = new Vector3(transform.parent.GetComponent<PlayerController>().rb.position.x, spawnPos.y, spawnPos.z);

        invRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

    }

    private void HorFollow()
    {
        invRb.position = new Vector3(spawnPos.x, spawnPos.y, transform.parent.GetComponent<PlayerController>().rb.position.z);

        invRb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
    }
}
