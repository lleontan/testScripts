using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerEnterDamage : MonoBehaviour {
	//On Trigger Enter applies damage to the root entity
	public int damage=20;
	public entity damageOwner;
	void OnTriggerEnter(Collider col){
		entity ent = col.GetComponent<entity> ();
		if (ent) {
			ApplyDamage (ent,damageOwner);
		} else {
			ent = col.GetComponentInParent<entity> ();
			if (ent) {
				ApplyDamage (ent,damageOwner);
			}
		}
	}
	void ApplyDamage(entity ent, entity damageOwner){
		if (damageOwner) {
			ent.TakeDamage (damage, damageOwner);
		} else {
			ent.TakeDamage (damage);
		}
	}
}
