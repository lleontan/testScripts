using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectHandle))]
public class GunGripHandler : HandleEvents {
	//Attach to the objectHandle of the gun.
	/* Fires guns on triggerClick, Releases mag on menuButton press,*/
	public Gun gun;					//Controller of the gun itself
	public override void onPickup(Hand controller){
	}
	public override void onDrop (Hand controller){
	}
	public override void onTriggerClickUp (Controller controller){}
	public override void onTriggerPress (Controller controller){
		if (gun.fireType == Gun.FULL_AUTO) {
			gun.AttemptFire ();
		}
	}
	public override void onTriggerClickDown (Controller controller){
		bool success=gun.AttemptFire ();
		//Debug.Log ("AttemptFire attempt "+success);
	}
	public override bool onGripUp(Controller controller){		//WARNING RETURN TRUE;
		return true;
	}
	public override void touchPadPress (Controller controller, bool triggerUp,
		bool triggerDown, bool triggerPress, bool gripUp, Vector2 touchpad){
		if (controller.getTouchpadClickDown ()&&touchpad.y<-.2&&touchpad.x>-.4&&touchpad.x<.4) {
			gun.releaseMagazine ();			
		}
	}
}
