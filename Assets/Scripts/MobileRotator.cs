using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;

public class MobileRotator : RotatorParent {
#region oldrotation
    //    
//    public float rotationSpeed;
//
//    public AudioClip to;
//    public AudioClip fro;
//
//    //if rotatorstrip is selected
//    private bool clicked;
//    private bool mouseOverObject;
//    //states of the rotatorstrip
//    private bool grown;
//    private bool shrunk = true;
//    private bool fadedIn;
//    private bool fadedOut = true;
//    private Vector3 originalScale;
//    private Vector3 targetScale;
//
//    private const float targetScaleXZ = 1.1f;
//    private const float targetScaleY = 4f;
//
//    //save routines so you can cancel them at any time
//    private Coroutine fadeInRoutine;
//    private Coroutine fadeOutRoutine;
//    private Coroutine growRoutine;
//    private Coroutine shrinkRoutine;
//
//    private void Awake() {
//        
//        originalScale = transform.parent.transform.localScale;
//        targetScale = new Vector3(transform.parent.transform.localScale.x * targetScaleXZ,
//            transform.parent.transform.localScale.y * targetScaleY, transform.parent.transform.localScale.z * targetScaleXZ);
//        
//        base.Setup();
//    }
//
//    private void Update() {
//
//        //if rotatorstrip is selected but mouse clicks anywhere else on the screen (excluding the game rotation area on the bottom)
//        //shrink rotatorstrip again
//        if(Input.GetMouseButtonDown(0) && !mouseOverObject && clicked && 
//           Input.mousePosition.y > GameObject.FindWithTag("mobileimage").GetComponent<RectTransform>().rect.height) {
//            signedAngle = 0;
//            helper = Vector3.zero;
//            helperDown = Vector3.zero;
//            clicked = false;
//            if(fadeInRoutine != null)
//                StopCoroutine(fadeInRoutine);
//            if(growRoutine != null)
//                StopCoroutine(growRoutine);
//            fadedIn = false;
//            grown = false;
//            fadeOutRoutine = StartCoroutine(FadeOut());
//            shrinkRoutine = StartCoroutine(Shrink());
//        }
//    }
//
//    private void OnMouseEnter() {
//        if(!enabled) return;
//        mouseOverObject = true;
//    }
//
//    private void OnMouseExit() {
//        if(!enabled) return;
//        mouseOverObject = false;
//    }
//    
//    private void OnMouseDown() {
//        
//        if (!enabled) return;
//
//        //set up rotation when rotatorstrip is grown
//        if (grown && fadedIn) {
//            
//            base.ResetValues();
//        }
//
//    }
//    
//    private void OnMouseUp() {
//        
//        if (!enabled) return;
//
//        //start rotation when rotatorstrip is grown
//        if (grown && fadedIn) {
//            GameController.rotatorClicked = false;
//            helper = Vector3.zero;
//            helperDown = Vector3.zero;
//            
//            if (!curved.gameObject.GetComponent<MeshRenderer>().enabled) {
//                lineIndex = 0;
//                lr.positionCount = 0;
//            }
//            
//            if (!GameController.rotating && !GameController.moving && !GameController.teleporting && 
//                curved.gameObject.GetComponent<MeshRenderer>().enabled) {
//                GameController.lastRotatorStrip = this;
//                base.rotateRoutine = StartCoroutine(Rotate());
//            }
//        }
//    }
//    
//    private void OnMouseDrag() {  
//        
//        if (!enabled) return;
//
//        if (grown && fadedIn) {
//           
//            DragMouse();
//
//        }
//    }
//
//    //used only when rotatorstrip is NOT grown
//    private void OnMouseUpAsButton() {
//
//        if(!enabled) return;
//        
//        if (shrunk && fadedOut && !fadedIn && !grown) {
//            if (!clicked && gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f) {
//                if(fadeOutRoutine != null)
//                    StopCoroutine(fadeOutRoutine);
//                if(shrinkRoutine != null)
//                    StopCoroutine(shrinkRoutine);
//                fadedOut = false;
//                shrunk = false;
//                fadeInRoutine = StartCoroutine(FadeIn());
//                growRoutine = StartCoroutine(Grow());
//            }
//
//            clicked = true;
//        }
//
//    }
//
//    private IEnumerator Grow() {
//        if (gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f) {
//            float timeStep = 0;
//            while (!GameController.Compare(targetScale, transform.parent.transform.localScale)) {
//                float x = Mathf.Lerp(transform.parent.transform.localScale.x, targetScale.x, timeStep);
//                float y = Mathf.Lerp(transform.parent.transform.localScale.y, targetScale.y, timeStep);
//                float z = Mathf.Lerp(transform.parent.transform.localScale.z, targetScale.z, timeStep);
//                transform.parent.transform.localScale = new Vector3(x, y, z);
//                timeStep += Time.deltaTime;
//                yield return null;
//            }
//
//            transform.parent.localScale = targetScale;
//            grown = true;
//        }
//    }
//    
//    private IEnumerator Shrink() {
//        if (gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f) {
//            float timeStep = 0;
//            while (!GameController.Compare(originalScale, transform.parent.transform.localScale)) {
//                float x = Mathf.Lerp(transform.parent.transform.localScale.x, originalScale.x, timeStep);
//                float y = Mathf.Lerp(transform.parent.transform.localScale.y, originalScale.y, timeStep);
//                float z = Mathf.Lerp(transform.parent.transform.localScale.z, originalScale.z, timeStep);
//                transform.parent.transform.localScale = new Vector3(x, y, z);
//                timeStep += Time.deltaTime;
//                yield return null;
//            }
//
//            transform.parent.localScale = originalScale;
//            shrunk = true;
//        }
//    }
//
//    private IEnumerator FadeIn() {
//        if (gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f) {
//
//            float timeStep = 0;
//            Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;
//            while (gameObject.GetComponentInParent<MeshRenderer>().material.color != Color.white) {
//            
//                gameObject.GetComponentInParent<MeshRenderer>().material.color =
//                    Color.Lerp(currentColor, Color.white, timeStep);
//                timeStep += Time.deltaTime*2;
//                yield return null;
//            }
//
//            gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.white;
//            fadedIn = true;
//        }
//    }
//    
//    private IEnumerator FadeOut() {
//        if (gameObject.GetComponentInParent<MeshRenderer>().material.color.a == 1.0f) {
//            
//            float timeStep = 0;
//            Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;
//            while (gameObject.GetComponentInParent<MeshRenderer>().material.color != rotatorColor) {
//            
//                gameObject.GetComponentInParent<MeshRenderer>().material.color =
//                    Color.Lerp(currentColor, rotatorColor, timeStep);
//                timeStep += Time.deltaTime*2;
//                yield return null;
//            }
//
//            gameObject.GetComponentInParent<MeshRenderer>().material.color = rotatorColor;
//            fadedOut = true;
//        }
//    }
//
//    IEnumerator Rotate(bool reverse = false) {
//
//        if(!hasBeenRotated)
//            GameObject.Find("Canvas").transform.transform.Find("MobileImage").GetComponentInChildren<Button>(true).gameObject.SetActive(true);
//        
//        hasBeenRotated = true;
//
//        if(signedAngle <= 0)
//            gameObject.GetComponent<AudioSource>().PlayOneShot(to);
//        else
//            gameObject.GetComponent<AudioSource>().PlayOneShot(fro);
//        
//        GameObject thing = GameObject.FindWithTag("thing");
//        
//        float timeCount = 0;
//        
//        //flip rotation direction by sign
//        Quaternion tempFrom = thing.transform.rotation;
//        Quaternion tempTo = Quaternion.AngleAxis(Mathf.Sign(signedAngle) * 90f, transform.up) * tempFrom;
//   
//        if (reverse)
//            tempTo = reverseFrom;
//        else
//            reverseFrom = tempFrom;
//        
//        while (thing.transform.rotation != tempTo) {
//            GameController.rotating = true;
//            thing.transform.rotation = Quaternion.Lerp(tempFrom, tempTo, timeCount);
//            timeCount += Time.deltaTime * rotationSpeed;
//            yield return null;
//        }
//
//        thing.transform.rotation = tempTo;
//        GameController.rotating = false;
//        GameController.lastRotatorStrip = null;
//
//    }
#endregion

