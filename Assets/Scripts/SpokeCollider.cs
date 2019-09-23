using UnityEngine;

public class SpokeCollider : MonoBehaviour
{
    void OnTriggerEnter(UnityEngine.Collider other) {

        //don't rotate into another spoke
        if (GameController.lastRotateSpoke != null && other.gameObject.CompareTag("spoke")) {
            
            GameController.lastRotateSpoke.StopCoroutine(nameof(RotateSpoke.RotateIt));
            GameController.lastRotateSpoke.StartCoroutine("RotateItBack");

        } 

    }

}
