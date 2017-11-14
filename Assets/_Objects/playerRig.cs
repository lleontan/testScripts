using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mob))]
public class playerRig : MonoBehaviour {
	// Attach to CameraRig's parent transform.
	public Transform hmd;
	protected Mob mob;
	void Start(){
		DontDestroyOnLoad(this.transform.gameObject);
		this.mob = this.GetComponent<Mob>();
		if (mob == null) {
			this.hmd = this.transform.Find ("Camera (head)");
		}
	}

	void Update(){
		/*
			Realign rotation if x or z is not 0.
		*/
		Vector3 newAngle= new Vector3 (0, this.transform.rotation.eulerAngles.y, 0);
		this.transform.eulerAngles = newAngle;
	}
	public Mob getMob(){
		return this.mob;
	}
	public void HMDmove(Vector2 direction){
		if (direction.magnitude > .05f) {
			//Vector3 withoutY = new Vector3 (direction.y,0,direction.x);
			Vector3 hmdForward = new Vector3 (hmd.forward.x, 0, hmd.forward.z);
			Vector3 hmdRight = new Vector3 (hmd.right.x, 0, hmd.right.z);

			Vector3 addForce = hmdRight * direction.x + direction.y * hmdForward;
			this.mob.Move2d (addForce);
		} else {
			mob.xZVelocityReset ();
		}
	}
}
