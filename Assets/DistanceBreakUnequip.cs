using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Holdable))]

public class DistanceBreakUnequip : ParentDistanceBreakEvent {
	// When the connection is broken to the parent, Equips the selected touchHold as a Holdable in the hand holding the visual obj.
	// Attach to visualObject. 
	// Requires this object to have a Rigidbody and Holdable.
	private Holdable thisHoldable;
	public Attachment attachment;					//Not set by default, must be set.
	public override void OnDistanceBreak(touchHold held){
		Hand hand = held.getLastController ();
		if (held.CheckHeld ()) {
			held.releaseControllerGrip ();
		}

		if (attachment) {
			attachment.AttemptDetach ();
		}
		if (held.CheckHeld() && thisHoldable.isHeld()) {
			thisHoldable.GetDefaultHandle ().holdableStart (hand.transform, hand);
		}
		Rigidbody visualRigid = held.gameObject.GetComponent<Rigidbody> ();
		visualRigid.useGravity = true;
		visualRigid.isKinematic = false;
	}
	void Start () {
		thisHoldable = this.GetComponent<Holdable> ();
	}

}
