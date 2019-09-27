using System;
using UnityEngine;

public class ArrowCollider : MonoBehaviour {

    //less "accurate" than unity's implementation
    private const float EPSILON = 9.99999944E-5f;
    
    private void OnTriggerStay(UnityEngine.Collider other) {
        
        //move the bub along the spoke
        if (other.gameObject.CompareTag("bub") && !GameController.rotating) {
            Vector3 spoke = other.gameObject.transform.parent.position - other.gameObject.transform.position;
            if (Compare(spoke.normalized,gameObject.transform.forward) && !GameController.moving) {

                other.gameObject.GetComponent<MoveAlongSpoke>().StartCoroutine(nameof(MoveAlongSpoke.MoveIt));
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
		
    }
    
    private static bool Compare(Vector3 lhs, Vector3 rhs)
    {
        return Vector3.SqrMagnitude(lhs - rhs) < EPSILON;
    }
}
