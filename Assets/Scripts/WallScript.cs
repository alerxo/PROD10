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
    public GameObject assignedObject;
    public LayerMask m_BlockLayer;

    bool xLocked = false;
    bool zLocked = false;
    bool isPlaying = false;

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

        if(assignedObject.GetComponent<Collider>().enabled == false && !isPlaying){

            isPlaying = true;
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = assignedObject.GetComponent<PuzzleElement>().responseClip;
            GetComponent<AudioSource>().loop = false;
            GetComponent<AudioSource>().Play();
        }


    }

        private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & m_BlockLayer) != 0)
        {
            Debug.Log("Collided with RayBlock");
            invRb.constraints = RigidbodyConstraints.FreezeAll;
            xLocked = true;
            zLocked = true;
            lastKnownPos = invRb.position;
        }
    }

    private void VerLock()
    {   
        //print("V: " + vInput + " H: " + hInput);
        invRb.interpolation = RigidbodyInterpolation.Interpolate;

        if(!xLocked && (transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,0,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,180,0)))
        {
            VerFollow();

            lastKnownPos = invRb.position;

            zLocked = true;

        }
        else if(!zLocked && (transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,90,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,270,0)))
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
        else{
            invRb.position = lastKnownPos;

            if(transform.parent.transform.position.x == lastKnownPos.x){
                xLocked = false;
                invRb.position = lastKnownPos;
            }
            else if(transform.parent.transform.position.z == lastKnownPos.z){
                zLocked = false;
                invRb.position = lastKnownPos;
            }
        }
    }

    private void HorLock()
    {
        //print("V: " + vInput + " H: " + hInput);
        invRb.interpolation = RigidbodyInterpolation.Interpolate;

        if(!zLocked && (transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,0,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,180,0)))
        {
            HorFollow();

            lastKnownPos = invRb.position;

            xLocked = true;

        }
        else if(!xLocked && (transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,90,0) ||
        transform.parent.GetComponent<PlayerController>().rb.rotation.eulerAngles == new Vector3(0,270,0)))
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
        else{
            invRb.position = lastKnownPos;

            if(transform.parent.transform.position.x == lastKnownPos.x){
                xLocked = false;
                invRb.position = lastKnownPos;
            }
            else if(transform.parent.transform.position.z == lastKnownPos.z){
                zLocked = false;
                invRb.position = lastKnownPos;
            }
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
