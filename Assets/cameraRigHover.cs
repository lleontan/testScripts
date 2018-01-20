using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRigHover : MonoBehaviour {

	public float hoverHeight= 1.9f;				//HoverHeight above the ground
	private Rigidbody rootRigid;
	private int layer;
	public Transform hoverTrans;
	public float heightMod=.8f;
	private float heightNoTele;
	void Start(){
		//layer = LayerMask.NameToLayer ("Default");
		//layer=LayerMask.NameToLayer("Default");
		layer=1;
		heightNoTele = this.hoverHeight * heightMod;
		this.rootRigid = this.transform.root.GetComponent<Rigidbody> ();
	}
	// Update is called once per frame
	void FixedUpdate () {
		RaycastHit hit;
		Debug.DrawRay (this.hoverTrans.position, Vector3.down, Color.red, hoverHeight);
		if (Physics.Raycast (new Vector3(this.hoverTrans.position.x,this.transform.position.y,hoverTrans.position.z), Vector3.down, out hit, hoverHeight,layer)
			&&hit.transform.GetComponent<Holdable>()==null && hit.transform.root!=this.transform.root) {
			Debug.Log ("Raycast hoverhit--"+hit.transform.name);
			//If there is ground underneath the player have the body hover over it.
			/*Vector3 newPosition = new Vector3 (
				this.transform.root.position.x,
				hit.point.y + hoverHeight * 1.05f,
				this.transform.root.position.z);
			this.transform.root.position = newPosition;*/
			rootRigid.velocity.Set (this.rootRigid.velocity.x, 0, this.rootRigid.velocity.z);
			rootRigid.useGravity = false;

			if (hit.distance < heightNoTele) {

				//this.transform.position += Vector3.up * .1f;
				this.transform.position = new Vector3(this.transform.position.x,hit.point.y+ heightNoTele,transform.position.z);
				//this.hoverTrans.position += Vector3.up * this.hoverHeight * 1.02f;
			}
		} else {
			//Debug.Log ("Raycast notHit");

			rootRigid.useGravity = true;
		}
	}
}
