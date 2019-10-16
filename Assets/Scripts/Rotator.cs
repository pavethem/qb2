#undef DEBUG
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rotator : RotatorParent {
    
    public float rotationSpeed;
    
    //fade in resetButton when game has been rotated at least once
    private bool hasBeenRotated;
    
    //helper to calculate angle between mouseDown and MouseUp
    private Vector3 helper;
    //basically ray hit position on mousedown
    private Vector3 helperDown;
    //plane of this rotatorStrip
    private Plane plane;

    private const float ANGLEMIN = 3f;

    private Color rotatorColor;

    //needed for fading to white while hovering over rotatorStrip
    private float hoverTime;
    private bool isClicked;
    private bool isHovering;

    private Quaternion reverseFrom;

    public AudioClip to;
    public AudioClip fro;

    //accept w,a,s,d,q,e for rotation
    private string[] acceptedInputStrings;
    //wait a bit before you can rotate with keys again
    private float keyDownTime;
    private const float MINKEYDOWNTIME = 0.9f;
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

        rotatorColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;
        rotatorColor.a = 1.0f;

    }

    IEnumerator Rotate(bool reverse = false) {
        
        #if (DEBUG)
        if (reverse) {
            if(GameController.lastrotations.Count > 0)
                GameController.lastrotations.Pop();
        }
        #endif

        if(!hasBeenRotated)
            GameObject.Find("Canvas").transform.transform.Find("MobileImage").GetComponentInChildren<Button>(true).gameObject.SetActive(true);
        
        hasBeenRotated = true;

        if(signedAngle <= 0)
            gameObject.GetComponent<AudioSource>().PlayOneShot(to);
        else
            gameObject.GetComponent<AudioSource>().PlayOneShot(fro);
        
        GameObject thing = GameObject.FindWithTag("thing");
        
        float timeCount = 0;
        
        //flip rotation direction by sign
        Quaternion tempFrom = thing.transform.rotation;
        Quaternion tempTo = Quaternion.AngleAxis(Mathf.Sign(signedAngle) * 90f, transform.up) * tempFrom;
   
        if (reverse)
            tempTo = reverseFrom;
        else
            reverseFrom = tempFrom;
        
        while (thing.transform.rotation != tempTo) {
            GameController.rotating = true;
            thing.transform.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
            timeCount += Time.deltaTime * rotationSpeed;
            yield return null;
        }

        thing.transform.rotation = tempTo;
        GameController.rotating = false;
        GameController.lastRotatorStrip = null;

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

        isClicked = true;
        GameController.rotatorClicked = true;
        
        gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.white;
        
        signedAngle = 0f;
        
        plane = new Plane(transform.up, 0);
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var hit, Mathf.Infinity,1<<LayerMask.NameToLayer("RotatorStrips"));

        plane.Raycast(ray, out var enter);

        //set helper to hitpoint
        helper= ray.GetPoint(enter);
//        helper = Vector3.ProjectOnPlane(hit.point, transform.up);
        helperDown = helper;

    }
    
    private void OnMouseUp() {
        
        if (!enabled) return;

        isClicked = false;
        GameController.rotatorClicked = false;
        
        if(!isHovering)
            StartCoroutine(nameof(OnMouseExit));
        
        if (!GameController.rotating && !GameController.moving && !GameController.teleporting && Mathf.Abs(signedAngle) > ANGLEMIN) {
            GameController.lastRotatorStrip = this;
            base.rotateRoutine = StartCoroutine(Rotate());
        }
    }

    private void OnMouseDrag() {  
        
        if (!enabled) return;

        //update helper position along transform's plane and calculate angle between it and mousedown helper position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var enter);

        helper= ray.GetPoint(enter);
        
        signedAngle = Vector3.SignedAngle(helperDown, helper, transform.up);

    }

    private void Update() {

        //rotate with keys
        if (Input.anyKeyDown && !GameController.rotating && !GameController.moving && !GameController.teleporting) {
            if (acceptedInputStrings.Contains(Input.inputString)) {
                keyPressed = true;
                if (acceptedInputStrings[0] == Input.inputString) {
                    signedAngle = 90f;
                    #if (DEBUG)
                        GameController.lastrotations.Push(GameObject.FindWithTag("thing").transform.rotation);
                    #endif
                    if(keyDownTime==0)
                        base.rotateRoutine = StartCoroutine(Rotate());
                }
                else if (acceptedInputStrings[1] == Input.inputString) {
                    signedAngle = -90f;
                    #if (DEBUG)
                        GameController.lastrotations.Push(GameObject.FindWithTag("thing").transform.rotation);
                    #endif
                    if(keyDownTime==0)
                        base.rotateRoutine = StartCoroutine(Rotate());
                }
                GameController.lastRotatorStrip = this;
            }
        }

        //wait a bit before you can rotate with keys again
        if (keyPressed) {
            keyDownTime += Time.deltaTime;
            if (keyDownTime > MINKEYDOWNTIME) {
                keyPressed = false;
                keyDownTime = 0;
            }
        }
        
    }
}
