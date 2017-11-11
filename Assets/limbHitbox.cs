using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limbHitbox : MonoBehaviour {
	public float damageMultiplier=1;
	public int minDamage=0;
	public int maxDamage=1000;

	public entity mob;
	void Start(){
		if (mob == null) {
			mob = this.transform.root.GetComponent<entity> ();
		}
	}
	public void takeDamage(int amount, CollisionDamage source, Vector3 point){
		mob.TakeDamage ((int)Mathf.Min(damageMultiplier*amount+minDamage,maxDamage),source, point);
	}
}
