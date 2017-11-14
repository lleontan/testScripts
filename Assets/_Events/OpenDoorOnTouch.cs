using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorOnTouch : MonoBehaviour {
	//When the controller touches a trigger of this object.
	//Open the specified doors.


	public Door[] doors;
	public bool pushOnStart=false;
	public Animator onPushAnimation;

	public void OnPush(){
		this.onPushAnimation.SetTrigger ("pushButton");
		foreach (Door door in doors) {
			door.ToggleOpenState ();
		}
	}
	void Start () {
		if (pushOnStart) {
			this.OnPush ();
		}
	}


	public void OnTriggerEnter(Collider col){
		this.OnPush ();
	}
	public void OnCollisionEnter(Collision col){
		Rigidbody rigid = col.transform.GetComponent<Rigidbody> ();
		if (rigid&&rigid.mass>.3) {
			this.OnPush ();
		}
	}

}
