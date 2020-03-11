using System.Collections;
using UnityEngine;

public class RotateSpoke : MonoBehaviour
{
    public float rotationSpeed;
    //sometimes you don't want the actual Transform to rotate, but only part of the thing
    public GameObject rotationObject;
    //some joints should be skipped while traversing through the scene graph (see arrow collider)
    public bool skipJoint;
    //how often this joint was rotated
    internal int rotated = 0;
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
            Transform trans = rotationObject == null ? transform : rotationObject.transform;
            
            
            //remove parent while rotating, else it starts to skew
            lastParent = trans.parent;
            lastRotation = trans.rotation;
            lastSpoke = spoke;
            lastAxis = axis;
            trans.parent = null;
            Vector3 forward = trans.forward;


            Quaternion tempFrom = trans.rotation;
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
            
            while (trans.rotation != tempTo) {
                GameController.rotatingSpoke = true;
                trans.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
                timeCount += Time.fixedDeltaTime * rotationSpeed;
                yield return new WaitForFixedUpdate();
            }
            

            trans.rotation = tempTo;
            trans.parent = lastParent;

            GameController.rotatingColliders.Remove(spoke);
            if(GameController.rotatingColliders.Count == 0)
                GameController.lastRotateSpoke = null;
            //only count rotation if it was different from before
            if(!GameController.Compare(forward, trans.forward))
                rotated++;
            GameController.rotatingSpoke = false;
        }
    }

    public IEnumerator RotateItBack() {
        
        GameController.rotatingSpoke = true;
        reversing = true;
        float timeCount = 0;
        
        Transform trans = rotationObject == null ? transform : rotationObject.transform;

        //remove parent while rotating, else it starts to skew, though it shouldn't have a parent at this point anyways
        trans.parent = null;
        
        Quaternion tempFrom = trans.rotation;
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
        
        while (trans.rotation != tempTo) {
            GameController.rotatingSpoke = true;
            trans.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
            timeCount += Time.fixedDeltaTime * rotationSpeed;
            yield return new WaitForFixedUpdate();
        }

        trans.parent = lastParent;
        trans.rotation = tempTo;
        reversing = false;
        GameController.rotatingColliders.Remove(lastSpoke);
        if(GameController.rotatingColliders.Count == 0)
            GameController.lastRotateSpoke = null;
        GameController.rotatingSpoke = false;
    }

}
