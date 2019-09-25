using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rotator : MonoBehaviour {
    
    public float rotationSpeed;
    
    //fade in resetButton when game has been rotated at least once
    private bool hasBeenRotated;
    
    //helper to calculate angle between mouseDown and MouseUp
    private Vector3 helper;
    //basically ray hit position on mousedown
    private Vector3 helperDown;
    //plane of this rotatorStrip
    private Plane plane;   
    //angle of the desired rotation
    internal float signedAngle;
    
    private const float ANGLEMIN = 5f;

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
        
        if (GameController.debug && reverse) {
            if(GameController.lastrotations.Count > 0)
                GameController.lastrotations.Pop();
        }

        if(!hasBeenRotated)
            GameObject.Find("Canvas").transform.GetComponentInChildren<Button>(true).gameObject.SetActive(true);
        
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

        isClicked = true;
        GameController.rotatorClicked = true;
        
        gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.white;
        
        signedAngle = 0f;
        
        plane = new Plane(transform.up, 0);

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity,1<<LayerMask.NameToLayer("RotatorStrips"));

        //set helper to hitpoint
        helper = Vector3.ProjectOnPlane(hit.point, transform.up);
        helperDown = helper;

    }
    
    private void OnMouseUp() {

        isClicked = false;
        GameController.rotatorClicked = false;
        
        if(!isHovering)
            StartCoroutine(nameof(OnMouseExit));
        
        if (!GameController.rotating && !GameController.moving && !GameController.teleporting && Mathf.Abs(signedAngle) > ANGLEMIN) {
            GameController.lastRotatorStrip = this;
            StartCoroutine(Rotate());
        }
    }

    private void OnMouseDrag() {  
      
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
                    if (GameController.debug) {
                        GameController.lastrotations.Push(GameObject.FindWithTag("thing").transform.rotation);
                    }
                    if(keyDownTime==0)
                        StartCoroutine(Rotate());
                }
                else if (acceptedInputStrings[1] == Input.inputString) {
                    signedAngle = -90f;
                    if (GameController.debug) {
                        GameController.lastrotations.Push(GameObject.FindWithTag("thing").transform.rotation);
                    }
                    if(keyDownTime==0)
                        StartCoroutine(Rotate());
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
