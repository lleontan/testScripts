using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHover : MonoBehaviour {
	// WARNING.
	// THE PARENTRIG SHOULD NEVER BE ROTATED IN THE XZ DIMENSIONS.

	public Transform head;
	public float headHoverDistance=.65f;		//head origin - body origin
	public float hoverHeight= .5f;				//HoverHeight above the ground
	//Makes the players body hover directly underneath its head.
	protected Rigidbody rootRigid;
	void Start(){
		rootRigid = this.transform.root.gameObject.GetComponent<Rigidbody> ();
	}
	void Update(){
		RaycastHit hit;
		if (Physics.Raycast (this.transform.position, Vector3.down, out hit, hoverHeight)) {
			//If there is ground underneath the player have the body hover over it.
			/*Vector3 newPosition = new Vector3 (
				this.transform.root.position.x,
				hit.point.y + hoverHeight * 1.05f,
				this.transform.root.position.z);
			this.transform.root.position = newPosition;*/
			rootRigid.useGravity = false;
		} else {
			this.transform.position = head.position - Vector3.up * this.headHoverDistance;
			rootRigid.useGravity = true;
		}
	}
	void OnCollisionEnter(Collision col){
		if (col.gameObject == head.gameObject) {
			this.GetComponent<Renderer> ().enabled = false;
			Physics.IgnoreCollision(col.collider, this.GetComponent<Collider>());
		}
	}
	void OnCollisionExit(Collision col){
		if (col.gameObject == head.gameObject) {
			this.GetComponent<Renderer> ().enabled = true;
		}
	}
}
