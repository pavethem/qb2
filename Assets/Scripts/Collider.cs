using UnityEngine;

public class Collider : MonoBehaviour {
	
	private float stayTime;
	private bool collided;
	private const float STAYTIME_THRESHOLD = 0.2f;

	void OnTriggerStay(UnityEngine.Collider other) {
		
		if (other.gameObject.CompareTag("bub") && !gameObject.CompareTag("key") && !gameObject.CompareTag("lock")) {
			if (stayTime > STAYTIME_THRESHOLD && !collided) {
				GameController.cubeCount++;
				collided = true;
			}
			else
				stayTime += Time.fixedDeltaTime;
			Behaviour halo = (Behaviour) other.GetComponent("Halo");
			halo.enabled = true;
		}
		
	}

	void OnTriggerExit(UnityEngine.Collider other) 
	{
		if (other.gameObject.CompareTag ("bub"))
		{
			if(collided)
				GameController.cubeCount--;
			Behaviour halo = (Behaviour) other.GetComponent("Halo");
			halo.enabled = false;
			stayTime = 0f;
			collided = false;
		}
	}
}
