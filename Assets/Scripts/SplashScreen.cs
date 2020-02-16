using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    void OnAnimationPlayed() {
        GameController.SplashScreenDone();
    }
}
