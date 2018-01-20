using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifterTrigger : MonoBehaviour {
	private Rigidbody rootRigid;
	public float liftingPower=1f;
	public Transform followTarget;
	public float relativeHeight=-.5f;
	private Vector3 relativeVector;
	void Start(){
		this.rootRigid = this.transform.root.GetComponent<Rigidbody> ();
		relativeVector = Vector3.up * relativeHeight;
	}
	void Update(){
		this.transform.position = followTarget.transform.position + relativeVector;
	}
	void OnTriggerStay(Collider col){
		if (this.rootRigid.velocity.y < 0) {
			//Debug.Log (col.name);
			this.rootRigid.velocity=new Vector3(rootRigid.velocity.x,0,rootRigid.velocity.z);
			this.rootRigid.useGravity = false;
			this.rootRigid.AddForce (Vector3.up*liftingPower);
		}
	}
	void OnTriggerExit(Collider col){
		this.rootRigid.useGravity = true;
	}
}
