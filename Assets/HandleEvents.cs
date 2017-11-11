using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HandleEvents : MonoBehaviour {
	public abstract void onPickup(Hand controller);
	public abstract void onDrop (Hand controller);
	public abstract void onTriggerClickUp (Controller controller);					//When this handles controller is clicked
	public abstract void onTriggerPress (Controller controller);					//When this handles controller is clicked
	public abstract void onTriggerClickDown (Controller controller);					//When this handles controller is clicked
	public abstract bool onGripUp(Controller controller);							//WARNING: true for release handle on gripup, false for no release, ALWAYS DEFAULT TO TRUE;
	public abstract void touchPadPress (Controller controller, bool triggerUp,
		bool triggerDown, bool triggerPress, bool gripUp, Vector2 touchpad);

}
