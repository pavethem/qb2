using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongSpoke : MonoBehaviour {
    public float moveSpeed;   
    
    //moves bub along its spoke
    public IEnumerator MoveIt() {

        Vector3 pos = transform.position;
        Vector3 goal = transform.parent.parent.position;

        float timeCount = 0;
        
        while (transform.position.normalized != goal.normalized) {

            GameController.moving = true;
            transform.position = Vector3.Lerp(pos, goal, timeCount);
            timeCount += Time.deltaTime * moveSpeed;
            yield return null;

        }

        transform.position = goal;
        transform.position.Normalize();
        transform.rotation.Normalize();
        GameController.moving = false;

        transform.parent = transform.parent.parent.parent;
    }
}
