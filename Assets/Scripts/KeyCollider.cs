using System.Collections;
using UnityEngine;

public class KeyCollider : MonoBehaviour {
    
    private Vector3 originalScale;
    private Quaternion originalRotation;
    public GameObject collisionkey;
    
    public float shrinkSpeed;
    public float shrinkScale;
    private bool shrunk;
    private bool rotating;

    private void Awake() {
        
        originalScale = gameObject.transform.localScale;
        originalRotation = gameObject.transform.rotation;

    }

    private void OnTriggerStay(UnityEngine.Collider other) {

        //pick up the key when done rotating
        if (other.gameObject.CompareTag("bub") && !GameController.rotating && 
            GameController.rotatingColliders.Count == 0 && other.gameObject.transform.childCount == 1) {
            gameObject.transform.parent = other.gameObject.transform;
            //create an invisible key, only to keep the original collision box
            collisionkey = Instantiate(collisionkey, other.gameObject.transform.position, gameObject.transform.rotation, other.gameObject.transform);
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
    }

    private void OnMouseUpAsButton() {
        if (!GameController.rotating && GameController.rotatingColliders.Count == 0 && !shrunk)
            StartCoroutine(Rotate());
    }

    private IEnumerator Rotate() {
        rotating = true;
        for (int i = 0; i < 4; i++) {
            float step = 0;
            Quaternion fromRotation = transform.rotation;
            Quaternion toRotation = Quaternion.AngleAxis(180f, transform.right) * transform.rotation;
            while (transform.rotation != toRotation) {
                transform.rotation = Quaternion.Slerp(fromRotation, toRotation, step);
                step += Time.deltaTime * 2f * ( 1 + Mathf.Min(i,2));
                if (step > 1)
                    break;
                yield return null;
            }
        }

        transform.rotation = originalRotation;
        rotating = false;

    }

    IEnumerator Shrink() {
        
        StopCoroutine(Rotate());

        float timeCount = 0;
            while (gameObject.transform.localScale.x > originalScale.x-shrinkScale) {

                float scale = originalScale.x - Mathf.Lerp(0, shrinkScale, timeCount);
                gameObject.transform.localScale = new Vector3(scale,scale,scale);
                timeCount += Time.deltaTime * shrinkSpeed;
                yield return null;
            }
            
            gameObject.transform.localScale = new Vector3(originalScale.x - shrinkScale,originalScale.y - shrinkScale,
                originalScale.z - shrinkScale);
            gameObject.GetComponent<BoxCollider>().size *= originalScale.x + shrinkScale;
            shrunk = true;
    }
		
}