    public float rotationSpeed;

    private bool isClicked;
    
    public AudioClip to;
    public AudioClip fro;

    private void Awake() {
        base.Setup();
    }

    IEnumerator Rotate(bool reverse = false) {

        if (GameController.DEBUG) {
            if (reverse) {
                if (GameController.lastrotations.Count > 0)
                    GameController.lastrotations.Pop();
            }
        }

        if(!hasBeenRotated)
            GameObject.Find("Canvas").transform.transform.Find("MobileImage").GetComponentInChildren<Button>(true).gameObject.SetActive(true);
        
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
        GameController.rotating = false;
        GameController.lastRotatorStrip = null;
        
    }

    private IEnumerator OnMouseDown() {
        
        if (!enabled) yield break;
        
        GameController.rotatorClicked = true;
        isClicked = true;
        ResetValues();
        
        StopCoroutine(nameof(OnMouseUp));
        StopCoroutine(nameof(FadeOut));

        float timeCount = 0;
        Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;

        while (gameObject.GetComponentInParent<MeshRenderer>().material.color != Color.white) {
            
            gameObject.GetComponentInParent<MeshRenderer>().material.color =
                Color.Lerp(currentColor, Color.white, timeCount);
            timeCount += Time.deltaTime;
            yield return null;

        }
        gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.white;

    }
    
