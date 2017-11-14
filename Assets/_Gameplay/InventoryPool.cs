using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPool : MonoBehaviour {
	//Pool of deactivated child gameobjects for use in inventories
	private HashSet<GameObject> entries;
	public void addToPool(GameObject obj){
		obj.SetActive (false);
		//obj.transform.parent = this.transform;
		entries.Add (obj);
	}
	public bool removeFromPool(GameObject obj){
		obj.SetActive (true);
		bool contains = entries.Contains (obj);
		if (contains) {
			entries.Remove (obj);
		}
		return contains;
	}
	void Awake(){
		entries=new HashSet<GameObject>();
	}
}
