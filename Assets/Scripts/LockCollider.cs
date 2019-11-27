using System.Collections;
using UnityEngine;

public class LockCollider : MonoBehaviour {
    
    private float stayTime;
    private const float STAYTIME_THRESHOLD = 0.25f;
    private bool collided;
    private bool shaking;
    private float shaketime;
    private const float SHAKETIME_MAX = 0.8f;
    private Vector3 originalPosition;

    private void Start() {
        originalPosition = transform.position;

    }

    void OnTriggerStay(UnityEngine.Collider other) {

        //key will let you go through locks
        if (other.gameObject.CompareTag("key") && GameController.locks.Contains(gameObject)) {
            
            if (stayTime > STAYTIME_THRESHOLD) {
                GetComponent<AudioSource>().Play();
                foreach (Transform child in other.gameObject.transform.parent.transform) {
                    if (child.name.Equals("Key") && GameController.keys.Contains(child.gameObject)) {
                        GameController.RemoveLock(gameObject, child.gameObject);
                        child.GetComponent<KeyCollider>().collisionkey.SetActive(false);
                        child.GetComponent<FadeInOut>().StartCoroutine("FadeOut", false);
                    }
                    if(child.tag.Equals("key"))
                        child.parent = null;
                }

                if (other.gameObject.CompareTag("key"))
                    other.gameObject.transform.parent = null;
            }
            else
                stayTime += Time.deltaTime;

        }

    }

    private void OnTriggerEnter(UnityEngine.Collider other) {
        
        if (((other.gameObject.CompareTag("bub") && other.gameObject.transform.childCount == 1)
             || other.gameObject.CompareTag("spoke")) && GameController.locks.Contains(gameObject) && stayTime == 0) {
            //can't go through locks
            if (GameController.lastRotateSpoke != null) {
                GameController.lastRotateSpoke.StopCoroutine(nameof(RotateSpoke.RotateIt));
                GameController.lastRotateSpoke.StartCoroutine("RotateItBack");
            }
            
            if (GameController.lastRotatorStrip != null && GameController.locks.Contains(gameObject)) {
                
                GameController.lastRotatorStrip.StopRotating();
                GameController.lastRotatorStrip.signedAngle *= -1;
                GameController.lastRotatorStrip.StartCoroutine("Rotate", true);
            }
            
            collided = true;

        }
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if(GameController.locks.Contains(gameObject))
            stayTime = 0;
    }
    
    private void OnMouseUpAsButton() {
        if(!shaking && !GameController.rotating && GameController.rotatingColliders.Count == 0)
            StartCoroutine(Shake(true));
    }

    private IEnumerator Shake(bool fromclick) {
        shaking = true;

        while (shaketime < SHAKETIME_MAX) {
            shaketime += Time.deltaTime;
            float amount = 8 * (1 + shaketime);
            transform.Translate(0, 0, Mathf.Sin(Mathf.Rad2Deg * shaketime) / amount);
            yield return null;
        }
        
        shaketime = 0;
        transform.position = originalPosition;
        collided = false;
        shaking = false;
    }

    private void Update() {
        
        if (collided && !shaking) {
            StartCoroutine(Shake(false));        
        }
    }
}
