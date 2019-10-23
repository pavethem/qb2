using UnityEngine;
using UnityEngine.EventSystems;

public class GameRotator : MonoBehaviour, IDragHandler {
    
    //depends on screen size
    private float ADJUST_FACTOR;

    public GameObject directionalLight;
    public GameObject spotLight;
    public GameObject pedestal;
    public Camera reflectionCamera;
//    public GameObject background;

    private void Start() {
        ADJUST_FACTOR = 5 * (float) Screen.width / Screen.height;
    }

    //rotate the background objects around the puzzle
    public void OnDrag(PointerEventData eventData) {

        if (!Application.isMobilePlatform) {
            if (!GameController.gameOver && !GameController.fallingLock && eventData.pointerId == -2) {
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
        else {
            if (!GameController.gameOver && !GameController.fallingLock)  {
                
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

}
