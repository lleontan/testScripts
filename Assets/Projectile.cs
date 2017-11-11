using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
	public bool addInitialForce=true;
	/* Instantiated projectile behavior, initialSpeed
	*/
	private Rigidbody rigid;
	public float initialForce=400f;
	// Use this for initialization
	void Start () {
		
		this.rigid = this.GetComponent <Rigidbody> ();
		if (this.addInitialForce) {
			this.rigid.AddForce (this.transform.forward * initialForce);
		}
	}
	void OnCollisionEnter(Collision col){
		Debug.Log ("Projectile Collision:"+col.gameObject.name+"-"+this.gameObject.name);
	}
}
