using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPooler : MonoBehaviour {
	//On Trigger Enter adds unheld objects to an inventory

	public bool canPool=true;		//Can the pooler pool objects.

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerStay(Collider col){
		Holdable hold = col.transform.root.GetComponent<Holdable> ();
		if (hold && !hold.isHeld ()) {
			
		}
	}
	private void addToInventory(GameObject obj){
		//Adds the selected object to the inventory.

	}
}
