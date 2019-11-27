using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rotator : RotatorParent {
    
    public float rotationSpeed;
    
    //needed for fading to white while hovering over rotatorStrip
    private float hoverTime;
    private bool isClicked;
    private bool isHovering;
    
    public AudioClip to;
    public AudioClip fro;

    //accept w,a,s,d,q,e for rotation
    private string[] acceptedInputStrings;

    private bool keyPressed;

    private void Awake() {

        switch (gameObject.tag) {

            case "rotatorStripX": {
                acceptedInputStrings = new [] {"a", "d"};
                break;
            }   
            case "rotatorStripY": {
                acceptedInputStrings = new []{"w", "s"};
                break;
            }   
            case "rotatorStripZ": {
                acceptedInputStrings = new [] {"q", "e"};
                break;
            }
        }

        base.Setup();
    }

    IEnumerator Rotate(bool reverse = false) {

        GameController.rotating = true;

        
        if (GameController.DEBUG) {
            if (reverse) {
                GameController.hitLock = true;
                if (GameController.lastrotations.Count > 0)
                    GameController.lastrotations.Pop();
            }
        }
        
        hasBeenRotated = true;

        if(lastAngle <= 0)
            gameObject.GetComponent<AudioSource>().PlayOneShot(to);
        else
            gameObject.GetComponent<AudioSource>().PlayOneShot(fro);
        
        GameObject thing = GameObject.FindWithTag("thing");
        
        float timeCount = 0;
        
        //flip rotation direction by sign
        Quaternion tempFrom = thing.transform.rotation;
        Quaternion tempTo = Quaternion.AngleAxis(Mathf.Sign(lastAngle) * 90f, transform.up) * tempFrom;
   
        if (reverse)
            tempTo = reverseFrom;
        else
            reverseFrom = tempFrom;
        
        while (thing.transform.rotation != tempTo) {
            GameController.rotating = true;
            thing.transform.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
            timeCount += Time.fixedDeltaTime * rotationSpeed;
            yield return new WaitForFixedUpdate();
        }

        thing.transform.rotation = tempTo;
        
        if (GameController.hardmode == 1 && !reverse) {
            GameController.CountDownRotations();
        }
        
        GameController.lastRotatorStrip = null;
        GameController.rotating = false;
        
    }

    private IEnumerator OnMouseOver() {

        if (!enabled) yield break;
        
        isHovering = true;
        StopCoroutine(nameof(OnMouseExit));

        //Dont change color if rotatorStip isn't faded in (alpha <1)
        if (gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f && !GameController.rotatorClicked) {

            Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;

            gameObject.GetComponentInParent<MeshRenderer>().material.color =
                Color.Lerp(currentColor, Color.white, hoverTime);
            hoverTime += Time.deltaTime;
            yield return null;
        }

    }

    private IEnumerator OnMouseExit() {

        if (!enabled) yield break;

        if (!isClicked) {
            hoverTime = 0;
            float timeCount = 0;
            Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;

            while (gameObject.GetComponentInParent<MeshRenderer>().material.color != rotatorColor) {

                gameObject.GetComponentInParent<MeshRenderer>().material.color =
                    Color.Lerp(currentColor, rotatorColor, timeCount);
                timeCount += Time.deltaTime;
                yield return null;

            }
        }
        
        isHovering = false;
    }

    private void OnMouseDown() {

        if (!enabled) return;

        if (!GameController.rotating && !GameController.moving && !GameController.teleporting && GameController.rotatingColliders.Count == 0) {
            gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.white;
            ResetValues();
            isClicked = true;
            GameController.rotatorClicked = true;
        }

    }
    
    private void OnMouseUp() {
        
        if (!enabled || !isClicked) return;

        isClicked = false;
        GameController.rotatorClicked = false;
        
        if(!isHovering)
            StartCoroutine(nameof(OnMouseExit));

        if (!curved.gameObject.GetComponent<MeshRenderer>().enabled) {
            lineIndex = 0;
            lr.positionCount = 0;
        }

        if (!GameController.rotating && !GameController.moving && !GameController.teleporting && GameController.rotatingColliders.Count == 0 &&
            curved.gameObject.GetComponent<MeshRenderer>().enabled) {
            GameController.lastRotatorStrip = this;
            base.rotateRoutine = StartCoroutine(Rotate());
        }
    }

    private void OnMouseDrag() {  
        
        if (!enabled) return;

        if(!GameController.rotating && !GameController.moving && !GameController.teleporting && 
           GameController.rotatingColliders.Count == 0 && isClicked)
            DragMouse();

    }

    private void DrawArrow() {
        //clear last linerenders
        curved.gameObject.GetComponent<MeshRenderer>().enabled = false;
        GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripX").transform.GetChild(0).gameObject.
            GetComponent<LineRenderer>().positionCount = 0;
        GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripY").transform.GetChild(0).gameObject.
            GetComponent<LineRenderer>().positionCount = 0;
        GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripZ").transform.GetChild(0).gameObject.
            GetComponent<LineRenderer>().positionCount = 0;


        //camera position if it was on the same plane as rotatorStripX
        Vector3 correctedCameraPosition = new Vector3(Camera.main.transform.position.x, 0,
            Camera.main.transform.position.z);
        Ray ray = new Ray(transform.position, correctedCameraPosition);
        
        helper = ray.GetPoint(transform.parent.GetComponent<MeshRenderer>().bounds.size.x / 2);

        //ray doesn't really work with any rotatorstrip other than X (and even then it only works when rotation is not free)
        //so instead use closest intersection as starting point for arrow
        if (CompareTag("rotatorStripY") || CompareTag("rotatorStripZ")) {
            float distance = Mathf.Infinity;
            foreach (Transform intersection in transform.parent.transform) {
                if (intersection.name == "intersection") {
                    float temp = Vector3.Distance(correctedCameraPosition, intersection.transform.position);
                    if (temp < distance) {
                        distance = temp;
                        helper = intersection.position;
                    }
                }
            }
        }

        //correct arrow position
        float radius = 0;
        Vector3 toPosition = helper.normalized;
        if (CompareTag("rotatorStripX")) {
            radius = transform.parent.GetComponent<MeshRenderer>().bounds.size.x / 2;
            toPosition = new Vector3(helper.normalized.x * radius, -1, helper.normalized.z * radius);
        }
        else if (CompareTag("rotatorStripY")) {
//            radius = transform.parent.GetComponent<MeshRenderer>().bounds.size.y / 2;
            toPosition = new Vector3(helper.x, helper.y, -1);
        }
        else if (CompareTag("rotatorStripZ")) {
//            radius = transform.parent.GetComponent<MeshRenderer>().bounds.size.z / 2;
            toPosition = new Vector3(-1, helper.y, helper.z);
        }

        lineIndex = 0;
        lr.positionCount = 1;
        lr.SetPosition(lineIndex, toPosition);

        //draw slerped 28 degree arrow in given direction
        Vector3 from = toPosition;
        Quaternion toRotation = Quaternion.AngleAxis(lastAngle * 28f, transform.up);
        Vector3 goalPosition = toRotation * toPosition;

        float step = 0;
        while (!GameController.Compare(goalPosition, toPosition)) {
            if (lineIndex > 60) break;

            lineIndex++;
            lr.positionCount = lineIndex + 1;
            toPosition = Vector3.Slerp(from, goalPosition, step);
            lr.SetPosition(lineIndex, toPosition);
            step += 0.1f;
        }

        SetCurved(from, toPosition);
    }

    private void Update() {
        
        //rotate with keys
        if (Input.anyKeyDown && !GameController.rotating && !GameController.moving && !GameController.teleporting &&
            GameController.rotatingColliders.Count == 0) {
            if (acceptedInputStrings.Contains(Input.inputString)) {
                keyPressed = true;
                if (acceptedInputStrings[0] == Input.inputString) {
                    signedAngle = 90f;
                    lastAngle = 1;
                    if (GameController.DEBUG)
                        GameController.lastrotations.Push(GameObject.FindWithTag("thing").transform.rotation);
                    if (GameController.keyDownTime == 0) {
                        DrawArrow();
                        base.rotateRoutine = StartCoroutine(Rotate());
                    }
                }
                else if (acceptedInputStrings[1] == Input.inputString) {
                    signedAngle = -90f;
                    lastAngle = -1;
                    if (GameController.DEBUG)
                        GameController.lastrotations.Push(GameObject.FindWithTag("thing").transform.rotation);
                    if (GameController.keyDownTime == 0) {
                        DrawArrow();
                        base.rotateRoutine = StartCoroutine(Rotate());
                    }
                }
                GameController.lastRotatorStrip = this;
            }
        }
        
        //wait a bit before you can rotate with keys again
        if (keyPressed) {
            GameController.keyDownTime += Time.deltaTime;
            if (GameController.keyDownTime > GameController.MINKEYDOWNTIME) {
                keyPressed = false;
                GameController.keyDownTime = 0;
            }
        }
        
    }
}
