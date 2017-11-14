using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRigHover : MonoBehaviour {

	public float hoverHeight= 1.9f;				//HoverHeight above the ground
	private Rigidbody rootRigid;
	private int layer;
	public Transform hoverTrans;
	void Start(){
		//layer = LayerMask.NameToLayer ("Default");
		layer=1;
		this.rootRigid = this.transform.root.GetComponent<Rigidbody> ();
	}
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Debug.DrawRay (this.hoverTrans.position, Vector3.down, Color.red, hoverHeight);
		if (Physics.Raycast (this.hoverTrans.position, Vector3.down, out hit, hoverHeight,layer)&&hit.transform.GetComponent<Holdable>()==null && hit.transform.root!=this.transform) {
			//Debug.Log ("Raycast hoverhit");
			//If there is ground underneath the player have the body hover over it.
			/*Vector3 newPosition = new Vector3 (
				this.transform.root.position.x,
				hit.point.y + hoverHeight * 1.05f,
				this.transform.root.position.z);
			this.transform.root.position = newPosition;*/
			rootRigid.useGravity = false;
			rootRigid.velocity.Set (this.rootRigid.velocity.x, 0, this.rootRigid.velocity.z);
			this.transform.position = hit.point + Vector3.up * this.hoverHeight * 0.99f;
		} else {
			//Debug.Log ("Raycast notHit");

			rootRigid.useGravity = true;
		}
	}
}
