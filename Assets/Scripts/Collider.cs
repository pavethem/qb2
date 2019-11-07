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
//			Behaviour halo = (Behaviour) other.GetComponent("Halo");
//			halo.enabled = true;
			other.gameObject.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
		}
		
	}

	void OnTriggerExit(UnityEngine.Collider other) 
	{
		if (other.gameObject.CompareTag ("bub"))
		{
			if(collided)
				GameController.cubeCount--;
//			Behaviour halo = (Behaviour) other.GetComponent("Halo");
//			halo.enabled = false;
			other.gameObject.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
			stayTime = 0f;
			collided = false;
		}
	}
}
