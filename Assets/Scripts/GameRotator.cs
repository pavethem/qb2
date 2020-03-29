using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    public IEnumerator Rotate() {
        float angle = 0f;
        float adjust = Mathf.Pow(2f, GameController.currentScene / 10f);
        float rotateby = -2f * adjust;
        
        while (angle < 360f) {
            Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);

            directionalLight.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);

            spotLight.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);


            pedestal.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);

            reflectionCamera.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);
            angle += -rotateby;
            yield return null;

        }

        if (GameController.currentScene < GameController.LEVELCOUNT) {

            // GameController.instance.StartCoroutine("AsyncLoad","level" + GameController.currentScene + "_final");
            Scene scene = SceneManager.GetSceneByName("level" + GameController.currentScene + "_final");
            foreach (var go in scene.GetRootGameObjects()) {
                if (go.name != "rotatorStrips") {
                    go.SetActive(false);
                    Destroy(go);
                }
            }

            GameController.currentScene++;

            SceneManager.LoadScene("level" + GameController.currentScene + "_final", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
        else {
            GameController.ChangeBackgroundOption();
            StartCoroutine(KeepRotating());
        }

    }

    public IEnumerator KeepRotating() {
        float angle = 0f;
        float rotateby = -1f;
        
        while (angle < 3600f) {
            Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);

            directionalLight.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);

            spotLight.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);


            pedestal.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);

            reflectionCamera.transform.RotateAround(Vector3.zero, Vector3.up, rotateby);
            angle += -rotateby;
            yield return null;

        }
    }

    //rotate the background objects around the puzzle
    public void OnDrag(PointerEventData eventData) {

        if (!Application.isMobilePlatform) {
            if (!GameController.gameOver && !GameController.fallingLock && eventData.pointerId == -2 && !GameController.tutorialLock) {
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

                if (GameController.freeRotation == 1) {
                    Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    directionalLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    spotLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    pedestal.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    reflectionCamera.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                }

            }
        }
        else {
            if (!GameController.gameOver && !GameController.fallingLock && !GameController.tutorialLock)  {
                
                if (GameController.freeRotation == 1  && gameObject.name == "MobileImageY") {
                    Camera.main.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    directionalLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    spotLight.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    pedestal.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                    reflectionCamera.transform.RotateAround(Vector3.zero, Camera.main.transform.right, -(eventData.delta.y / ADJUST_FACTOR));
                }
                else {

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

                }

                Physics.gravity = -pedestal.transform.up * 70;
                
                
            }
        }
    }

}
