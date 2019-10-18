using UnityEngine;

public class RotatorParent : MonoBehaviour {
    //angle of the desired rotation
    internal float signedAngle;
    internal Coroutine rotateRoutine;
    
    internal Color rotatorColor;
    internal LineRenderer lr;

    internal GameObject curved;
    
    //fade in resetButton when game has been rotated at least once
    internal bool hasBeenRotated;
    
    internal Quaternion reverseFrom;

    //helper to calculate angle between mouseDown and MouseUp
    internal Vector3 helper;
    //basically ray hit position on mousedown
    internal Vector3 helperDown;
    //plane of this rotatorStrip
    internal Plane plane;

    //wait a while before rotating with keys again
    internal const float MINKEYDOWNTIME = 0.9f;
    //for drawing the arrow next to the rotator Strips
    internal const float MINDISTANCE = 0.5f;
    internal const float MAXDISTANCE = 12f;
    internal const float MAXDRAWANGLE = 30f;
    
    internal int lineIndex = 0;
    internal float lastAngle = 0;

    internal void Setup() {
        rotatorColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;
        rotatorColor.a = 1.0f;

        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;

        curved = GameObject.FindWithTag("curved");
        if(curved != null)
            curved.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    internal void DragMouse() {
        
        //update helper position along transform's plane and calculate angle between it and mousedown helper position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var enter);

        helper = ray.GetPoint(enter);
        
        signedAngle = Vector3.SignedAngle(helperDown, helper, transform.up);

        if (lr.positionCount > lineIndex) {
            //set linerenderer position to next to the rotator strip, but don't add it yet
            float radius = 0;
            Vector3 from = lr.GetPosition(lineIndex);
            Vector3 toPosition = helper.normalized;
            if (CompareTag("rotatorStripX")) {
                radius = transform.parent.GetComponent<MeshRenderer>().bounds.size.x / 2;
                toPosition = new Vector3(helper.normalized.x * radius, -1, helper.normalized.z * radius);
            } else if (CompareTag("rotatorStripY")) {
                radius = transform.parent.GetComponent<MeshRenderer>().bounds.size.y / 2;
                toPosition = new Vector3(helper.normalized.x * radius, helper.normalized.y * radius, -1);
            }
            else if (CompareTag("rotatorStripZ")) {
                radius = transform.parent.GetComponent<MeshRenderer>().bounds.size.z / 2;
                toPosition = new Vector3(-1, helper.normalized.y * radius, helper.normalized.z * radius);
            }

            //add new linerenderer position and set curved arrow position
            if (Vector3.Distance(from, toPosition) > MINDISTANCE && Mathf.Abs(signedAngle) < MAXDRAWANGLE && lineIndex < 100) {
                
                //remove renderer line when it is being drawn in opposite direction
                if (lastAngle != 0 && Mathf.Sign(lastAngle) != Mathf.Sign(Vector3.SignedAngle(from, toPosition, transform.up))) {
                    lr.positionCount = 1;
                    curved.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    lineIndex = 0;
                    lr.SetPosition(lineIndex,toPosition);
                }

                lastAngle = Vector3.SignedAngle(from, toPosition, transform.up);

                //don't add if linerenderer is at max distance
                if (Vector3.Distance(lr.GetPosition(0), lr.GetPosition(lineIndex)) < MAXDISTANCE) {
                    lineIndex++;
                    lr.positionCount = lineIndex + 1;
                    lr.SetPosition(lineIndex, toPosition);
                    
                    if (Vector3.Distance(lr.GetPosition(0), toPosition) >
                        curved.gameObject.GetComponent<MeshRenderer>().bounds.size.x / 2) {
                        SetCurved(from,toPosition);
                    }
                }
            }
        }
    }

    internal void SetCurved(Vector3 from, Vector3 toPosition) {
        {
            //set and correct curved position and rotation
            curved.gameObject.GetComponent<MeshRenderer>().enabled = true;
            curved.transform.position = toPosition;

            curved.transform.LookAt(Vector3.zero);

            if (CompareTag("rotatorStripX")) {
                if (Vector3.SignedAngle(from, toPosition, transform.up) > 0)
                    curved.transform.RotateAround(curved.transform.position, curved.transform.forward,
                        180f);
            }
            else if (CompareTag("rotatorStripY")) {
                curved.transform.RotateAround(curved.transform.position, curved.transform.forward, 270f);
                if (Vector3.SignedAngle(from, toPosition, transform.forward) > 0)
                    curved.transform.RotateAround(curved.transform.position, curved.transform.forward,
                        180f);
            }
            else if (CompareTag("rotatorStripZ")) {
                curved.transform.RotateAround(curved.transform.position, curved.transform.forward, 90f);
                if (Vector3.SignedAngle(from, toPosition, transform.forward) > 0)
                    curved.transform.RotateAround(curved.transform.position, curved.transform.forward,
                        180f);
            }
        }
    }

    internal void ResetValues() {
        
        GameController.rotatorClicked = true;

        signedAngle = 0f;

        plane = new Plane(transform.up, 0);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hit, Mathf.Infinity,
            1 << LayerMask.NameToLayer("RotatorStrips"));

        //set helper to hitpoint
        plane.Raycast(ray, out var enter);
        helper = ray.GetPoint(enter);
        helperDown = helper;
        
        //clear last linerenders
        curved.gameObject.GetComponent<MeshRenderer>().enabled = false;
        GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripX").transform.GetChild(0).gameObject.GetComponent<LineRenderer>().positionCount = 0;
        GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripY").transform.GetChild(0).gameObject.GetComponent<LineRenderer>().positionCount = 0;
        GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripZ").transform.GetChild(0).gameObject.GetComponent<LineRenderer>().positionCount = 0;

        //set first line renderer position
        lineIndex = 0;
        lr.positionCount = 1;
        Vector3 linePos = helper.normalized * (transform.parent.GetComponent<MeshRenderer>().bounds.size.x / 2);
        if (CompareTag("rotatorStripX"))
            linePos.y = -1;
        else if (CompareTag("rotatorStripY")) {
            linePos = helper.normalized * (transform.parent.GetComponent<MeshRenderer>().bounds.size.y / 2);
            linePos.z = -1;
        }
        else if (CompareTag("rotatorStripZ")) {
            linePos = helper.normalized * (transform.parent.GetComponent<MeshRenderer>().bounds.size.z / 2);
            linePos.x = -1;
        }
        lr.SetPosition(lineIndex, linePos);
    }

    internal void StopRotating() {
        if(rotateRoutine != null)
            StopCoroutine(rotateRoutine);
    }
}
