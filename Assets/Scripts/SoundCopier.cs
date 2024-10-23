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
    public bool colliding = false;
    [SerializeField] float lowVolume;
    [SerializeField] float mediumVolume;
    [SerializeField] float highVolume;
    [SerializeField] GameObject oneAway;
    [SerializeField] GameObject twoAway;
    [SerializeField] LayerMask m_LayerToInclude0;
    [SerializeField] LayerMask m_LayerToInclude1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = transform.parent.gameObject.GetComponent<PlayerController>().speed;
    }

    void Update()
    {
        xVel = transform.parent.gameObject.GetComponent<PlayerController>().moveDirection.x;
        zVel = transform.parent.gameObject.GetComponent<PlayerController>().moveDirection.z;

        rb.velocity = new Vector3(xVel, 0, zVel) * speed;

        if(!colliding && 
        (oneAway != null && !oneAway.GetComponent<SoundCopier>().colliding) &&
        (twoAway != null && !twoAway.GetComponent<SoundCopier>().colliding)){
            twoAway.GetComponent<AudioSource>().Stop();
            twoAway.GetComponent<AudioSource>().clip = null;
        }       
        else if(colliding && 
        (oneAway != null && oneAway.GetComponent<SoundCopier>().colliding) &&
        (twoAway != null && twoAway.GetComponent<SoundCopier>().colliding)){
            twoAway.GetComponent<AudioSource>().Stop();
            twoAway.GetComponent<AudioSource>().clip = null;
        } 
    }

    void OnCollisionStay(Collision collision){
        if(twoAway != null && oneAway != null){
            if(!twoAway.GetComponent<AudioSource>().isPlaying){
                twoAway.GetComponent<AudioSource>().clip = audioSource.clip;
                twoAway.GetComponent<AudioSource>().Play();
            }

            if(collision.gameObject.GetComponent<PuzzleElement>() && collision.gameObject.GetComponent<PuzzleElement>().solved && !collision.gameObject.GetComponent<PuzzleElement>().solutionPlayed){
                twoAway.GetComponent<AudioSource>().Stop();
                InterpolateVolume(twoAway.GetComponent<AudioSource>(), highVolume);
                twoAway.GetComponent<AudioSource>().PlayOneShot(collision.gameObject.GetComponent<PuzzleElement>().responseClip);
                collision.gameObject.GetComponent<PuzzleElement>().solutionPlayed = true ;
                collision.gameObject.GetComponent<Collider>().enabled = false;
                print("its here");
            }
        }


    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<AudioSource>().clip != null && ((1 << collision.gameObject.layer) & (m_LayerToInclude0 | m_LayerToInclude1)) != 0)
        {
            colliding = true;
            audioSource = collision.gameObject.GetComponent<AudioSource>();
            
            if(twoAway == null && oneAway == null){
                if(!GetComponent<AudioSource>().isPlaying){
                    GetComponent<AudioSource>().clip = audioSource.clip;
                    InterpolateVolume(GetComponent<AudioSource>(), lowVolume);
                    GetComponent<AudioSource>().Play();
                }
                else{
                    InterpolateVolume(GetComponent<AudioSource>(), lowVolume);
                }
            }
            else if(twoAway == null && oneAway != null){
                if(!oneAway.GetComponent<AudioSource>().isPlaying){
                    oneAway.GetComponent<AudioSource>().clip = audioSource.clip;
                    InterpolateVolume(oneAway.GetComponent<AudioSource>(), mediumVolume);
                    oneAway.GetComponent<AudioSource>().Play();
                }
                else{
                    InterpolateVolume(oneAway.GetComponent<AudioSource>(), mediumVolume);
                }
                
            }
            else if(twoAway != null && oneAway != null){
                if(!twoAway.GetComponent<AudioSource>().isPlaying){
                    twoAway.GetComponent<AudioSource>().clip = audioSource.clip;
                    InterpolateVolume(twoAway.GetComponent<AudioSource>(), highVolume);
                    twoAway.GetComponent<AudioSource>().Play();
                }
                else{
                    InterpolateVolume(twoAway.GetComponent<AudioSource>(), highVolume);
                }
            }  

        }


    }
    void OnCollisionExit(Collision collision){
        if (((1 << collision.gameObject.layer) & (m_LayerToInclude0 | m_LayerToInclude1)) != 0){
            colliding = false;
        }
        

        if(collision.gameObject.GetComponent<PuzzleElement>() && collision.gameObject.GetComponent<PuzzleElement>().solved){
            if(twoAway == null && oneAway == null){
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().clip = null;
                GetComponent<AudioSource>().PlayOneShot(collision.gameObject.GetComponent<PuzzleElement>().responseClip);
            }
            else if(twoAway == null && oneAway != null){
                oneAway.GetComponent<AudioSource>().Stop();
                oneAway.GetComponent<AudioSource>().clip = null;
                oneAway.GetComponent<AudioSource>().PlayOneShot(collision.gameObject.GetComponent<PuzzleElement>().responseClip);
            }
            else if(twoAway != null && oneAway != null){
                twoAway.GetComponent<AudioSource>().Stop();
                twoAway.GetComponent<AudioSource>().clip = null;
                twoAway.GetComponent<AudioSource>().PlayOneShot(collision.gameObject.GetComponent<PuzzleElement>().responseClip);            
            }

        }
    }

    private void InterpolateVolume(AudioSource audioSource, float targetVolume, bool zeroStartVolume = false)
    {
        StartCoroutine(VolumeCoroutine(audioSource, targetVolume, zeroStartVolume));
    }

    private IEnumerator VolumeCoroutine(AudioSource audioSource, float targetVolume, bool zeroStartVolume = false)
    {   
        float startVolume;
        if(zeroStartVolume == true){
            startVolume = 0;
        }
        else{
            startVolume = audioSource.volume;
        }
        
        float time = 0.5f;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / time);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
