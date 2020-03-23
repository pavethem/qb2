using UnityEngine;

public class CanvasResize : MonoBehaviour {
    private float scalefactor;
    private bool readytoScale;

    protected void OnRectTransformDimensionsChange() {

        //adjust this for resolutions higher than hd
        float adjustment = 1 + (((Screen.width - 1920f) / 1920f) * 2f);
        
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
