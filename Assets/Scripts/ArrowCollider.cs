using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollider : MonoBehaviour {

    //goal to move to
    private GameObject goal;
    //traverse scene graph for children only once
    private bool traversed;
    private bool travesedParents;
    private bool rotating;

    private Quaternion originalRotation;

    private void Awake() {
        originalRotation = transform.rotation;
    }

    private void OnTriggerStay(UnityEngine.Collider other) {

        if (GameController.rotating) {
            goal = null;
            traversed = false;
            travesedParents = false;
        }
        
        if (other.gameObject.CompareTag("joint") && !GameController.rotating && !GameController.moving && 
            GameController.rotatingColliders.Count == 0 && goal == null) {

            Transform parent = null;
            Vector3 spoke = new Vector3();
            if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.name != "Empty" && !travesedParents) {
                parent = TraverseParents(other.gameObject.transform);
                travesedParents = true;
                //search for parents in the direction of the arrow first
                spoke = parent.position - other.gameObject.transform.position;
                if (GameController.Compare(spoke.normalized, gameObject.transform.forward) && !GameController.moving) {
                    goal = parent.gameObject;
                }
            }

            //now search for children if no parents where found
            if (other.gameObject.transform.childCount > 0 && !GameController.rotating && 
                GameController.rotatingColliders.Count == 0 && goal == null && !traversed) {

                traversed = true;
                List<Transform> children = TraverseChildren(other.gameObject.transform);
                foreach (Transform child in children) {
                    spoke = child.position - other.gameObject.transform.position;

                    if (GameController.Compare(spoke.normalized, gameObject.transform.forward) && !GameController.moving) {
                        goal = child.gameObject;
                        break;
                    }
                }
            }
        }
        //if a joint was found as a parent or child, go there
        if (other.gameObject.CompareTag("bub") && !GameController.rotating && !GameController.moving &&
            GameController.rotatingColliders.Count == 0 && goal != null) {
            other.gameObject.GetComponent<MoveAlongSpoke>().StartCoroutine(nameof(MoveAlongSpoke.MoveIt), goal);
            gameObject.GetComponent<AudioSource>().Play();
            goal = null; 
        }
		
    }

    private void OnTriggerExit(UnityEngine.Collider other) {
        if (other.gameObject.CompareTag("joint")) {
            goal = null;
            traversed = false;
            travesedParents = false;
        }
    }
    
    private void OnMouseUpAsButton() {
        if (!GameController.rotating && !rotating && GameController.rotatingColliders.Count == 0)
            StartCoroutine(Rotate());
    }

    private IEnumerator Rotate() {
        rotating = true;
        for (int i = 0; i < 2; i++) {
            float step = 0;
            Quaternion fromRotation = transform.rotation;
            Quaternion toRotation = Quaternion.AngleAxis(180f, transform.forward) * transform.rotation;
            while (transform.rotation != toRotation) {
                transform.rotation = Quaternion.Slerp(fromRotation, toRotation, step);
                step += Time.deltaTime * 2f;
                if (step > 1)
                    break;
                yield return null;
            }
        }

        transform.rotation = originalRotation;
        rotating = false;

    }

    //find next joint in parents
    private Transform TraverseParents(Transform trans) {
        Transform parent = null;
        if (!trans.parent.CompareTag("joint")) {
            parent = TraverseParents(trans.parent);
        }
        else {
            //skip joints that have been rotated an odd number of times
            //and thus form a straight line with their parent and child spoke
            if (trans.parent.GetComponent<RotateSpoke>() != null) {
                if (trans.parent.GetComponent<RotateSpoke>().skipJoint) {
                    if(trans.parent.GetComponent<RotateSpoke>().rotated % 2 == 0)
                        parent = trans.parent;
                    else
                        parent = TraverseParents(trans.parent);
                } else
                    parent = trans.parent;
            } else
                parent = trans.parent;
        }

        return parent;
    }
    
    //find list of joints in children, but only go as deep as the first joint found (breadth-first search)
    private List<Transform> TraverseChildren(Transform trans) {
        List<Transform> children = new List<Transform>();

        foreach (Transform kid in trans) {
            if (kid.CompareTag("joint")) {
                //skip joints that have been rotated an odd number of times
                //and thus form a straight line with their parent and child spoke
                if (kid.GetComponent<RotateSpoke>() != null) {
                    if (kid.GetComponent<RotateSpoke>().skipJoint) {
                        if(kid.GetComponent<RotateSpoke>().rotated % 2 == 0)
                            children.Add(kid);
                    } else
                        children.Add(kid);
                } else
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
