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
        ADJUST_FACTOR = 10 * (float) Screen.width / Screen.height;
    }

    //rotate the background objects around the puzzle
    public void OnDrag(PointerEventData eventData) {
        
        if (eventData.pointerId == -2 && !GameController.gameOver && !GameController.fallingLock) {
            Camera.main.transform.RotateAround(Vector3.zero,Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );
            
            directionalLight.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            directionalLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );
            
            spotLight.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            spotLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );
            
//            background.transform.RotateAround(Vector3.zero, pedestal.transform.up, eventData.delta.x / ADJUST_FACTOR);
//            background.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );
            
            pedestal.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            pedestal.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

            reflectionCamera.transform.RotateAround(Vector3.zero,Vector3.up, eventData.delta.x / ADJUST_FACTOR);
//            reflectionCamera.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / ADJUST_FACTOR) );

            Physics.gravity = -pedestal.transform.up * 70;
            
        }

    #region oldrotation
//        if (eventData.pointerId == -2 && !GameController.rotating && !GameController.moving && !GameController.teleporting && !GameController.gameOver) {
//            
//            foreach (var cube in GameController.cubes) {
//                cube.transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//                cube.transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                    (eventData.delta.y / ADJUST_FACTOR) );
//            }
//            
//            foreach (var arrow in GameController.arrows) {
//                arrow.transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//                arrow.transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                    (eventData.delta.y / ADJUST_FACTOR) );
//            } 
//            
//            foreach (var rotator in GameController.rotators) {
//                rotator.transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//                rotator.transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                    (eventData.delta.y / ADJUST_FACTOR) );
//            }
//            
//            foreach (var teleporter in GameController.teleporters) {
//                teleporter.transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//                teleporter.transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                    (eventData.delta.y / ADJUST_FACTOR) );
//            }
//            
//            foreach (var locky in GameController.locks) {
//                locky.transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//                locky.transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                    (eventData.delta.y / ADJUST_FACTOR) );
//            }
//            
//            foreach (var key in GameController.keys) {
//                key.transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//                key.transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                    (eventData.delta.y / ADJUST_FACTOR) );
//            }
//
//            GameObject rotatorStrips = GameObject.FindGameObjectWithTag("rotatorStrips");
//            for (int i = 0; i < rotatorStrips.transform.childCount; i++) {
//                rotatorStrips.transform.GetChild(i).transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//                rotatorStrips.transform.GetChild(i).transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                    (eventData.delta.y / ADJUST_FACTOR) );
//            }
//
//            GameObject thing = GameObject.FindGameObjectWithTag("thing");
//            thing.transform.RotateAround(Vector3.zero, Vector3.up, -eventData.delta.x / ADJUST_FACTOR);
//            thing.transform.RotateAround(Vector3.zero, Camera.main.transform.right,
//                (eventData.delta.y / ADJUST_FACTOR) );
//
//            //Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, eventData.delta.x / 5);
//            //Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right,-(eventData.delta.y / 5) );
//        }
    #endregion
    }

}
