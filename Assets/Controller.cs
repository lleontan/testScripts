using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : Hand {
	/* Base Controller. Checks for menuRaycasts. Hovered Objects.
	 * Adds touchHold functionality to Hand
	 * WARNING!!!
	 * - Requires root transform to contain PlayerRig
	 * WARNING!!!
	*/
	protected Valve.VR.EVRButtonId triggerId = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;	
	protected Valve.VR.EVRButtonId touchpadId=Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
	protected Valve.VR.EVRButtonId gripButtonId= Valve.VR.EVRButtonId.k_EButton_Grip;
	protected Valve.VR.EVRButtonId menuButtonId= Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
	protected SteamVR_TrackedObject trackedObject;
	protected SteamVR_Controller.Device controller {get{ return SteamVR_Controller.Input ((int)trackedObject.index);}}
	protected SteamVR_LaserPointer laserPointer;
	protected playerRig player;						//The player
	protected float menuRaycastDistance=2f;			//Raycasts from hands
	public float pickupDistance=3f;					//pickup raycast distance
	public bool executeControls = true;		//if true controls will be executed when menuRaycasts are false;
	protected Mob playerMob;						//The players mob script
	private int menuLayermask = 0;
	public Collider[] ignoreCollisions;
	protected touchHold heldObj;
	public menuHoldActive menuButtonActivationObj;
	private ObjectHandle lastHeldHandle;
	public menuButtonReleaseEquip updateLastHeld;
	public override bool getCanAutoPickup(){
		return base.getCanAutoPickup () && heldObj == null;
	}
	public override bool isHeld(){
		return base.isHeld () || heldObj != null;
	}
	public override void releaseGrasp(){
		Debug.Log ("Release Grasp");
		if (isHeld()) {
			this.lastHeldHandle= this.pickup;
			if (updateLastHeld) {
				updateLastHeld.item = lastHeldHandle.gameObject;
				if (updateLastHeld.itemDescription) {
					updateLastHeld.itemDescription.text = pickup.getDescription();
				}
			}
		}
		base.releaseGrasp ();
		heldObj = null;
	}

	public void releaseTouchHold(){
		//Only releases the held touchHold if it exists
		//Does not affect objectHandles
		if(heldObj){
			heldObj.controllerRelease ();
			this.heldTransform = null;
			heldObj = null;
		}
	}
	public override void grabTouchHoldObj(touchHold newTouch){
		this.heldObj = newTouch;
		base.grabTouchHoldObj (newTouch);
	}
	public override void PickUpObjectHandle(ObjectHandle handle){
		this.fullReleaseGrasp ( false);
		Debug.Log ("Attempting pickup of-"+handle.name);
		heldObj = null;
		base.PickUpObjectHandle (handle);

	}
	public override void fullReleaseGrasp(bool canDepool){
		if (heldObj) {
			this.heldObj.controllerRelease ();
		}
		base.fullReleaseGrasp (canDepool);
	}
	public entity getControllerEntity(){
		return this.transform.root.GetComponent<entity> ();
	}
	public bool getTriggerPress(){
		return controller.GetPress (this.triggerId);
	}
	public bool getTriggerClickDown(){
		return controller.GetPressDown (this.triggerId);
	}
	public bool getTriggerClickUp(){
		return controller.GetPress (this.triggerId);
	}
	public bool getTouchpadClickDown(){
		return controller.GetPressDown (this.touchpadId);
	}


	protected override void Start () {
		base.Start ();
		this.menuLayermask = LayerMask.NameToLayer ("UI") | LayerMask.NameToLayer ("player");
		trackedObject=this.GetComponent<SteamVR_TrackedObject>();
		player = this.transform.root.GetComponent<playerRig>();
		playerMob = player.GetComponent<Mob> ();
		Collider thisCollider = this.GetComponent<Collider> ();
		foreach (Collider col in this.ignoreCollisions) {
			Physics.IgnoreCollision (thisCollider,col);
		}
	}
	
	// Update is called once per frame
	public bool getMenuButtonUp(){
		return controller.GetPressUp (this.menuButtonId);
	}
	protected override void Update () {
		base.Update ();
		bool triggerUp = controller.GetPressUp (this.triggerId);
		bool triggerDown = controller.GetPressDown (this.triggerId);
		bool triggerPress = controller.GetPress (this.triggerId);
		bool GripUp = controller.GetPressUp (this.gripButtonId);
		bool menuButtonUp = controller.GetPressUp (this.menuButtonId);
		Vector2 touchpad = controller.GetAxis (this.touchpadId);

		bool menuRaycastHit=false;
		if (controller.GetPressDown(this.menuButtonId)) {
			this.menuButtonActivationObj.gameObject.SetActive (true);
			this.menuButtonActivationObj.Activate (this);
		}
		if(!isHeld()){
			menuRaycastHit = CheckMenuRaycasts (triggerUp, triggerDown, triggerPress, GripUp, touchpad);
		}

		if (this.executeControls) {	//Executes menu raycasts.
			if (ExecuteControls (triggerUp, triggerDown, triggerPress, GripUp, touchpad, menuButtonUp)) {		//Interactable objects in world override inHand Objects.

			}else if(GripUp && !menuRaycastHit){	//If the grip was pressed and no menuItem was raycast then drop or pickup with grip.
				PickupRaycast();
			}
		}
		AlwaysExecute(triggerUp, triggerDown, triggerPress, GripUp, touchpad);


		if (this.heldObj) {
			if (triggerUp) {
				//Always release touchHold objects when trigger is released no matter what.
				releaseTouchHold();
			} else {
				// otherwise run the positionClamp
				heldObj.OnHandHold(this);
			}
		}

	}
	void OnTriggerStay(Collider col){
		OnControllerTouch obj = col.GetComponent<OnControllerTouch> ();
		if (obj) {
			checkOnControllerTouch (obj, col);
		}else {
			obj = col.transform.parent.GetComponent<OnControllerTouch> ();
			if (obj) {
				checkOnControllerTouch (obj,col);
			}
		}

	}
	private void checkOnControllerTouch(OnControllerTouch obj, Collider col){
		obj.onTriggerStay (this);
		if (controller.GetPressUp (gripButtonId)) {
			ObjectHandle handle = col.gameObject.GetComponent<ObjectHandle> ();
			if (handle) {
				this.PickUpObjectHandle (handle);
			} else {
				Holdable newHoldable = col.transform.root.GetComponent<Holdable> ();
				if (newHoldable&&newHoldable.defaultHandle!=null) {
					this.PickUpObjectHandle (newHoldable.defaultHandle);
				}
			}
		}
	}
	protected override void OnTriggerEnter(Collider col){
		Debug.Log (col.name+": On Trigger Enter");
		base.OnTriggerEnter (col);
		OnControllerTouch obj = col.GetComponent<OnControllerTouch> ();
		if(obj){
			
			obj.onTriggerEnter(this);
		}
	}
	protected void OnTriggerExit(Collider col){
		OnControllerTouch obj = col.GetComponent<OnControllerTouch> ();
		if(obj){
			obj.onTriggerExit(this);
		}
	}
	public bool getMenuButtonPress(){
		return this.controller.GetPress (this.menuButtonId);
	}
	protected virtual void PickupRaycast(){
		//Raycasts for picking up objectHandles.
		//Releases held object if handle exists.

		if (!this.isHeld()) {
			RaycastHit[] hits = Physics.RaycastAll (this.transform.position,
				                    this.transform.forward, this.pickupDistance);
			bool foundHandle = false;
			foreach (RaycastHit hit in hits) {
				ObjectHandle newHandle = hit.collider.transform.gameObject.GetComponent<ObjectHandle> ();
				if (newHandle) {
					this.PickUpObjectHandle (newHandle);
					foundHandle = true;
					break;
				}
			}
			if (!foundHandle) {
				//Checking Holdables after checking for direct handle hits.
				foreach (RaycastHit hit in hits) {
					Holdable newHold = hit.transform.root.GetComponent <Holdable> ();
					if (newHold && !newHold.isHeld () && newHold.hasDefaultHandle ()) {
						Debug.Log ("objectRaycast handle pickup");
						this.PickUpObjectHandle (newHold.GetDefaultHandle ());
						break;
					}
				}
			}
		} else{
			//this.fullReleaseGrasp ();
		}
	}
	protected virtual bool ExecuteControls(bool triggerUp, bool triggerDown, bool triggerPress, bool GripUp, Vector2 touchpad, bool menuButtonUp){
		//Will not be executed if menu is gazed

		if (this.pickup) {
			if(this.pickup.controllerEvents (this, triggerUp, triggerDown, triggerPress, GripUp, touchpad,menuButtonUp)){
				this.fullReleaseGrasp(true);
				return true;
			};
		}
		return false;
	}
	protected virtual void AlwaysExecute(bool triggerUp, bool triggerDown,bool triggerPress, bool GripUp, Vector2 touchpad){
		//Will always be executed even if menu is gazed.
	}
	protected virtual bool CheckMenuRaycasts(bool triggerUp, bool triggerDown,bool triggerPress, bool GripUp, Vector2 touchpad){
		//Checks menuRaycastElements.
		if (!isHeld()) {
			string[] layers = { "UI", "player" };
			int layermask = LayerMask.GetMask (layers);			//Layermask for UI(4) and player
			//int layermask=16;
			bool elementHit = false;
			RaycastHit[] hits = Physics.RaycastAll (this.transform.position,
				                   this.transform.forward, menuRaycastDistance, layermask);
			foreach (RaycastHit hit in hits) {
				MenuElement element = hit.collider.transform.gameObject.GetComponent<MenuElement> ();
				if (element) {
					elementHit = true;
					this.MenuElementClickEvents (element, hit.point, triggerUp,
						triggerDown, triggerPress, GripUp);
				}
			}
			if (elementHit) {
				// Add a element hit handler here!!!!
			}
			return elementHit;
		}
		return false;
	}
	private void MenuElementClickEvents(MenuElement element, Vector3 hitPostion,bool triggerUp,
		bool triggerDown,bool triggerPress, bool GripUp){
		//Debug.Log (hitPostion+":"+triggerDown+":"+triggerPress+":"+this.transform.position);
		element.OnCastOver (hitPostion, triggerPress, this.transform);
		if (triggerUp) {
			element.OnClickUp (hitPostion, this.transform);
		} else if (triggerDown) {
			element.OnClickDown (hitPostion, this.transform);
		} else if(GripUp){
			element.OnGripUp (hitPostion, this.transform);
		}
	}

	public touchHold getCurrentTouchHold(){
		return this.heldObj;
	}
}
