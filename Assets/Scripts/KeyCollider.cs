using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollider : MonoBehaviour {
    
    private Vector3 originalScale;
    public GameObject collisionkey;
    
    public float shrinkSpeed;
    public float shrinkScale;


    private void Awake() {
        
        originalScale = gameObject.transform.localScale;

    }

    private void OnTriggerStay(UnityEngine.Collider other) {
		
        //pick up the key when done rotating
        if (other.gameObject.CompareTag("bub") && !GameController.rotating && other.gameObject.transform.childCount == 0) {
            gameObject.transform.parent = other.gameObject.transform;
            //create an invisible key, only to keep the original collision box
            collisionkey = Instantiate(collisionkey,gameObject.transform.position,gameObject.transform.rotation,other.gameObject.transform);
            collisionkey.SetActive(true);
            GameController.keys.Add(collisionkey);
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<HingeJoint>().autoConfigureConnectedAnchor = false;
            gameObject.GetComponent<HingeJoint>().connectedBody = other.gameObject.GetComponent<Rigidbody>();
            gameObject.GetComponent<HingeJoint>().connectedAnchor = Vector3.zero;
            StartCoroutine(Shrink());
        }

        IEnumerator Shrink() {

            
            float timeCount = 0;
            while (gameObject.transform.localScale.x > originalScale.x-shrinkScale) {

                float scale = originalScale.x - Mathf.Lerp(0, shrinkScale, timeCount);
                gameObject.transform.localScale = new Vector3(scale,scale,scale);
                timeCount += Time.deltaTime * shrinkSpeed;
                yield return null;
            }
            
            gameObject.transform.localScale = new Vector3(originalScale.x - shrinkScale,originalScale.y - shrinkScale,originalScale.z - shrinkScale);
            gameObject.GetComponent<BoxCollider>().size *= originalScale.x + shrinkScale;
        }
		
    }
}
