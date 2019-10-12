using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameRotator : MonoBehaviour, IDragHandler {
    
    //depends on screen size
    private float ADJUST_FACTOR;

    public GameObject directionalLight;
    public GameObject spotLight;
    public GameObject pedestal;
    public Camera reflectionCamera;
//    public GameObject background;

    private void Start() {
        ADJUST_FACTOR = 10 * (float) Screen.width / Screen.height;
    }

    private void Update() {

        if (Input.touchSupported && !Input.mousePresent) {

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved) {
                
                Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, touch.deltaPosition.x);
                //            Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

                directionalLight.transform.RotateAround(Vector3.zero, Vector3.up,
                    touch.deltaPosition.x);
                //            directionalLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

                spotLight.transform.RotateAround(Vector3.zero, Vector3.up, touch.deltaPosition.x);
                //            spotLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

                //            background.transform.RotateAround(Vector3.zero, pedestal.transform.up, eventData.delta.x / ADJUST_FACTOR);
                //            background.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

                pedestal.transform.RotateAround(Vector3.zero, Vector3.up, touch.deltaPosition.x);
                //            pedestal.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

                reflectionCamera.transform.RotateAround(Vector3.zero, Vector3.up,
                    touch.deltaPosition.x);
                //            reflectionCamera.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

                Physics.gravity = -pedestal.transform.up * 70;
            }
        }

    }

    //rotate the background objects around the puzzle
    public void OnDrag(PointerEventData eventData) {

        if ((!GameController.gameOver && !GameController.fallingLock) &&
            (eventData.pointerId == -2 && Input.mousePresent)) {
            Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

            directionalLight.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            directionalLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

            spotLight.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            spotLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

//            background.transform.RotateAround(Vector3.zero, pedestal.transform.up, eventData.delta.x / ADJUST_FACTOR);
//            background.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

            pedestal.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            pedestal.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

            reflectionCamera.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            reflectionCamera.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

            Physics.gravity = -pedestal.transform.up * 70;

        }
    }

}
