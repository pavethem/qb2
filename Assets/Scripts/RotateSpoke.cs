using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class RotateSpoke : MonoBehaviour
{
    public float rotationSpeed;
    private Transform lastParent;
    private Quaternion lastRotation;
    private Vector3 lastAxis;
    internal bool reversing;

    public AudioClip[] rotationSounds;

    public IEnumerator RotateIt(Vector3 axis) {

        if (!reversing) {

            float timeCount = 0;

            //remove parent while rotating, else it starts to skew
            lastParent = transform.parent;
            lastRotation = transform.rotation;
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
                GameController.rotating = true;
                transform.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
                timeCount += Time.deltaTime * rotationSpeed;
                yield return null;
            }

            transform.rotation = tempTo;
            transform.parent = lastParent;

            GameController.rotating = false;
        }
    }

    public IEnumerator RotateItBack() {
        
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
            GameController.rotating = true;
            transform.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
            timeCount += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        transform.parent = lastParent;
        transform.rotation = tempTo;
        GameController.rotating = false;
    }

}
