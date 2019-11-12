using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResize : MonoBehaviour {
    private float scalefactor;
    private bool readytoScale;

    protected void OnRectTransformDimensionsChange() {
        
        //resize canvas, but only up to a point (scalefactor of 0 means fullscreen)
        if (Screen.width > 1100) {
            if(scalefactor != 0)
                GetComponent<Canvas>().scaleFactor = scalefactor;
            else {
                GetComponent<Canvas>().scaleFactor = 1.33f;
            }
        }
        else {
            scalefactor = GetComponent<Canvas>().scaleFactor;
        }
    }
    
}
