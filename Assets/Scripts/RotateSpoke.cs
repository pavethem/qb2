using System.Collections;
using UnityEngine;

public class RotateSpoke : MonoBehaviour
{
    public float rotationSpeed;
    private Transform lastParent;
    private Quaternion lastRotation;
    private Vector3 lastAxis;
    private GameObject lastSpoke;
    internal bool reversing;

    public AudioClip[] rotationSounds;

    public IEnumerator RotateIt(GameObject spoke) {

        if (!reversing) {
            GameController.rotatingSpoke = true;

            float timeCount = 0;

            Vector3 axis = spoke.transform.up;
            
            //remove parent while rotating, else it starts to skew
            lastParent = transform.parent;
            lastRotation = transform.rotation;
            lastSpoke = spoke;
            lastAxis = axis;
            transform.parent = null;

            Quaternion tempFrom = transform.rotation;
            Quaternion tempTo = Quaternion.AngleAxis(-90f, axis) * tempFrom;

            //rotation sound is equal to that of the rotator strips
            if (axis == GameObject.FindGameObjectWithTag("rotatorStripX").transform.up) {
                gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[0]);
            } else if (axis == GameObject.FindGameObjectWithTag("rotatorStripY").transform.up) {
                gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[2]);
            } else if (axis == GameObject.FindGameObjectWithTag("rotatorStripZ").transform.up) {
                gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[4]);
            } else if (-axis == GameObject.FindGameObjectWithTag("rotatorStripX").transform.up) {
                gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[1]);
            } else if (-axis == GameObject.FindGameObjectWithTag("rotatorStripY").transform.up) {
                gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[3]);
            } else if (-axis == GameObject.FindGameObjectWithTag("rotatorStripZ").transform.up) {
                gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[5]);
            }
            
            while (transform.rotation != tempTo) {
                GameController.rotatingSpoke = true;
                transform.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
                timeCount += Time.fixedDeltaTime * rotationSpeed;
                yield return new WaitForFixedUpdate();
            }

            transform.rotation = tempTo;
            transform.parent = lastParent;

            GameController.rotatingColliders.Remove(spoke);
            if(GameController.rotatingColliders.Count == 0)
                GameController.lastRotateSpoke = null;
            GameController.rotatingSpoke = false;
        }
    }

    public IEnumerator RotateItBack() {
        
        GameController.rotatingSpoke = true;
        reversing = true;
        float timeCount = 0;
        
        //remove parent while rotating, else it starts to skew, though it shouldn't have a parent at this point anyways
        transform.parent = null;
        
        Quaternion tempFrom = transform.rotation;
        Quaternion tempTo = lastRotation;
        
        //rotation sound is equal to that of the rotator strips
        if (lastAxis == GameObject.FindGameObjectWithTag("rotatorStripX").transform.up) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[1]);
        } else if (lastAxis == GameObject.FindGameObjectWithTag("rotatorStripY").transform.up) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[3]);
        } else if (lastAxis == GameObject.FindGameObjectWithTag("rotatorStripZ").transform.up) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[5]);
        } else if (-lastAxis == GameObject.FindGameObjectWithTag("rotatorStripX").transform.up) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[0]);
        } else if (-lastAxis == GameObject.FindGameObjectWithTag("rotatorStripY").transform.up) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[2]);
        } else if (-lastAxis == GameObject.FindGameObjectWithTag("rotatorStripZ").transform.up) {
            gameObject.GetComponent<AudioSource>().PlayOneShot(rotationSounds[4]);
        }
        
        while (transform.rotation != tempTo) {
            GameController.rotatingSpoke = true;
            transform.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
            timeCount += Time.fixedDeltaTime * rotationSpeed;
            yield return new WaitForFixedUpdate();
        }

        transform.parent = lastParent;
        transform.rotation = tempTo;
        reversing = false;
        GameController.rotatingColliders.Remove(lastSpoke);
        if(GameController.rotatingColliders.Count == 0)
            GameController.lastRotateSpoke = null;
        GameController.rotatingSpoke = false;
    }

}
