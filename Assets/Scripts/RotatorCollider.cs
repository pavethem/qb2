using UnityEngine;

public class RotatorCollider : MonoBehaviour {
	
	//rotate only once
	private bool rotate;
	
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
}
