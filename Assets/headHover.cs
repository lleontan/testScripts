using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headHover : MonoBehaviour {
	//Sets the global position of the SteamVR PlayArea to slightly above the raycast impact from the players head to the ground.
	// distance to raycast is the local Y of the head transform.

	//WARNING: ATTACH TO "Camera(Eye)" not Camera(Head)

	//DEPRECIATED DO NOT USE, BAD BEHAVIOURS ABOUND IN THIS SCRIPT.
	// Use cameraRigHoverInstead

	// Use this for initialization
	protected Rigidbody rootRigid;
	void Start () {
		this.rootRigid = this.transform.root.GetComponent<Rigidbody>();
		if (this.transform.localPosition.y < .2f) {
			this.transform.localPosition += Vector3.up*.3f;
		}
	}

	// Update is called once per frame
	void Update () {
		LayerMask mask = new LayerMask ();
		mask=~(1<<LayerMask.NameToLayer("player"));
		RaycastHit hit;
		if (Physics.Raycast (this.transform.position, Vector3.down, out hit,
			this.transform.localPosition.y*1.05f, mask.value)) {
			//There is a floor.
			rootRigid.useGravity = false;
			rootRigid.transform.position = hit.point+Vector3.up*.10f;
		} else {
			rootRigid.useGravity = true;
		}
	}
}
