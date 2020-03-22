using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    public static bool DEBUG;
    public static bool SKIPSPLASHSCREEN;

    public DebugObject debugScriptableObject;
    public SkipSplashScreenObject skipSplashScreenObject;

    //for fading in and out when loading the level
    public GameObject screenWipe;
    public GameObject directionalLight;
    public GameObject mobileImage;
    public GameObject mobileImageY;
    public GameObject reflectionImage;
    public GameObject titleLevel;
    private bool wipingIn;
    public static bool wiping;

    public static GameController instance = null;

    //less "accurate" than unity's implementation
    private const float EPSILON = 9.99999944E-5f;

    //amount by which to scale rotator strip colliders on mobile
    private const float SCALEAMOUNT = 3;
    public const int LEVELCOUNT = 26;

    //ambient colors for the alternate background
    private static Color newSkyColor = new Color(0.05046278f,0.06603771f,0.05046278f);
    private static Color newGroundColor = new Color(0.1356875f,0.3918137f,0.1499713f);
    private static Color oldSkyColor;
    private static Color oldGroundColor;

    //wait a while before rotating with keys again
    public static float MINKEYDOWNTIME = 0.9f;
    public static float keyDownTime;
    public static bool keyPressed;

    //tutorial stuff
    public static bool skipTutorials;
    private string[] tutorialLevels = {"level1", "level2", "level4", "level6", "level8", "level12", "level19"};
    public static bool tutorialLock;
    private bool shownTutorial;
    public static bool gameCompleted;

    //all cubes in scene
    public static GameObject[] cubes;
    public static int cubeCount;
    public static List<GameObject> locks;
    public static List<GameObject> keys;
    public static GameObject[] arrows;
    public static GameObject[] rotators;
    public static GameObject[] teleporters;

    public GameObject skyboxCamera;

    //for hard mode
    public static int numberRotations;
    public static int maxRotations;

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

    public static bool firstTimeLoading = true;
    public static bool gameOver;
    private bool gameOverWorking;
    public static int freeRotation = -1;
    public static int hardmode = -1;
    private bool isLoadingNextLevel;
    private bool resetting;
    private bool fpstested;
    public static bool fpsTestFailed;

    //do not rotate while the lock is falling (after the game rotation change)
    public static bool rotating;
    public static bool moving;
    public static bool teleporting;
    public static bool fallingLock;

    public static bool rotatingSpoke;

    //all rotatorColliders eligible for rotation right now
    public static List<GameObject> rotatingColliders;

    public static int currentScene;

    //the maximum level reached by the player
    public static int maxScene;

    //last rotator to be used (needed to reverse rotations when hitting locks)
    public static RotatorParent lastRotatorStrip;
    public static RotateSpoke lastRotateSpoke;
    public static bool rotatorClicked;
    public static bool splashScreenDone;

    public AudioMixer mixer;
    public AudioClip transition;
    private bool doneFadingIn;
    private static bool changeBackgroundMusic;
    private static bool changedBackground;
    
    public static Stack<Quaternion> lastrotations;
    private float solveTimeout = 0;
    public static bool hitLock;
    private int lastRotation;
    private List<string> inputs = new List<string>();
    private List<string> solved = new List<string>();
    float deltaTime = 0.0f;

    private void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        Application.targetFrameRate = 60;

        if (currentScene == 0) {
            pedestalPosition = GameObject.Find("Pedestal").transform.position;
            pedestalRotation = GameObject.Find("Pedestal").transform.rotation;
            spotlightPosition = GameObject.Find("Spot Light").transform.position;
            spotlightRotation = GameObject.Find("Spot Light").transform.rotation;
            directionalLightPosition = GameObject.Find("Directional Light").transform.position;
            directionalLightRotation = GameObject.Find("Directional Light").transform.rotation;
            cameraPosition = Camera.main.transform.position;
            cameraRotation = Camera.main.transform.rotation;
            reflectionCameraPosition = GameObject.Find("ReflectionCamera").transform.position;
            reflectionCameraRotation = GameObject.Find("ReflectionCamera").transform.rotation;
            gravity = Physics.gravity;

            oldSkyColor = RenderSettings.ambientSkyColor;
            oldGroundColor = RenderSettings.ambientGroundColor;

            mobileImage.GetComponent<RectTransform>().sizeDelta = new Vector2(mobileImage.GetComponent<RectTransform>().sizeDelta.x,
                Mathf.Clamp(Screen.height / 15f, 40f, 45f));
            mobileImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,
                0 - mobileImage.GetComponent<RectTransform>().rect.height);

            mobileImageY.GetComponent<RectTransform>().sizeDelta = new Vector2(mobileImage.GetComponent<RectTransform>().sizeDelta.y * 2,
                mobileImageY.GetComponent<RectTransform>().sizeDelta.y);
            mobileImageY.GetComponent<RectTransform>().offsetMin = new Vector2(0, mobileImage.GetComponent<RectTransform>().sizeDelta.y);

            DEBUG = debugScriptableObject.DEBUG;
            SKIPSPLASHSCREEN = skipSplashScreenObject.SKIPSPLASHSCREEN;

            if (DEBUG) {
                SceneManager.LoadScene("test");
                lastrotations = new Stack<Quaternion>();
                skipTutorials = true;
            }
            else if (!SKIPSPLASHSCREEN){
                SceneManager.LoadScene("splashScreen");
            }
            else if (SKIPSPLASHSCREEN) {
                SplashScreenDone();
            }
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        //set last audio levels and other variables
        mixer.SetFloat("BackgroundVolume", PlayerPrefs.GetFloat("BackgroundVolume", 1f));
        mixer.SetFloat("EffectsVolume", PlayerPrefs.GetFloat("EffectsVolume", 1f));
        hardmode = PlayerPrefs.GetInt("HardMode", -1);
        freeRotation = PlayerPrefs.GetInt("FreeRotation", -1);
        maxScene = PlayerPrefs.GetInt("maxScene", 1);
        changedBackground = PlayerPrefs.GetInt("changedBackground",-1) == 1;
        gameCompleted = PlayerPrefs.GetInt("gameCompleted", -1) == 1;
        skipTutorials = PlayerPrefs.GetInt("skipTutorials",-1) == 1;
        
        if (maxScene > LEVELCOUNT)
            maxScene = LEVELCOUNT;

        if (changedBackground) {
            instance.reflectionImage.SetActive(false);
            GameObject.Find("ReflectionCamera").GetComponent<Camera>().enabled = false;
            RenderSettings.ambientSkyColor = newSkyColor;
            RenderSettings.ambientGroundColor = newGroundColor;
            instance.skyboxCamera.transform.Rotate(90f,0,0);
        }

        if (Application.isMobilePlatform && fpsTestFailed)
            ReplaceMeshes(titleLevel);
    }

    public static void SplashScreenDone() {

        SceneManager.LoadScene("mainmenu");
        if (!changedBackground)
            instance.reflectionImage.SetActive(true);
        else
            GameObject.Find("ReflectionCamera").GetComponent<Camera>().enabled = false;
        instance.gameObject.GetComponents<AudioSource>()[0].Play();
        splashScreenDone = true;
    }

    public void LoadCurrentScene() {
        Load();
        StartCoroutine(nameof(ScreenWipeIn), true);
    }
    
    public void LoadLastScene() {
        Load();
        //99 means bonus level
        if(currentScene <= LEVELCOUNT || currentScene == 99)
            StartCoroutine(nameof(ScreenWipeIn), true);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        if (scene.name != "tutorialScreen")
            gameOverWorking = false;
        
        if (scene.name == "mainmenu") {
            StartCoroutine(nameof(ScreenWipeOut));
            titleLevel.SetActive(true);
            gameOver = false;
            GameObject.Find("Canvas").transform.Find("DesktopImage").GetComponent<Image>().enabled = false;
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
            Physics.gravity = gravity;
        }

        if (Application.isMobilePlatform && !scene.name.StartsWith("level") && scene.name != "bonus") {
            //change to MobileButtons for main menu
            GameObject.Find("MainCanvas").transform.Find("DesktopButtons").gameObject.SetActive(false);
            GameObject.Find("MainCanvas").transform.Find("MobileButtons").gameObject.SetActive(true);
        }

        if (scene.name.StartsWith("level") && scene.name != "level0" || scene.name == "test" || scene.name == "bonus") {
            GameObject.Find("Canvas").transform.Find("MobileImage").Find("BackButton").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("MobileImage").Find("ResetButton").gameObject.SetActive(true);
            if (hardmode == 1)
                GameObject.Find("Canvas").transform.Find("HardModePanel").gameObject.SetActive(true);
            InitGame();
        }

        if (changedBackground) {
            RenderSettings.ambientSkyColor = newSkyColor;
            RenderSettings.ambientGroundColor = newGroundColor;
        }
    }

    void InitGame() {
        titleLevel.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(nameof(ScreenWipeOut));

        //fade in tutorial if level should display a tutorial
        tutorialLock = false;
        if (tutorialLevels.Contains("level" + currentScene) && !skipTutorials && !tutorialLock &&
            PlayerPrefs.GetInt("seenTutorial" + currentScene, -1) != 1 && !shownTutorial)
        {
            SceneManager.LoadScene("tutorialScreen", LoadSceneMode.Additive);
            tutorialLock = true;
            shownTutorial = true;
        }

        if (!tutorialLock) {
            StartCoroutine(nameof(MoveInButtons));
            if (Application.isMobilePlatform && freeRotation == 1)
                StartCoroutine(nameof(MoveInFromTheRight));
        }

        cubeCount = 0;
        gameOver = false;
        isLoadingNextLevel = false;
        resetting = false;
        cubes = GameObject.FindGameObjectsWithTag("node");

        if (hardmode == 1) {
            numberRotations = 0;
            maxRotations = GameObject.FindWithTag("thing").GetComponent<RotationCount>().rotationCount;
            GameObject.Find("Canvas").transform.Find("HardModePanel").GetComponentInChildren<TextMeshProUGUI>().text = "" + maxRotations;
        }

        arrows = GameObject.FindGameObjectsWithTag("arrow");
        rotators = GameObject.FindGameObjectsWithTag("rotator");
        teleporters = GameObject.FindGameObjectsWithTag("teleporter");
        keys = GameObject.FindGameObjectsWithTag("key").ToList();
        locks = GameObject.FindGameObjectsWithTag("lock").ToList();

        if (Application.isMobilePlatform) {
            //change image for rotating the game world
            mobileImage.GetComponent<RawImage>().enabled = true;
            mobileImage.GetComponent<GameRotator>().enabled = true;
            mobileImageY.GetComponent<RawImage>().enabled = true;
            mobileImageY.GetComponent<GameRotator>().enabled = true;
            GameObject.Find("Canvas").transform.Find("DesktopImage").gameObject.SetActive(false);

            //change rotator scripts
            try {
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripX").GetComponentInChildren<Rotator>().enabled = false;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripY").GetComponentInChildren<Rotator>().enabled = false;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripZ").GetComponentInChildren<Rotator>().enabled = false;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripX").GetComponentInChildren<MobileRotator>().enabled = true;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripY").GetComponentInChildren<MobileRotator>().enabled = true;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripZ").GetComponentInChildren<MobileRotator>().enabled = true;

                GameObject.Find("rotatorStrips").transform.Find("rotatorStripX").Find("collider").transform.localScale
                    += new Vector3(0, SCALEAMOUNT, 0);
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripY").Find("collider").transform.localScale
                    += new Vector3(0, SCALEAMOUNT, 0);
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripZ").Find("collider").transform.localScale
                    += new Vector3(0, SCALEAMOUNT, 0);
                if (fpsTestFailed) {
                    //change models to their respective low poly versions
                    foreach (var r in rotators) {
                        r.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("arrowCW_lowpoly");
                    }

                    foreach (var l in locks) {
                        l.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("lock_lowpoly");
                    }


                    GameObject.Find("rotatorStrips").transform.Find("rotatorStripX").GetComponent<MeshFilter>().sharedMesh =
                        Resources.Load<Mesh>("rotatorStrip_lowpoly");
                    GameObject.Find("rotatorStrips").transform.Find("rotatorStripY").GetComponent<MeshFilter>().sharedMesh =
                        Resources.Load<Mesh>("rotatorStrip_lowpoly");
                    GameObject.Find("rotatorStrips").transform.Find("rotatorStripZ").GetComponent<MeshFilter>().sharedMesh =
                        Resources.Load<Mesh>("rotatorStrip_lowpoly");

                    ReplaceMeshes(GameObject.FindWithTag("thing"));
                }
            }
            catch (NullReferenceException e) {
            }
        }
        else {
            GameObject.Find("Canvas").transform.Find("DesktopImage").GetComponent<Image>().enabled = true;
        }

        rotating = false;
        moving = false;
        teleporting = false;
        fallingLock = false;
        rotatorClicked = false;
        rotatingSpoke = false;
        rotatingColliders = new List<GameObject>();

        keyDownTime = 0;
        keyPressed = false;

        ClearPedestal();

        if (DEBUG)
            lastrotations.Clear();

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

    //shows fps
    void OnGUI() {
        if (Debug.isDebugBuild && DEBUG) {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }

    void Update() {
        if (Debug.isDebugBuild && DEBUG)
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (SceneManager.GetActiveScene().name == "splashScreen" && !fpstested && Time.time >= 1f)
        {
            fpstested = true;
            float fps = 1.0f / Time.deltaTime;
            if (fps < 40f)
            {
                fpsTestFailed = true;
                directionalLight.GetComponent<Light>().shadows = LightShadows.Hard;
                int index = -1;
                for (int i = 0; i < QualitySettings.names.Length; i++)
                {
                    if (QualitySettings.names[i] == "Shadows")
                        index = i;
                }
                if(index != -1)
                    QualitySettings.SetQualityLevel(index, true);

                if (Application.isMobilePlatform)
                    ReplaceMeshes(titleLevel);
            }
        }
    }

    void LateUpdate() {
        //wait a bit before you can rotate with keys again
        if (keyPressed) {
            keyDownTime += Time.deltaTime;
            if (keyDownTime > MINKEYDOWNTIME) {
                keyPressed = false;
                keyDownTime = 0;
            }
        }

        if (DEBUG) {
            if (!rotating && !moving && !teleporting && rotatingColliders.Count == 0) {
//                Solve();
            }
        }

        //fade in background audio
        if (gameObject.GetComponents<AudioSource>()[0].volume < 0.5f && splashScreenDone && !doneFadingIn) {
            gameObject.GetComponents<AudioSource>()[0].volume += Time.deltaTime / 10;
        }
        else if(gameObject.GetComponents<AudioSource>()[0].volume >= 0.5f)
            doneFadingIn = true;

        //crossfade background audio
        if (changeBackgroundMusic) {
            StartCoroutine(nameof(ChangeBackgroundMusic));
        }

        //reset scene
        if (Input.GetKeyUp(KeyCode.R) && ((SceneManager.GetActiveScene().name.StartsWith("level") || SceneManager.GetActiveScene().name == "bonus") 
            && !tutorialLock || DEBUG)) {
            StartCoroutine(Reset());
        }

        //DELETE THIS EVENTUALLY
        // if (Input.GetKeyUp(KeyCode.L)) {
        //     GameOver();
        // }
        // if (Input.GetKeyUp(KeyCode.P)) {
        //     StartCoroutine(ScreenShake());
        //     gameObject.GetComponents<AudioSource>()[2].Play();
        // }
        //
        // if (Input.GetKeyUp(KeyCode.K)) {
        //     if (currentScene > 1)
        //         currentScene -= 2;
        //     GameOver();
        // }

        if (DEBUG) {
            if (Input.GetKeyUp(KeyCode.Y)) {
                GameObject.FindWithTag("thing").transform.rotation = lastrotations.Pop();
            }
        }

        if (gameOver && !isLoadingNextLevel && !gameOverWorking) {
            GameOver();
        }
    }

    void FixedUpdate() {
        if (cubes != null) {
            //turn on physics for all cubes in scene and add a small amount of velocity and torque
            if (cubeCount == cubes.Length && cubes.Length > 0 && !gameOver) {
                foreach (var cube in cubes) {
                    cube.GetComponent<Rigidbody>().useGravity = true;
                    cube.GetComponent<UnityEngine.Collider>().isTrigger = false;
                    cube.GetComponent<Rigidbody>().AddForce(new Vector3(
                        Random.Range(0, 5), Random.Range(5, 10), Random.Range(0, 5)), ForceMode.VelocityChange);
                    cube.GetComponent<Rigidbody>().AddTorque(new Vector3(
                        Random.Range(10, 20), Random.Range(10, 20), Random.Range(10, 20)), ForceMode.Impulse);

                    foreach (Transform child in cube.transform) {
                        child.gameObject.SetActive(true);
                    }
                }

                gameOver = true;
            }
        }
    }

    //for hard mode
    public static void CountDownRotations() {
        numberRotations++;
        int n = maxRotations - numberRotations;
        GameObject.Find("Canvas").transform.Find("HardModePanel").GetComponentInChildren<TextMeshProUGUI>().text = "" + n;

        if (n == 0) {
            if (GameObject.Find("rotatorStrips").transform.Find("rotatorStripX").GetComponentInChildren<Rotator>().enabled) {
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripX").GetComponentInChildren<Rotator>().disabled = true;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripY").GetComponentInChildren<Rotator>().disabled = true;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripZ").GetComponentInChildren<Rotator>().disabled = true;
            }
            else {
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripX").GetComponentInChildren<MobileRotator>().disabled = true;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripY").GetComponentInChildren<MobileRotator>().disabled = true;
                GameObject.Find("rotatorStrips").transform.Find("rotatorStripZ").GetComponentInChildren<MobileRotator>().disabled = true;
            }
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
        locky.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(10, 20), Random.Range(10, 20), Random.Range(10, 20)), ForceMode.Impulse);

        foreach (Transform child in locky.transform) {
            child.gameObject.SetActive(true);
        }
    }

    public void ResetButton() {
        if (!tutorialLock)
            StartCoroutine(Reset());
    }

    public void BackButton() {
        if (!tutorialLock)
            StartCoroutine(LoadMainMenu());
    }

    public IEnumerator LoadMainMenu() {
        
        if (!isLoadingNextLevel && !resetting) {
            resetting = true;
            StartCoroutine(nameof(MoveOutButtons));
            if (Application.isMobilePlatform && freeRotation == 1)
                StartCoroutine(nameof(MoveOutFromTheRight));
            StartCoroutine(nameof(ScreenWipeIn), false);
            while (!wipingIn) {
                yield return null;
            }

            GameObject.FindWithTag("curved").gameObject.GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("Canvas").transform.Find("HardModePanel").gameObject.SetActive(false);
            isLoadingNextLevel = true;
            gameOver = true;
            shownTutorial = false;

            SceneManager.LoadScene("mainmenu");
        }
    }

    public IEnumerator Reset() {
        if (!isLoadingNextLevel && !resetting) {
            resetting = true;
            StartCoroutine(nameof(ScreenWipeIn), false);
            while (!wipingIn) {
                yield return null;
            }

            Scene loadedLevel = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadedLevel.buildIndex);
        }
    }

    void GameOver() {

        gameOverWorking = true;
        
        if (hardmode == 1) {
            PlayerPrefs.SetInt("level" + currentScene, 1);
            PlayerPrefs.Save();
        }
        
        shownTutorial = false;
        isLoadingNextLevel = true;
        //bonus level
        if(currentScene!=99)
            currentScene++;

        if (maxScene < currentScene && currentScene < 99) {
            maxScene = currentScene;
        }

        string scene = "level" + currentScene + "_final";
        if (DEBUG)
            StartCoroutine(nameof(LoadNextSceneAsync), "test");
        else {
            Save();
            GameObject.FindWithTag("curved").gameObject.GetComponent<MeshRenderer>().enabled = false;
            GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripX").transform.GetChild(0).gameObject
                .GetComponent<LineRenderer>().positionCount = 0;
            GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripY").transform.GetChild(0).gameObject
                .GetComponent<LineRenderer>().positionCount = 0;
            GameObject.FindWithTag("rotatorStrips").transform.Find("rotatorStripZ").transform.GetChild(0).gameObject
                .GetComponent<LineRenderer>().positionCount = 0;

            if (currentScene <= LEVELCOUNT) {
                StartCoroutine(nameof(ScreenWipeIn), false);
                StartCoroutine(nameof(LoadNextSceneAsync), scene);
            } else if (currentScene == 99) {
                gameCompleted = true;
                isLoadingNextLevel = false;
                if (hardmode == 1) {
                    StartCoroutine(MoveOutButtons());
                    tutorialLock = true;
                    SceneManager.LoadSceneAsync("tutorialScreen", LoadSceneMode.Additive);
                } else {
                    StartCoroutine(LoadMainMenu());
                }
            } else {
                if (!gameCompleted) {
                    currentScene--;
                    isLoadingNextLevel = false;
                    gameCompleted = true;
                    PlayerPrefs.SetInt("gameCompleted", 1);
                    PlayerPrefs.SetInt("changedBackground", 1);
                    PlayerPrefs.Save();
                    Save();
                    StartCoroutine(MoveOutButtons());
                    StartCoroutine(ScreenShake());
                    gameObject.GetComponents<AudioSource>()[2].Play();
                    tutorialLock = true;
                } else {
                    isLoadingNextLevel = false;
                    StartCoroutine(LoadMainMenu());
                }
            }
        }
    }

    IEnumerator AsyncLoad(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

        IEnumerator LoadNextSceneAsync(string scene) {
        if (currentScene > 1) {
//            yield return new WaitForSeconds(2.1f);
            while (!wipingIn) {
                yield return null;
            }

            GetComponent<AudioSource>().PlayOneShot(transition);
//            yield return new WaitForSeconds(0.4f);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }

    private void ClearPedestal() {
        //destroy all things lying on the pedestal
        foreach (Transform child in GameObject.Find("Pedestal").transform) {
            Destroy(child.gameObject);
        }
    }

    IEnumerator MoveInButtons() {
        float step = 0;
        float y = mobileImage.GetComponent<RectTransform>().anchoredPosition.y;
        while (mobileImage.GetComponent<RectTransform>().anchoredPosition.y < 0) {
            mobileImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Lerp(y, 0, step));
            step += Time.deltaTime * 2f;
            yield return null;
        }

        mobileImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    //for moving mobileimageY
    IEnumerator MoveInFromTheRight() {
        float step = 0;
        float x = mobileImageY.GetComponent<RectTransform>().anchoredPosition.x;
        while (mobileImageY.GetComponent<RectTransform>().anchoredPosition.x > -mobileImageY.GetComponent<RectTransform>().rect.width / 2) {
            mobileImageY.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(x,
                -mobileImageY.GetComponent<RectTransform>().rect.width / 2, step), mobileImageY.GetComponent<RectTransform>().anchoredPosition.y);
            step += Time.deltaTime * 2f;
            yield return null;
        }

        mobileImageY.GetComponent<RectTransform>().anchoredPosition = new Vector2(-mobileImageY.GetComponent<RectTransform>().rect.width / 2,
            mobileImageY.GetComponent<RectTransform>().anchoredPosition.y);
    }


    IEnumerator MoveOutButtons() {
        float step = 0;
        float y = -mobileImage.GetComponent<RectTransform>().rect.height;
        while (mobileImage.GetComponent<RectTransform>().anchoredPosition.y > y) {
            mobileImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Mathf.Lerp(0, y, step));
            step += Time.deltaTime * 2f;
            yield return null;
        }

        mobileImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
    }

    IEnumerator MoveOutFromTheRight() {
        float step = 0;
        float x = mobileImageY.GetComponent<RectTransform>().rect.width / 2;
        while (mobileImage.GetComponent<RectTransform>().anchoredPosition.x < x) {
            mobileImageY.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(-x, x, step),
                mobileImageY.GetComponent<RectTransform>().anchoredPosition.y);
            step += Time.deltaTime * 2f;
            yield return null;
        }

        mobileImageY.GetComponent<RectTransform>().anchoredPosition = new Vector2(x,
            mobileImageY.GetComponent<RectTransform>().anchoredPosition.y);
    }

    IEnumerator ScreenWipeIn(bool levelbutton = false) {
        //wait a bit for cubes to fall
        if (gameOver)
            yield return new WaitForSeconds(1.6f);
        wiping = true;
        float timeStep = 0;
        while (screenWipe.GetComponent<Image>().fillAmount < 1) {
            screenWipe.GetComponent<Image>().fillAmount = Mathf.Lerp(0, 1, timeStep);
            directionalLight.GetComponent<Light>().shadowStrength = 1 - screenWipe.GetComponent<Image>().fillAmount;
            timeStep += Time.deltaTime;
            yield return null;
        }

        //looks weird otherwise
        GameObject.Find("Canvas").transform.Find("HardModePanel").gameObject.SetActive(false);
        screenWipe.GetComponent<Image>().fillAmount = 0.8f;
        directionalLight.GetComponent<Light>().shadowStrength = 0;
        wipingIn = true;
        wiping = false;
        ClearPedestal();

        //load scene when level or start button was pressed
        if (levelbutton) {
            bool levelLoaded = false;
            for(int i =0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == "bonus" || SceneManager.GetSceneAt(i).name == "level" + currentScene + "_final")
                    levelLoaded = true;
            }
            //sometimes the level gets loaded twice for some reason
            if (!levelLoaded)
            {
                if (currentScene == 99)
                    StartCoroutine(nameof(AsyncLoad), "bonus");
                else
                    StartCoroutine(nameof(AsyncLoad), "level" + currentScene + "_final");
            }
        }
    }

    IEnumerator ScreenWipeOut() {
        wipingIn = false;
        wiping = true;
        float timeStep = 0;
        while (screenWipe.GetComponent<Image>().fillAmount > 0) {
            screenWipe.GetComponent<Image>().fillAmount = Mathf.Lerp(1, 0, timeStep);
            directionalLight.GetComponent<Light>().shadowStrength = 1 - screenWipe.GetComponent<Image>().fillAmount;
            timeStep += Time.deltaTime;
            yield return null;
        }

        screenWipe.GetComponent<Image>().fillAmount = 0;
        directionalLight.GetComponent<Light>().shadowStrength = 1;
        wiping = false;
    }

    //used for final level background transition
    public IEnumerator ScreenShake() {
        float timeCount = 0;
        Quaternion originalRotation = skyboxCamera.transform.rotation;
        while (timeCount < 2f) {
            Vector3 random = Random.insideUnitSphere;
            skyboxCamera.transform.Rotate(random,0.2f);
            timeCount += Time.deltaTime;
            yield return null;
            skyboxCamera.transform.Rotate(-random,0.2f);
        }

        skyboxCamera.transform.rotation = originalRotation;
        StartCoroutine(ChangeBackground());
    }
    
    //used for final level background transition after screenshake
    public IEnumerator ChangeBackground() {
        reflectionImage.SetActive(false);
        GameObject.Find("ReflectionCamera").GetComponent<Camera>().enabled = false;
        
        float timeCount = 0;
        Quaternion tempFrom = skyboxCamera.transform.rotation;
        Quaternion tempTo = Quaternion.AngleAxis(90f, skyboxCamera.transform.right) * tempFrom;

        Color skyColor = RenderSettings.ambientSkyColor;
        Color groundColor = RenderSettings.ambientGroundColor;

        bool changedMusic = false;

        while (skyboxCamera.transform.rotation != tempTo) {
            skyboxCamera.transform.rotation = Quaternion.Slerp(tempFrom, tempTo, timeCount);
            RenderSettings.ambientSkyColor = Color.Lerp(skyColor, newSkyColor, timeCount);
            RenderSettings.ambientGroundColor = Color.Lerp(groundColor, newGroundColor, timeCount);
            if (timeCount > 0.5f && !changedMusic) {
                StartCoroutine(ChangeBackgroundMusic());
                changedMusic = true;
            }
            timeCount += Time.deltaTime/2;
            yield return null;
        }

        changedBackground = true;
        SceneManager.LoadSceneAsync("tutorialScreen", LoadSceneMode.Additive);
    }

    public static void ChangeBackgroundOption() {
        if (!changedBackground) {
            PlayerPrefs.SetInt("changedBackground",1);
            PlayerPrefs.Save();
            instance.reflectionImage.SetActive(false);
            GameObject.Find("ReflectionCamera").GetComponent<Camera>().enabled = false;
            RenderSettings.ambientSkyColor = newSkyColor;
            RenderSettings.ambientGroundColor = newGroundColor;
            instance.skyboxCamera.transform.Rotate(90f,0,0);
        }
        else {
            PlayerPrefs.SetInt("changedBackground",-1);
            PlayerPrefs.Save();
            GameObject.Find("ReflectionCamera").GetComponent<Camera>().enabled = true;
            instance.reflectionImage.SetActive(true);
            RenderSettings.ambientSkyColor = oldSkyColor;
            RenderSettings.ambientGroundColor = oldGroundColor;
            instance.skyboxCamera.transform.Rotate(-90f,0,0);
        }
        instance.StopCoroutine(nameof(ChangeBackgroundMusic));
        instance.StartCoroutine(nameof(ChangeBackgroundMusic),changedBackground);
        changedBackground = !changedBackground;
    }

    public IEnumerator ChangeBackgroundMusic(bool reverse = false) {
        doneFadingIn = true;
        changeBackgroundMusic = false;
        bool changing = true;
        while (changing) {
            if (!reverse) {
                if(!gameObject.GetComponents<AudioSource>()[1].isPlaying)
                    gameObject.GetComponents<AudioSource>()[1].Play();
                if (gameObject.GetComponents<AudioSource>()[0].volume >= 0)
                    gameObject.GetComponents<AudioSource>()[0].volume -= Time.deltaTime / 5;
                if (gameObject.GetComponents<AudioSource>()[1].volume < 0.5f)
                    gameObject.GetComponents<AudioSource>()[1].volume += Time.deltaTime / 5;
                if (gameObject.GetComponents<AudioSource>()[0].volume <= 0 && gameObject.GetComponents<AudioSource>()[1].volume >= 0.5f) {
                    changeBackgroundMusic = false;
                    changing = false;
                    gameObject.GetComponents<AudioSource>()[0].Stop();
                }
            }
            else {
                if(!gameObject.GetComponents<AudioSource>()[0].isPlaying)
                    gameObject.GetComponents<AudioSource>()[0].Play();
                if (gameObject.GetComponents<AudioSource>()[1].volume >= 0)
                    gameObject.GetComponents<AudioSource>()[1].volume -= Time.deltaTime / 5;
                if (gameObject.GetComponents<AudioSource>()[0].volume < 0.5f)
                    gameObject.GetComponents<AudioSource>()[0].volume += Time.deltaTime / 5;
                if (gameObject.GetComponents<AudioSource>()[1].volume <= 0 && gameObject.GetComponents<AudioSource>()[0].volume >= 0.5f) {
                    changeBackgroundMusic = false;
                    changing = false;
                    gameObject.GetComponents<AudioSource>()[1].Stop();
                }
            }
            yield return null;
        }
    }

    public static bool Compare(Vector3 lhs, Vector3 rhs) {
        return Vector3.SqrMagnitude(lhs - rhs) < EPSILON;
    }

    //replace all spoke meshes in thing, where appropriate
    private static void ReplaceMeshes(GameObject g) {
        if (fpsTestFailed) {

            if (g.GetComponent<MeshFilter>() == null && g.transform.childCount == 0)
                return;

            if (g.GetComponent<MeshFilter>() != null) {
                string bla = g.GetComponent<MeshFilter>().sharedMesh.name;

                if (bla == "Sphere")
                    g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("sphere_lowpoly");

                int last = bla.LastIndexOf("_", StringComparison.Ordinal);
                if (last != -1) {
                    string name = bla.Substring(0, last);
                    switch (name) {
                        case "spoke_corner_flat": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_corner_flat_lowpoly");
                            break;
                        }
                        case "spoke_corner_flat_small": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_corner_flat_small_lowpoly");
                            break;
                        }
                        case "spoke_corner": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_corner_lowpoly");
                            break;
                        }
                        case "spoke_end": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_end_lowpoly");
                            break;
                        }
                        case "spoke_end_small": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_end_small_lowpoly");
                            break;
                        }
                        case "spoke": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_lowpoly");
                            break;
                        }
                        case "spoke_threeway_flat": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_threeway_flat_lowpoly");
                            break;
                        }
                        case "spoke_threeway_flat_small": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_threeway_flat_small_lowpoly");
                            break;
                        }
                        case "spoke_threeway_small": {
                            g.GetComponent<MeshFilter>().sharedMesh = Resources.Load<Mesh>("spoke_threeway_small_lowpoly");
                            break;
                        }
                    }
                }
            }

            if (g.transform.childCount != 0) {
                foreach (Transform child in g.transform) {
                    ReplaceMeshes(child.gameObject);
                }
            }
        }
    }

    private static void Save() {
        PlayerPrefs.SetInt("currentScene", currentScene);
        if (maxScene > LEVELCOUNT)
            maxScene = LEVELCOUNT;
        PlayerPrefs.SetInt("maxScene", maxScene);
        PlayerPrefs.Save();
    }

    private static void Load() {
        currentScene = PlayerPrefs.GetInt("currentScene", 1);
        //bonus level
        if (currentScene == 100)
            currentScene = 99;
        if (currentScene > LEVELCOUNT && currentScene != 99)
            currentScene = LEVELCOUNT;
        maxScene = PlayerPrefs.GetInt("maxScene", 1);
        if (maxScene >= 99)
            maxScene = LEVELCOUNT;
    }

    private void Solve() {
        if (gameOver && !isLoadingNextLevel) {
            string finishedinputs = "";
            foreach (var entry in inputs) {
                finishedinputs += entry + ", ";
            }

            if ((solved.Count > inputs.Count && inputs.Count > 0) || (solved.Count == 0 && inputs.Count > 0)) {
                finishedinputs += inputs.Count;
                solved.Clear();
                solved.AddRange(inputs);
                Debug.Log(finishedinputs);
                ScreenCapture.CaptureScreenshot("/home/tino/" + solved.Count + " Moves " + Time.deltaTime);
            }

            inputs.Clear();
//                StartCoroutine(Reset());
        }

        solveTimeout += Time.deltaTime;
        if (solveTimeout > 0.5f) {
            solveTimeout = 0;
            int random = Random.Range(1, 7);

            if (!(hitLock && random == lastRotation)) {
                if (!gameOver) {
                    switch (random) {
                        case 1: {
                            GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                                    .lastAngle =
                                90f;
                            GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                                    .rotateRoutine =
                                GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                                    .StartCoroutine("Rotate", false);
                            lastRotatorStrip = GameObject.FindWithTag("rotatorStripX").transform.GetChild(0)
                                .GetComponent<Rotator>();
                            solveTimeout = 0;
                            inputs.Add("a");
                            break;
                        }
                        case 2: {
                            GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                                    .lastAngle =
                                -90f;
                            GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                                    .rotateRoutine =
                                GameObject.FindWithTag("rotatorStripX").transform.GetChild(0).GetComponent<Rotator>()
                                    .StartCoroutine("Rotate", false);
                            lastRotatorStrip = GameObject.FindWithTag("rotatorStripX").transform.GetChild(0)
                                .GetComponent<Rotator>();
                            solveTimeout = 0;
                            inputs.Add("d");
                            break;
                        }
                        case 3: {
                            GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                                    .lastAngle =
                                90f;
                            GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                                    .rotateRoutine =
                                GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                                    .StartCoroutine("Rotate", false);
                            lastRotatorStrip = GameObject.FindWithTag("rotatorStripY").transform.GetChild(0)
                                .GetComponent<Rotator>();
                            solveTimeout = 0;
                            inputs.Add("w");
                            break;
                        }
                        case 4: {
                            GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                                    .lastAngle =
                                -90f;
                            GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                                    .rotateRoutine =
                                GameObject.FindWithTag("rotatorStripY").transform.GetChild(0).GetComponent<Rotator>()
                                    .StartCoroutine("Rotate", false);
                            lastRotatorStrip = GameObject.FindWithTag("rotatorStripY").transform.GetChild(0)
                                .GetComponent<Rotator>();
                            solveTimeout = 0;
                            inputs.Add("s");
                            break;
                        }
                        case 5: {
                            GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                                    .lastAngle =
                                90f;
                            GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                                    .rotateRoutine =
                                GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                                    .StartCoroutine("Rotate", false);
                            lastRotatorStrip = GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0)
                                .GetComponent<Rotator>();
                            solveTimeout = 0;
                            inputs.Add("e");
                            break;
                        }
                        case 6: {
                            GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                                    .lastAngle =
                                -90f;
                            GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                                    .rotateRoutine =
                                GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0).GetComponent<Rotator>()
                                    .StartCoroutine("Rotate", false);
                            lastRotatorStrip = GameObject.FindWithTag("rotatorStripZ").transform.GetChild(0)
                                .GetComponent<Rotator>();
                            solveTimeout = 0;
                            inputs.Add("q");
                            break;
                        }
                    }

                    hitLock = false;
                    lastRotation = random;
                }
            }

            if (inputs.Count > solved.Count && solved.Count > 0 ||
                inputs.Count > GameObject.FindWithTag("thing").GetComponent<RotationCount>().rotationCount) {
                inputs.Clear();
                StartCoroutine(Reset());
            }
        }
    }
}