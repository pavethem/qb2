using System.Collections.Generic;
using UnityEngine;

public class ArrowCollider : MonoBehaviour {

    //goal to move to
    private Vector3? goal;
    //traverse scene graph for children only once
    private bool traversed;

    private void OnTriggerStay(UnityEngine.Collider other) {

        if (GameController.rotating) {
            goal = null;
            traversed = false;
        }

        if (other.gameObject.CompareTag("joint") && !GameController.rotating && !GameController.moving && goal == null) {

            Transform parent = null;
            Vector3 spoke = new Vector3();
            if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.name != "Empty") {
                parent = TraverseParents(other.gameObject.transform);

                //search for parents in the direction of the arrow first
                spoke = parent.position - other.gameObject.transform.position;
                if (GameController.Compare(spoke.normalized, gameObject.transform.forward) && !GameController.moving) {
                    goal = parent.position;
                }
            }

            //now search for children if no parents where found
            if (other.gameObject.transform.childCount > 0 && !GameController.rotating && goal == null && !traversed) {

                traversed = true;
                List<Transform> children = TraverseChildren(other.gameObject.transform);
                foreach (Transform child in children) {
                    spoke = child.position - other.gameObject.transform.position;

                    if (GameController.Compare(spoke.normalized, gameObject.transform.forward) && !GameController.moving) {
                        goal = child.position;
                        break;
                    }
                }
            }
        }
        //if a joint was found as a parent or child, go there
        if (other.gameObject.CompareTag("bub") && !GameController.rotating && !GameController.moving  && goal != null) {
            other.gameObject.GetComponent<MoveAlongSpoke>().StartCoroutine(nameof(MoveAlongSpoke.MoveIt), goal);
            gameObject.GetComponent<AudioSource>().Play();
            goal = null; 
        }
		
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if (other.gameObject.CompareTag("joint")) {
            goal = null;
            traversed = false;
        }
    }

    //find next joint in parents
    private Transform TraverseParents(Transform trans) {
        Transform parent = null;
        if (!trans.parent.CompareTag("joint")) {
            parent = TraverseParents(trans.parent);
        }
        else {
            parent = trans.parent;
        }

        return parent;
    }
    
    //find list of joints in children, but only go as deep as the first joint found (breadth-first search)
    private List<Transform> TraverseChildren(Transform trans) {
        List<Transform> children = new List<Transform>();

        foreach (Transform kid in trans) {
            if (kid.CompareTag("joint")) {
                children.Add(kid);
            }
        }

        if (children.Count == 0) {
            foreach (Transform kid in trans) {
                if (kid.childCount > 0)
                    children.AddRange(TraverseChildren(kid));
            }
        }

        return children;
    }


}
