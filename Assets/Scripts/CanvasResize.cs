using UnityEngine;

public class CanvasResize : MonoBehaviour {
    private float scalefactor;
    private bool readytoScale;

    protected void OnRectTransformDimensionsChange() {

        float adjustment = 1 + (((Screen.currentResolution.width - 1920) / 1920) * 2);
        
        //resize canvas, but only up to a point (scalefactor of 0 means fullscreen)
        if (Screen.width > 1100 * adjustment) {
            if(scalefactor != 0)
                GetComponent<Canvas>().scaleFactor = scalefactor;
            else {
                GetComponent<Canvas>().scaleFactor = 1.33f + adjustment - 1;
            }
        }
        else {
            scalefactor = GetComponent<Canvas>().scaleFactor;
        }
    }
    
}
