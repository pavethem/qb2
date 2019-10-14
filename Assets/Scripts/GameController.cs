#undef DEBUG
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {

    public static GameController instance = null;
    
    //less "accurate" than unity's implementation
    private const float EPSILON = 9.99999944E-5f;

    //all cubes in scene
    public static GameObject[] cubes;
    public static int cubeCount;
    public static List<GameObject> locks;
    public static List<GameObject> keys;
    public static GameObject[] arrows;
    public static GameObject[] rotators;
    public static GameObject[] teleporters;

    //backup original rotations to reset them when gameover
    private static Quaternion pedestalRotation;
    private static Vector3 pedestalPosition;
    private static Quaternion backgroundRotation;
    private static Vector3 backgroundPosition;
    private static Quaternion spotlightRotation;
    private static Vector3 spotlightPosition;
    private static Quaternion directionalLightRotation;
    private static Vector3 directionalLightPosition;
    private static Quaternion cameraRotation;
    private static Vector3 cameraPosition;
    private static Vector3 reflectionCameraPosition;
    private static Quaternion reflectionCameraRotation;
    private static Vector3 gravity;
    
    public static bool gameOver;
    private bool isLoadingNextLevel;

    //do not rotate while the lock is falling (after the game rotation change)
    public static bool rotating;
    public static bool moving;
    public static bool teleporting;
    public static bool fallingLock;

    public static int currentScene;
    //last rotator to be used (needed to reverse rotations when hitting locks)
    public static Rotator lastRotatorStrip;
    public static RotateSpoke lastRotateSpoke;
    public static bool rotatorClicked;

    public AudioClip transition;
    
    //use Z to go to last rotation
    #if DEBUG
    public static Stack<Quaternion> lastrotations;
    private float solveTimeout = 0;
    private List<string> inputs = new List<string>();
    private List<string> solved = new List<string>();
#endif
    private void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (currentScene == 0) {
            
            pedestalPosition = GameObject.Find("Pedestal").transform.position;
            pedestalRotation = GameObject.Find("Pedestal").transform.rotation;
//            backgroundPosition = GameObject.Find("Background").transform.position;
//            backgroundRotation = GameObject.Find("Background").transform.rotation;
            spotlightPosition = GameObject.Find("Spot Light").transform.position;
            spotlightRotation = GameObject.Find("Spot Light").transform.rotation;
            directionalLightPosition = GameObject.Find("Directional Light").transform.position;
            directionalLightRotation = GameObject.Find("Directional Light").transform.rotation;
            cameraPosition = Camera.main.transform.position;
            cameraRotation = Camera.main.transform.rotation;
            reflectionCameraPosition = GameObject.Find("ReflectionCamera").transform.position;
            reflectionCameraRotation = GameObject.Find("ReflectionCamera").transform.rotation;
            gravity = Physics.gravity;

            currentScene = 1;
            #if (DEBUG)
                SceneManager.LoadScene("test");
                lastrotations = new Stack<Quaternion>();
            #else
                Load();
                SceneManager.LoadScene("level" + currentScene + "_final");
            #endif

        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
        gameObject.GetComponent<AudioSource>().Play();
        InitGame();
        
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {   
        InitGame();

    }

    void InitGame() {

        StopAllCoroutines();

        if (Application.isMobilePlatform) {
            foreach(var child in GameObject.Find("Canvas").transform.GetComponentsInChildren<Button>(true))
            {
                if(child.name == "RightButton" || child.name == "LeftButton")
                    child.gameObject.SetActive(true);
                
            }
        }
        
        cubeCount = 0;
        gameOver = false;
        isLoadingNextLevel = false;
        cubes = GameObject.FindGameObjectsWithTag("node");

        arrows = GameObject.FindGameObjectsWithTag("arrow");
        rotators = GameObject.FindGameObjectsWithTag("rotator");
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");
        keys = GameObject.FindGameObjectsWithTag("key").ToList();
        locks = GameObject.FindGameObjectsWithTag("lock").ToList();

        rotating = false;
        moving = false;
        teleporting = false;
        fallingLock = false;
        rotatorClicked = false;
        
        //destroy all things lying on the pedestal
        foreach (Transform child in GameObject.Find("Pedestal").transform) {
            Destroy(child.gameObject);
        }
        
        #if (DEBUG)
        lastrotations.Clear();
        #endif

        Camera.main.transform.rotation = cameraRotation;
        Camera.main.transform.position = cameraPosition;
        GameObject.Find("Pedestal").transform.rotation = pedestalRotation;
        GameObject.Find("Pedestal").transform.position = pedestalPosition;
        GameObject.Find("Spot Light").transform.rotation = spotlightRotation;
        GameObject.Find("Spot Light").transform.position = spotlightPosition;
        GameObject.Find("Directional Light").transform.rotation = directionalLightRotation;
        GameObject.Find("Directional Light").transform.position = directionalLightPosition;
        GameObject.Find("ReflectionCamera").transform.position = reflectionCameraPosition;
        GameObject.Find("ReflectionCamera").transform.rotation = reflectionCameraRotation;
//        GameObject.Find("Background").transform.rotation = backgroundRotation;
//        GameObject.Find("Background").transform.position = backgroundPosition;
        Physics.gravity = gravity;

    }

    void LateUpdate() {

        #if (DEBUG)
        if(!rotating && !moving && !teleporting)
//            Solve();
        #endif
        
        //fade in Audio
        if (gameObject.GetComponent<AudioSource>().volume < 0.5f)
        {
            gameObject.GetComponent<AudioSource>().volume = gameObject.GetComponent<AudioSource>().volume + Time.deltaTime;
        }

        //reset scene
        if (Input.GetKeyUp(KeyCode.R)) {
            Reset();
        }
        
        //DELETE THIS EVENTUALLY
        if (Input.GetKeyUp(KeyCode.L)) {
            GameOver();
        }
        if (Input.GetKeyUp(KeyCode.K)) {
            if(currentScene > 1)
                currentScene -= 2;
            GameOver();
        }
        
        #if (DEBUG)
        if (Input.GetKeyUp(KeyCode.Y)) {
            GameObject.FindWithTag("thing").transform.rotation = lastrotations.Pop();
        }
        #endif

        if (gameOver && !isLoadingNextLevel) {
            
            GameOver();

        }

    }

    void FixedUpdate() {

        //turn on physics for all cubes in scene and add a small amount of velocity and torque
        if (cubeCount == cubes.Length && cubes.Length > 0 && !gameOver) {

            foreach (var cube in cubes) {
                
                cube.GetComponent<Rigidbody>().useGravity = true;
                cube.GetComponent<UnityEngine.Collider>().isTrigger = false;
                cube.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0,5),Random.Range(5,10),Random.Range(0,5)),ForceMode.VelocityChange);
                cube.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(10,20),Random.Range(10,20),Random.Range(10,20)),ForceMode.Impulse);

                foreach (Transform child in cube.transform) {
                    child.gameObject.SetActive(true);
                }
                
            }
            
            gameOver = true;

        }
        
    }

    //unlock lock with key
    public static void RemoveLock(GameObject locky, GameObject key) {

        fallingLock = true;

        locks.Remove(locky);
        keys.Remove(key);

        locky.GetComponent<Rigidbody>().useGravity = true;
        locky.GetComponent<UnityEngine.Collider>().isTrigger = false;
//        locky.GetComponent<Rigidbody>().AddForce(Vector3.down,ForceMode.Acceleration);
        locky.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(10,20),Random.Range(10,20),Random.Range(10,20)),ForceMode.Impulse);

        foreach (Transform child in locky.transform) {
            child.gameObject.SetActive(true);
        }
        
    }

    public void Reset() {
        
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
        
    }

    void GameOver() {

        isLoadingNextLevel = true;
        currentScene++;
        string scene = "level" + currentScene + "_final";
        #if (DEBUG)
            StartCoroutine(nameof(LoadYourAsyncScene),"test");
        #else
            Save();
            StartCoroutine(nameof(LoadYourAsyncScene),scene);
        #endif

        
    }
    
    IEnumerator LoadYourAsyncScene(string scene)
    {
        //wait a bit
        if (currentScene > 1) {
            yield return new WaitForSeconds(2.1f);
            GetComponent<AudioSource>().PlayOneShot(transition);
            yield return new WaitForSeconds(0.4f);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
    }
    
    public static bool Compare(Vector3 lhs, Vector3 rhs)
    {
        return Vector3.SqrMagnitude(lhs - rhs) < EPSILON;
    }

    private static void Save() {
        PlayerPrefs.SetInt("currentScene", currentScene);
        PlayerPrefs.Save();
    }
    
    private static void Load() {
        currentScene = PlayerPrefs.GetInt("currentScene",1);
    }
    
#if (DEBUG)
    private void Solve() {


        int random = Random.Range(1, 7);
        solveTimeout += Time.deltaTime;
        if (solveTimeout >= 0.5f && !gameOver) {
            switch (random) {
                case 1: {
                    GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>().signedAngle =
                        90f;
                    GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                        .StartCoroutine("Rotate", false);
                    lastRotatorStrip = GameObject.FindWithTag("rotatorStripX").transform.GetChild(0)
                        .GetComponent<Rotator>();
                    solveTimeout = 0;
                    inputs.Add("a");
                    break;
                }
                case 2: {
                    GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>().signedAngle =
                        -90f;
                    GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                        .StartCoroutine("Rotate", false);
                    lastRotatorStrip = GameObject.FindWithTag("rotatorStripX").transform.GetChild(0)
                        .GetComponent<Rotator>();
                    solveTimeout = 0;
                    inputs.Add("d");
                    break;

                }
                case 3: {
                    GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>().signedAngle =
                        90f;
                    GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                        .StartCoroutine("Rotate", false);
                    lastRotatorStrip = GameObject.FindWithTag("rotatorStripY").transform.GetChild(0)
                        .GetComponent<Rotator>();
                    solveTimeout = 0;
                    inputs.Add("w");
                    break;
                }
                case 4: {
                    GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>().signedAngle =
                        -90f;
                    GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                        .StartCoroutine("Rotate", false);
                    lastRotatorStrip = GameObject.FindWithTag("rotatorStripY").transform.GetChild(0)
                        .GetComponent<Rotator>();
                    solveTimeout = 0;
                    inputs.Add("s");
                    break;
                }
                case 5: {
                    GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>().signedAngle =
                        90f;
                    GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                        .StartCoroutine("Rotate", false);
                    lastRotatorStrip = GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0)
                        .GetComponent<Rotator>();
                    solveTimeout = 0;
                    inputs.Add("e");
                    break;
                }
                case 6: {
                    GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>().signedAngle =
                        -90f;
                    GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                        .StartCoroutine("Rotate", false);
                    lastRotatorStrip = GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0)
                        .GetComponent<Rotator>();
                    solveTimeout = 0;
                    inputs.Add("q");
                    break;
                }
            }
        }

        if (inputs.Count > solved.Count && solved.Count > 0 && !gameOver) {
            inputs.Clear();
            Reset();
        }

        if (gameOver && !isLoadingNextLevel) {
            string finishedinputs = "";
            foreach (var entry in inputs) {
                finishedinputs += entry + ", ";
            }

            if (solved.Count > inputs.Count || solved.Count == 0) {
                finishedinputs += inputs.Count;
                solved.Clear();
                solved.AddRange(inputs);
                Debug.Log(finishedinputs);
                ScreenCapture.CaptureScreenshot("/media/Storage/" + solved.Count + " Moves " + Time.deltaTime + random);
            }
            inputs.Clear();
        }

    }
#endif
}


