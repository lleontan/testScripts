using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detachAttachmentsOnTriggerClick : HandleEvents {
	//On TriggerClick detach every attachment in each slot;
	public AttachmentSlot[] slots;
	public override void onPickup(Hand controller){}
	public override void onDrop (Hand controller){}
	public override void onTriggerClickUp (Controller controller){}
	public override void onTriggerPress (Controller controller){}
	public override void onTriggerClickDown (Controller controller){
		foreach (AttachmentSlot slot in slots) {
			slot.AttemptDetach ();
		}
	}
	public override bool onGripUp(Controller controller){
		return true;
	}
	public override void touchPadPress (Controller controller, bool triggerUp,
		bool triggerDown, bool triggerPress, bool gripUp, Vector2 touchpad){

	}
}
