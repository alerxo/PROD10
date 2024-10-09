using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundCopier : MonoBehaviour
{
    private AudioSource audioSource;
    private Rigidbody rb;
    private float xVel; 
    private float zVel;
    private float speed;
    private GameObject player;
    [SerializeField] LayerMask m_LayerToInclude0;
    [SerializeField] LayerMask m_LayerToInclude1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = transform.parent.gameObject;
        speed = transform.parent.gameObject.GetComponent<PlayerController>().speed;
        //transform.parent = null;
    }

    void Update()
    {
        xVel = player.GetComponent<PlayerController>().moveDirection.x;
        zVel = player.GetComponent<PlayerController>().moveDirection.z;

        rb.velocity = new Vector3(xVel, 0, zVel) * speed;
 
    }
private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.GetComponent<AudioSource>().clip != null && ((1 << collision.gameObject.layer) & (m_LayerToInclude0 | m_LayerToInclude1)) != 0)
    {
        audioSource = collision.gameObject.GetComponent<AudioSource>();
        print("It do be colliding");
        GetComponent<AudioSource>().clip = audioSource.clip;
        //GetComponent<AudioSource>().time = audioSource.time;
        GetComponent<AudioSource>().Play();
        
    }
    else{
        print("No clip found!");
    }
}
    void OnCollisionExit(Collision collision){
        print("It do be stopping");
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = null;
    }
    
}
