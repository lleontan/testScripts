using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuButtonReleaseEquip : OnControllerTouch {
	// 
	public GameObject item;		//Item to equip
	public menuHoldActive menu;		//Menu to close on equip
	public Material highlight;
	private Material defaultMat;
	public TextMesh itemDescription;
	// Use this for initialization
	void Start () {
		this.defaultMat = this.GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void OnDisable () {
		this.GetComponent<Renderer> ().material = defaultMat;
	}
	public override void onTriggerEnter(Controller controller){
		this.GetComponent<Renderer> ().material = highlight;
	}
	public override void onTriggerStay (Controller controller){
		if (controller.getMenuButtonUp ()) {
			if(item){item.SetActive (true);
			Holdable objectHoldable = item.GetComponent<Holdable> ();
			if (objectHoldable) {
				controller.PickUpObjectHandle (item.GetComponent<Holdable> ().GetDefaultHandle ());
			} else {
				ObjectHandle handle=item.GetComponent<ObjectHandle> ();
				if (handle) {
					controller.PickUpObjectHandle (handle);
				}
			}
			item.SetActive (true);																			//Can be deactivated through itemPoolOption
			menu.Deactivate ();
			}
		}
	}
	public override void onTriggerExit (Controller controller){
		this.GetComponent<Renderer> ().material = defaultMat;
	}
	public override void onCollisionEnter(Controller controller){}
	public override void onCollisionStay (Controller controller){}
	public override void onCollisionExit (Controller controller){}
}
