using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveAlongSpoke : MonoBehaviour {
    public float moveSpeed;   
    
    //moves bub along its spoke
    public IEnumerator MoveIt(GameObject target) {

        Vector3 pos = transform.position;
        Vector3 goal = target.transform.position;
//        Vector3 goal = transform.parent.parent.position;

        float timeCount = 0;

        while (!GameController.Compare(goal,transform.position)) {

            GameController.moving = true;
            transform.position = Vector3.Lerp(pos, goal, timeCount);
            timeCount += Time.deltaTime * moveSpeed;
            yield return null;
            //while check doesn't work when bub is in the middle (0,0,0)
            if (GameController.Compare(transform.position, Vector3.zero) && GameController.Compare(Vector3.zero, goal)) {
                break;
            }

        }

        transform.position = goal;
        transform.position.Normalize();
        transform.rotation.Normalize();
        transform.parent = target.transform;
        GameController.moving = false;
        
    }
}
