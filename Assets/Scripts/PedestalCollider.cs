using UnityEngine;

//Plays Dice Sound when cubes are colliding with it
public class PedestalCollider : MonoBehaviour
{
    private void OnTriggerEnter(UnityEngine.Collider other) {
        if (other.gameObject.CompareTag ("nodecolliders")) {
            other.gameObject.GetComponent<AudioSource>().Play();
            if(other.transform.parent.GetComponent<LockCollider>() != null)
                other.transform.parent.gameObject.GetComponent<LockCollider>().enabled = false;
        }
    }

    //parent things lying on the pedestal so they don't slip around while the pedestal is rotating
    private void OnTriggerStay(UnityEngine.Collider other) {
        GameController.fallingLock = false;
        other.transform.parent.parent = transform;
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if (other.gameObject.CompareTag ("nodecolliders")) {
            other.gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
