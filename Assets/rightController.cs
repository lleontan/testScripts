using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightController : Controller {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	}
	protected override bool ExecuteControls(bool triggerUp, bool triggerDown, bool triggerPress,
		bool GripUp, Vector2 touchpad, bool menuButtonUp){
		//Will not be executed if menu is gazed

		//Touchpad states, each arm of the cross is an action.
		if (touchpad.x > .3 && touchpad.y <.6f && touchpad.y > -.6f) {
			//Right
			
		} else if (touchpad.x < .3 && touchpad.y <.6f && touchpad.y > -.6f) {
			//Left
		}
		if (touchpad.y > .3  && touchpad.x <.6f && touchpad.x > -.6f) {
			//Top
		} else if (touchpad.y < .3 && touchpad.x <.6f && touchpad.x > -.6f) {
			//Bottom
		}
		return base.ExecuteControls(triggerUp, triggerDown, triggerPress,
			GripUp, touchpad, menuButtonUp);	//Handles object grabbing;
	}
	protected override void AlwaysExecute(bool triggerUp,
		bool triggerDown, bool triggerPress, bool GripUp, Vector2 touchpad){
		//Will always be executed even if a menu is gazed.
	}
}
