using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    bool m_Started;
    [SerializeField] LayerMask m_LayerMask;
    private Collider[] hitColliders;

    // Potential to include a default recording (Rufus)
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        m_Started = true;
    }

    // Update is called once per frame
    void Update()
    {
        CollisionDetection();
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_Started){
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }    
    }
}
