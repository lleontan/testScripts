using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attachmentGripHandler : HandleEvents {
	//Calls the handle trigger events for the specified OnAttach script
	public OnAttach attachment;
	public void Start(){
		if (attachment == null) {
			this.attachment = this.GetComponent<OnAttach> ();
		}
	}
	public override void onPickup(Hand controller){
	}
	public override void onDrop (Hand controller){
	}
	public override void onTriggerClickUp (Controller controller){
		Debug.Log ("laserSight toggle");

		this.attachment.onTriggerClickUp (controller);
	}
	public override void onTriggerPress (Controller controller){}
	public override void onTriggerClickDown (Controller controller){
	}
	public override bool onGripUp(Controller controller){
		return true;
	}
	public override void touchPadPress (Controller controller, bool triggerUp,
		bool triggerDown, bool triggerPress, bool gripUp, Vector2 touchpad){

	}
}
