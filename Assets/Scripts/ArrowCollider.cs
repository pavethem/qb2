using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollider : MonoBehaviour {
    
    private void OnTriggerStay(UnityEngine.Collider other) {
		
        //move the bub along the spoke
        if (other.gameObject.CompareTag("bub") && !GameController.rotating) {
            Vector3 spoke = other.gameObject.transform.parent.position - other.gameObject.transform.position;
            if (gameObject.transform.forward == spoke.normalized && !GameController.moving) {

                other.gameObject.GetComponent<MoveAlongSpoke>().StartCoroutine(nameof(MoveAlongSpoke.MoveIt));
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
		
    }
}
