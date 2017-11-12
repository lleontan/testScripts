using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to a gameobject with a collider
[RequireComponent(typeof(Collider))]
public class limbHitbox : MonoBehaviour {
	public float damageMultiplier=1;
	public int minDamage=0;
	public int maxDamage=1000;

	public entity thisEnt;
	void Start(){
		if (thisEnt == null) {
			thisEnt = this.transform.root.GetComponent<entity> ();
		}
	}
	public void takeDamage(int amount, CollisionDamage source, Vector3 point){
		thisEnt.TakeDamage ((int)Mathf.Min(damageMultiplier*amount+minDamage,maxDamage),source, point);
	}
}
