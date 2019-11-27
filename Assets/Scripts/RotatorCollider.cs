using System.Collections;
using UnityEngine;

public class RotatorCollider : MonoBehaviour {
	
	//rotate only once
	private bool rotate;
	private bool rotating;

	private Quaternion originalRotation;

	private void Awake() {
		originalRotation = transform.rotation;
	}
	
	private void OnTriggerStay(UnityEngine.Collider other) {

		//rotate bub's parent around transform.up
        if (other.gameObject.CompareTag("bub") && !GameController.rotating && rotate && !GameController.rotatingSpoke) {
			
	        rotate = false;

	        if (other.gameObject.transform.parent.GetComponent<RotateSpoke>() != null) {
		        GameController.lastRotateSpoke = other.gameObject.transform.parent.GetComponent<RotateSpoke>();
		        GameController.lastRotateSpoke.reversing = false;
		        other.gameObject.transform.parent.GetComponent<RotateSpoke>()
				        .StartCoroutine(nameof(RotateSpoke.RotateIt), gameObject);
		        if(!rotating)
					StartCoroutine(Rotate());
	        }
        }
    }

    private void OnTriggerEnter(UnityEngine.Collider other) {
	    if (other.gameObject.CompareTag("bub")) {
		    if (other.gameObject.transform.parent.GetComponent<RotateSpoke>() != null) {
			    rotate = true;
			    GameController.rotatingColliders.Add(gameObject);
		    }
	    }
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
	    if (other.gameObject.CompareTag("bub")) {
		    if (other.gameObject.transform.parent.GetComponent<RotateSpoke>() != null) {
			    rotate = false;
			    GameController.rotatingColliders.Remove(gameObject);
		    }
	    }
    }
    
    private void OnMouseUpAsButton() {
	    if (!GameController.rotating && !rotating && !GameController.rotatingSpoke)
		    StartCoroutine(Rotate());
    }

    //just the little animation onclick
    private IEnumerator Rotate() {
	    rotating = true;
	    for (int i = 0; i < 2; i++) {
		    float step = 0;
		    Quaternion fromRotation = transform.rotation;
		    Quaternion toRotation = Quaternion.AngleAxis(180f, transform.up) * transform.rotation;
		    while (transform.rotation != toRotation) {
			    transform.rotation = Quaternion.Slerp(fromRotation, toRotation, step);
			    step += Time.deltaTime * 2f;
			    if (step > 1)
				    break;
			    yield return null;
		    }
	    }

	    transform.rotation = originalRotation;
	    rotating = false;

    }
    
}
