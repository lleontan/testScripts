using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserSight : OnAttach {
	//Draws a laserSight in game.

	private LineRenderer line;
	public float maxDistance=20f;
	// Use this for initialization
	private Attachment thisAttachment;
	void Start () {
		this.line = this.GetComponent<LineRenderer> ();
		thisAttachment = this.GetComponent<Attachment> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//if (!this.thisAttachment.isAttached ()) {
			UpdateLaserSight ();
		//}
	}
	public override void onLateUpdate ()
	{
		//this.UpdateLaserSight ();
	}
	public void UpdateLaserSight(){
		RaycastHit hit;
		Vector3 newPoint;
		if (Physics.Raycast (this.transform.position,
			this.transform.forward, out hit, this.maxDistance)) {
			newPoint = hit.point;
		} else {
			newPoint = this.transform.forward * this.maxDistance + this.transform.position;
		}
		Vector3[] newPositions={this.transform.position,newPoint};
		line.SetPositions (newPositions);
	}
	public override void onAttach(Attachment attachment){
		this.line.enabled = true;
	}
	public override void onDetach(){
		//this.line.enabled = false;
	}
	public override void onTriggerClickUp(Controller controller){
		Debug.Log ("laserSight toggle clickup");
		this.activationToggle();
	}
	public void activationToggle(){
		this.line.enabled = !this.line.enabled;
	}
	public void setActive(){
		this.line.enabled =true;
	}
	public void setInactive(){
		this.line.enabled =false;

	}
}
