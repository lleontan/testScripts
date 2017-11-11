using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour {
	//Destroys this gameobject after a delay
	public float lifetime=6f;
	public bool timerActive = true;
	// Update is called once per frame
	void Update () {
		if (timerActive) {
			if (lifetime < 0) {
				Destroy (this.gameObject);
			}
			lifetime -= Time.deltaTime;
		}
	}
}
