using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollider : MonoBehaviour
{
    private void OnTriggerStay(UnityEngine.Collider other) {
		
        //pick up the key when done rotating
        if (other.gameObject.CompareTag("bub") && !GameController.rotating) {
            gameObject.transform.parent = other.gameObject.transform;
        }
		
    }
}
