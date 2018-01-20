using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
	public bool addInitialForce=true;
	/* Instantiated projectile behavior, initialSpeed
	*/
	private Rigidbody rigid;
	public Transform ownerRoot;
	public float initialForce=400f;
	public Collider thisCollider;
	// Use this for initialization
	private int defaultLayer;
	void Start () {
		this.defaultLayer = LayerMask.NameToLayer ("Default");
		//Debug.Log (this.transform.rotation.eulerAngles);
		this.rigid = this.GetComponent <Rigidbody> ();
		if (this.addInitialForce) {
			AddStartingForce ();
		}
	}
	void OnCollisionEnter(Collision col){
		Debug.Log ("Projectile Collision:"+col.gameObject.name+"-"+col.collider.name+"-"+this.gameObject.name);
		/*if (col.transform.root == ownerRoot) {
			Debug.Log ("Projectile root match:"+col.gameObject.name+"-"+col.collider.name+"-"+this.gameObject.name);
			Physics.IgnoreCollision (ignoreCollisions,col.collider);
		}*/
	}
	void Update(){
		//Raycast forward .5m, if true then set collider trigger to a collider.
		if (this.thisCollider.isTrigger) {
			RaycastHit hit;
			if (Physics.Raycast (this.transform.position, this.transform.forward,out hit, 0.5f,defaultLayer)&&hit.transform.tag!="p") {
				thisCollider.isTrigger = false;
			}
		}
	}
	public void AddStartingForce(){
		this.rigid.AddForce (this.transform.forward * initialForce);
	}
}
