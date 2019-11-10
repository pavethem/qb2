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

		//rotate bub's grandparent around transform.up
        if (other.gameObject.CompareTag("bub") && !GameController.rotating && rotate) {
			
	        rotate = false;

	        if (other.gameObject.transform.parent.parent != null) {
		        other.gameObject.transform.parent.parent.GetComponent<RotateSpoke>()
				        .StartCoroutine(nameof(RotateSpoke.RotateIt), transform.up);
	        }
        }
    }

    private void OnTriggerEnter(UnityEngine.Collider other) {
	    if(other.gameObject.CompareTag("bub"))
			rotate = true;
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
	    if (other.gameObject.CompareTag("bub")) {
		    rotate = false;

		    GameController.lastRotateSpoke = other.gameObject.transform.parent.parent.GetComponent<RotateSpoke>();
		    GameController.lastRotateSpoke.reversing = false;
	    }
    }
    
    private void OnMouseUpAsButton() {
	    if (!GameController.rotating && !rotating)
		    StartCoroutine(Rotate());
    }

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
