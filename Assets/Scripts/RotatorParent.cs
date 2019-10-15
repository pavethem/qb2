using UnityEngine;

public class RotatorParent : MonoBehaviour {
    //angle of the desired rotation
    internal float signedAngle;
    internal Coroutine rotateRoutine;

    public void StopRotating() {
        if(rotateRoutine != null)
            StopCoroutine(rotateRoutine);
    }
}
