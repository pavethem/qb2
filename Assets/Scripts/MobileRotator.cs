#undef DEBUG
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MobileRotator : RotatorParent {
    
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

    private Quaternion reverseFrom;

    public AudioClip to;
    public AudioClip fro;

    private bool clicked;
    private bool mouseOverObject;
    private bool grown;
    private bool shrunk = true;
    private bool fadedIn;
    private bool fadedOut = true;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Coroutine fadeInRoutine;
    private Coroutine fadeOutRoutine;
    private Coroutine growRoutine;
    private Coroutine shrinkRoutine;

    private void Awake() {

        rotatorColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;
        rotatorColor.a = 1.0f;
        originalScale = transform.parent.transform.localScale;
        targetScale = new Vector3(transform.parent.transform.localScale.x * 1.1f,
            transform.parent.transform.localScale.y * 4, transform.parent.transform.localScale.z * 1.1f);
    }

    private void Update() {

        if(Input.GetMouseButtonDown(0) && !mouseOverObject && clicked && 
           Input.mousePosition.y > GameObject.FindWithTag("mobileimage").GetComponent<RectTransform>().rect.height) {
            signedAngle = 0;
            helper = Vector3.zero;
            helperDown = Vector3.zero;
            clicked = false;
            if(fadeInRoutine != null)
                StopCoroutine(fadeInRoutine);
            if(growRoutine != null)
                StopCoroutine(growRoutine);
            fadedIn = false;
            grown = false;
            fadeOutRoutine = StartCoroutine(FadeOut());
            shrinkRoutine = StartCoroutine(Shrink());
        }
    }

    private void OnMouseEnter() {
        if(!enabled) return;
        mouseOverObject = true;
    }

    private void OnMouseExit() {
        if(!enabled) return;
        mouseOverObject = false;
    }
    
    private void OnMouseDown() {
        
        if (!enabled) return;

        if (grown && fadedIn) {
            GameController.rotatorClicked = true;

            signedAngle = 0f;

            plane = new Plane(transform.up, 0);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out var hit, Mathf.Infinity,
                1 << LayerMask.NameToLayer("RotatorStrips"));

            //set helper to hitpoint
            plane.Raycast(ray, out var enter);
            helper = ray.GetPoint(enter);
//            helper = Vector3.ProjectOnPlane(hit.point, transform.up);
            helperDown = helper;
        }

    }
    
    private void OnMouseUp() {
        
        if (!enabled) return;

        if (grown && fadedIn) {
            GameController.rotatorClicked = false;
            helper = Vector3.zero;
            helperDown = Vector3.zero;
            
            if (!GameController.rotating && !GameController.moving && !GameController.teleporting &&
                Mathf.Abs(signedAngle) > ANGLEMIN) {
                GameController.lastRotatorStrip = this;
                base.rotateRoutine = StartCoroutine(Rotate());
            }
        }
    }
    
    private void OnMouseDrag() {  
        
        if (!enabled) return;

        if (grown && fadedIn) {
            //update helper position along transform's plane and calculate angle between it and mousedown helper position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            plane.Raycast(ray, out var enter);
            
            helper = ray.GetPoint(enter);
            signedAngle = Vector3.SignedAngle(helperDown, helper, transform.up);

        }
    }

    private void OnMouseUpAsButton() {

        if(!enabled) return;

        if (shrunk && fadedOut && !fadedIn && !grown) {
            if (!clicked) {
                if(fadeOutRoutine != null)
                    StopCoroutine(fadeOutRoutine);
                if(shrinkRoutine != null)
                    StopCoroutine(shrinkRoutine);
                fadedOut = false;
                shrunk = false;
                fadeInRoutine = StartCoroutine(FadeIn());
                growRoutine = StartCoroutine(Grow());
            }

            clicked = true;
        }

    }

    private IEnumerator Grow() {
        float timeStep = 0;
        while (!GameController.Compare(targetScale,transform.parent.transform.localScale)) {
            float x = Mathf.Lerp(transform.parent.transform.localScale.x, targetScale.x, timeStep);
            float y = Mathf.Lerp(transform.parent.transform.localScale.y, targetScale.y, timeStep);
            float z = Mathf.Lerp(transform.parent.transform.localScale.z, targetScale.z, timeStep);
            transform.parent.transform.localScale = new Vector3(x,y,z);
            timeStep += Time.deltaTime;
            yield return null;
        }
        
        transform.parent.localScale = targetScale;
        grown = true;
    }
    
    private IEnumerator Shrink() {
        float timeStep = 0;
        while (!GameController.Compare(originalScale,transform.parent.transform.localScale)) {
            float x = Mathf.Lerp(transform.parent.transform.localScale.x, originalScale.x, timeStep);
            float y = Mathf.Lerp(transform.parent.transform.localScale.y, originalScale.y, timeStep);
            float z = Mathf.Lerp(transform.parent.transform.localScale.z, originalScale.z, timeStep);
            transform.parent.transform.localScale = new Vector3(x,y,z);
            timeStep += Time.deltaTime;
            yield return null;
        }

        transform.parent.localScale = originalScale;
        shrunk = true;
    }

    private IEnumerator FadeIn() {
        if (gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f) {

            float timeStep = 0;
            Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;
            while (gameObject.GetComponentInParent<MeshRenderer>().material.color != Color.white) {
            
                gameObject.GetComponentInParent<MeshRenderer>().material.color =
                    Color.Lerp(currentColor, Color.white, timeStep);
                timeStep += Time.deltaTime*2;
                yield return null;
            }

            gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.white;
            fadedIn = true;
        }
    }
    
    private IEnumerator FadeOut() {
        if (gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f) {
            
            float timeStep = 0;
            Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;
            while (gameObject.GetComponentInParent<MeshRenderer>().material.color != rotatorColor) {
            
                gameObject.GetComponentInParent<MeshRenderer>().material.color =
                    Color.Lerp(currentColor, rotatorColor, timeStep);
                timeStep += Time.deltaTime*2;
                yield return null;
            }

            gameObject.GetComponentInParent<MeshRenderer>().material.color = rotatorColor;
            fadedOut = true;
        }
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
}
