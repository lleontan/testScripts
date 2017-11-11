using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leftController : Controller {
	//Requires baseParent of rig to have mob.

	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	}
	void FixedUpdate(){
		this.player.HMDmove (this.controller.GetAxis(this.touchpadId));		//Only the left controller moves the player.
	}
	protected override bool ExecuteControls(bool triggerUp, bool triggerDown, bool triggerPress,
		bool GripUp, Vector2 touchpad, bool menuButtonUp){
		//Will not be executed if menu is gazed

		return base.ExecuteControls(triggerUp, triggerDown, triggerPress,
			GripUp, touchpad, menuButtonUp);
	}
	protected override void AlwaysExecute(bool triggerUp,
		bool triggerDown, bool triggerPress,bool GripUp, Vector2 touchpad){
		//Will always be executed even if menu is gazed.
	}
}
