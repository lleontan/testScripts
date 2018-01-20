using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour {
	public int damage=25;
	public float maxDistance = 50f;
	public float hitForce = 10f;
	// Use this for initialization
	void Start () {
		RaycastHit hit;
		Vector3 point;
		if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, maxDistance)) {
			point = hit.point;
			entity ent = hit.transform.root.GetComponent<entity> ();
			if (ent) {
				limbHitbox box = hit.transform.GetComponent<limbHitbox> ();
				if (box) {
					box.TakeDamage (damage);
				}else{
					ent.TakeDamage (damage);
				}
			}
			Rigidbody rigid = hit.transform.root.GetComponent<Rigidbody> ();
			if (rigid) {
				rigid.AddForceAtPosition (this.transform.position+ this.transform.forward*hitForce,point);
			}

		} else {
			point=(this.transform.position+ this.transform.forward*maxDistance);
		}
		this.GetComponent<LineRenderer> ().SetPosition (0,this.transform.position);
		this.GetComponent<LineRenderer> ().SetPosition (1,point);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
