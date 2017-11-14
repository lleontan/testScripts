using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleporterGrip : HandleEvents {
	//Onclick sets the replacement instance to the raycastPoint.
	//Onclick with a replacment instance sets the controllers parent position to the replacmentInstances position.
	//Grip cancels the replacement instance if it exists.
	//Grip without a replacement instance unequips this item into the players inventory.

	public bool canTeleport = true;
	public float minTeleportDistance=.5f;
	public float maxTeleportDistance=10f;
	public GameObject replacementToken;			//Replacement Avatar prefab from resources folder
	protected GameObject replacementInstance;	//In game instance
	public float addHeightOnTeleport = .3f;

	public Transform teleportRayOrigin;			//Where the ray originates from.
	public GameObject inventory;				//The inventory script to hold this objects instance, Change from gameobject to inventory when inventory system is implemented.
	public ObjectHandle handle;
	private Transform teleportSubject;			//What is being teleported, probably the player;
	public laserSight laserSight;				//Toggle on activation; if null don't do anything
	private int layer;
	void Awake(){
		layer = 1;
		if (this.handle == null) {
			this.handle = this.GetComponent<ObjectHandle> ();
		}
	}
	private void setReplacementInstancePosition(){
		//Sets the position of the replacementInstance to the position of the controller Raycast.
		if (this.replacementInstance) {
			RaycastHit hit;
			if (Physics.Raycast (this.teleportRayOrigin.position,
				this.teleportRayOrigin.forward, out hit, this.maxTeleportDistance,layer) &&hit.collider.gameObject.tag!="noTeleports") {
				if (hit.distance > this.minTeleportDistance) {
					this.replacementInstance.transform.position = hit.point + hit.normal * this.addHeightOnTeleport;
				} else {
					ceaseReplacement ();
				}
			}
		}
	}
	void Update(){
		this.setReplacementInstancePosition ();
	}
	public override void onPickup(Hand controller){
		this.teleportSubject = controller.transform.root;
		ceaseReplacement ();
	}
	public override void onDrop (Hand controller){
		ceaseReplacement ();
	}
	public override void onTriggerClickUp (Controller controller){
		if (this.replacementInstance) {
			this.teleportSubject.transform.position = this.replacementInstance.transform.position;
		} else {
			RaycastHit hit;
			if (Physics.Raycast (this.teleportRayOrigin.position,
				this.teleportRayOrigin.forward, out hit, this.maxTeleportDistance,layer) && hit.collider.gameObject.tag!="noTeleports") {
				if (hit.distance > this.minTeleportDistance) {
					this.replacementInstance = Instantiate (this.replacementToken, hit.point + hit.normal * this.addHeightOnTeleport,replacementToken.transform.rotation);
					if (this.laserSight) {
						laserSight.maxDistance = this.maxTeleportDistance;
						this.laserSight.setActive ();
					}
				} else {
					ceaseReplacement ();
				}
			}
		}
	}
	public void ceaseReplacement(){
		if (this.replacementInstance) {
			GameObject.Destroy (this.replacementInstance);
			if (this.laserSight) {
				this.laserSight.setInactive ();
			}
		}
	}
	public override void onTriggerPress (Controller controller){}
	public override void onTriggerClickDown (Controller controller){
	}
	public override bool onGripUp(Controller controller){
		if (this.replacementInstance) {
			ceaseReplacement ();
			return false;
		}
		return true;
	}
	public override void touchPadPress (Controller controller, bool triggerUp,
		bool triggerDown, bool triggerPress, bool gripUp, Vector2 touchpad){

	}
}
