using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// On Trigger enter halts the arrows and parents it to whatever was hit.
// On Click or after x number of seconds play a deathanimation/explode/destroy self. 
public class arrowCollision : OnControllerTouch {
	private Rigidbody thisRigid;
	public float duration=30f;			//Time til self destruct
	private float currentDuration; 
	private GameObject bufferObj;
	public GameObject arrowParent;
	private bool attached=false;
	public Transform centerOfMass;
	void Start () {
		this.currentDuration = duration;
		thisRigid = this.GetComponent<Rigidbody>();
		thisRigid.centerOfMass = centerOfMass.localPosition;
	}
	
	void Update () {
		currentDuration -= Time.deltaTime;
		if (currentDuration < 0) {
			currentDuration = duration;
			OnDeath ();
		}
	}
	void OnDeath(){
		//Function plays on arrow death.
		if(bufferObj){
			Destroy (bufferObj);
		} else{
		Destroy(this.transform.gameObject);
		}
	}
	private Vector3 setScale(Transform currentTransform, Vector3 scale){
		//divides this objects scale by parents non recursively.
		return new Vector3 (currentTransform.localScale.x / scale.x, currentTransform.localScale.y / scale.y, currentTransform.localScale.z / scale.z);

	}
	void OnTriggerEnter(Collider col){
		Debug.Log ("Arrow Collision Entered");
		if(attached||col.transform.root.GetComponent<Holdable>()||col.transform.root.GetComponent<playerRig>()){
			
		}else{
			attached = true;
			thisRigid.velocity = Vector3.zero;
			Vector3 originalScale = this.transform.root.localScale;
			//!!!!
			//this.transform.localScale = this.transform.localScale / col.transform.lossyScale;
			//!!!!

			this.transform.SetParent(col.transform);
			this.transform.localScale=setScale (this.transform,col.transform.lossyScale);


			/*bufferObj=Instantiate(new GameObject(), this.transform.position,this.transform.rotation);
		this.bufferObj.transform.parent=col.transform;
		bufferObj.transform.localScale = new Vector3(1/this.transform.localScale.x,1/this.transform.localScale.y,1/this.transform.localScale.z);
		this.transform.parent=bufferObj.transform;*/
			Destroy (thisRigid);
		}
	}
	public override void onTriggerStay (Controller controller){
		if (controller.getTriggerPress()) {
			OnDeath ();
		}
	}
	public override void onTriggerEnter(Controller controller){}
	public override void onTriggerExit (Controller controller){}
	public override void onCollisionEnter(Controller controller){}
	public override void onCollisionStay (Controller controller){
		if (controller.getTriggerPress()) {
			OnDeath ();
		}
	}
	public override void onCollisionExit (Controller controller){}
}
