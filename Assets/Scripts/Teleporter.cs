using System.Collections;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    public float teleportSpeed;

    //teleports bub to position
    public IEnumerator TeleportIt(Vector3 position) {

        GameController.teleporting = true;

        Transform lastParent = transform.parent;
        transform.parent = null;

        float scale, originalScale;
        scale = originalScale = transform.localScale.x;


        gameObject.GetComponent<AudioSource>().Play();
        
        while (scale >= 0.5f) {

            scale -= Time.deltaTime * teleportSpeed;
            transform.localScale = new Vector3(scale,scale,scale);
            yield return null;

        }

        transform.position = position;
        
        while (scale < originalScale) {

            scale += Time.deltaTime * teleportSpeed;
            transform.localScale = new Vector3(scale,scale,scale);
            yield return null;

        }
        
        transform.localScale = new Vector3(originalScale,originalScale,originalScale);
        transform.parent = lastParent;
        GameController.teleporting = false;
    }
}