    private IEnumerator OnMouseUp() {
        
        if (!enabled) yield break;

        GameController.rotatorClicked = false;
        isClicked = false;

        if (!curved.gameObject.GetComponent<MeshRenderer>().enabled) {
            lineIndex = 0;
            lr.positionCount = 0;
        }

        if (!GameController.rotating && !GameController.moving && !GameController.teleporting && 
            curved.gameObject.GetComponent<MeshRenderer>().enabled) {
            GameController.lastRotatorStrip = this;
            base.rotateRoutine = StartCoroutine(Rotate());
            StartCoroutine(nameof(FadeIn));
        }
        else {

            StopCoroutine(nameof(OnMouseDown));
            float timeCount = 0;
            Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;

            while (gameObject.GetComponentInParent<MeshRenderer>().material.color != rotatorColor) {
                gameObject.GetComponentInParent<MeshRenderer>().material.color =
                    Color.Lerp(currentColor, rotatorColor, timeCount);
                timeCount += Time.deltaTime;
                yield return null;
            }
            gameObject.GetComponentInParent<MeshRenderer>().material.color = rotatorColor;
        }
    }
    
    private IEnumerator FadeIn() {
        float timeCount = 0;
        Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;

        while (gameObject.GetComponentInParent<MeshRenderer>().material.color != Color.white) {
            
            gameObject.GetComponentInParent<MeshRenderer>().material.color =
                Color.Lerp(currentColor, Color.white, timeCount);
            timeCount += Time.deltaTime;
            yield return null;

        }

        gameObject.GetComponentInParent<MeshRenderer>().material.color = Color.white;
    }

    private IEnumerator FadeOut() {
        float timeCount = 0;
        Color currentColor = gameObject.GetComponentInParent<MeshRenderer>().material.color;

        while (gameObject.GetComponentInParent<MeshRenderer>().material.color != rotatorColor) {
            gameObject.GetComponentInParent<MeshRenderer>().material.color =
                Color.Lerp(currentColor, rotatorColor, timeCount);
            timeCount += Time.deltaTime;
            yield return null;
        }

        gameObject.GetComponentInParent<MeshRenderer>().material.color = rotatorColor;
    }

    private void OnMouseDrag() {  
        
        if (!enabled) return;

        DragMouse();

    }

    private void Update() {

        if (gameObject.GetComponentInParent<MeshRenderer>().material.color == Color.white && !GameController.rotating && !isClicked)
            StartCoroutine(nameof(FadeOut));

    }
}
