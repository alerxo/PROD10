using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    bool m_Started;
    [SerializeField] LayerMask m_LayerMask;
    private Collider[] hitColliders;

    public bool isPlaying = false;
    // Potential to include a default recording (Rufus)
    public AudioClip audioClip;
    public AudioClip otherClip;

    // Start is called before the first frame update
    void Start()
    {
        m_Started = true;
    }

    // Update is called once per frame
    void Update()
    {
        CollisionDetection();

        if(isPlaying && hitColliders.Length < 0){
            AdversarySounds();
        } 
    }

    void CollisionDetection(){
        // Potential for several detections (Rufus)
        hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, m_LayerMask);
    }

    public void RecordSound(){
        for (int i = 0; i < hitColliders.Length; i++){
            audioClip = hitColliders[i].gameObject.GetComponent<AudioSource>().clip;
            print(audioClip);
        }
    }

    void AdversarySounds(){
        for (int i = 0; i < hitColliders.Length; i++){
            if(audioClip.name == "SFX_Water" && hitColliders[i].name == "Fire0"){
                audioClip = otherClip;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started){
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }    
    }
}
