using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : MonoBehaviour {
	/* When held, rotate this object so that its rotation is in the same direction as
		primaryHandle to secondary handle and set position to primary handle.

		Only assign hands in the ObjectHandles from the objecthandles themselves.
	*/ 
	public ObjectHandle[] handles;	//All handles.
	public GameObject[] layerChangingChildren;
	private ObjectHandle primaryHandle;
	private ObjectHandle secondaryHandle;
	public bool changeAllChildrenLayers=true;
	public bool canHold=true;
	private Queue<Vector3> framePositions;
	private Queue<float> frameTimings;
	public int averagedFramesCount=5;
	private Rigidbody rigid;
	public bool twoHanded = false;			//if twoHanded is true then the secondary handle can be used
	//public float maxDropForce=100f;
	public string heldLayer="ignorePlayer";
	private int heldLayerInt;
	private int notHeldLayerInt;
	public string notHeldLayer="Default";
	public bool changeOwnLayer=true;
	private entity lastHeldEntity;				//the last entity to hold this object
	public ObjectHandle defaultHandle;
	public bool doMovement = true;
	private Attachment thisAttachment;

	public bool poolOnDrop = false;				//if true and inventory exists, the item will be deactivated and set as a child to inventory
	public Transform inventory;
	public string description="";
	public string getDescription(){
		return this.description;
	}
	public ObjectHandle GetDefaultHandle(){
		//Returns the default handle
		//Returns null if no default Handle exits
		return this.defaultHandle;
	}
	public bool hasDefaultHandle(){
		return this.defaultHandle != null;
	}

	void Awake () {
		thisAttachment = this.GetComponent<Attachment> ();
		this.heldLayerInt = LayerMask.NameToLayer (heldLayer);
		this.notHeldLayerInt = LayerMask.NameToLayer(notHeldLayer);

		this.rigid = GetComponent<Rigidbody> ();
		framePositions = new Queue<Vector3> (this.averagedFramesCount);
		frameTimings = new Queue<float> (this.averagedFramesCount);
		foreach(ObjectHandle handle in handles){
			handleHandshake (handle);
		}
	}
	private void handleHandshake(ObjectHandle handle){
		// Assigns this object to handles parent reference.
		handle.setHoldable(this);
	}
	public entity getLastHolder(){
		return this.lastHeldEntity;
	}
	public bool attemptAssignHandle(ObjectHandle handle, entity newHolder){
		if (this.attemptAssignHandle (handle)) {
			this.lastHeldEntity = newHolder;
			return true;
		} else {
			return false;
		}
	}
	public bool attemptAssignHandle(ObjectHandle handle){
		if(thisAttachment){
			if (thisAttachment.canDetach ()) {
				thisAttachment.AttemptDetach ();
			}
		}
		assignHandle (handle);
		return true;
	}
	private void assignHandle(ObjectHandle handle){
		//When a handle says its being grabbed assign it to primary or secondary.


		if (primaryHandle == null) {
			// If there is no primary
			if (secondaryHandle == null) {
				// No Primary and Secondary
				if(this.changeAllChildrenLayers){
					this.stringentAssignLayer (this.heldLayerInt);
				}
				primaryHandle = handle;
			} else {
				// No primary and there IS a secondary
				if (secondaryHandle.priority > handle.priority) {
					primaryHandle = secondaryHandle;
					secondaryHandle = handle;
				} else {
					primaryHandle = handle;
				}
			}
		} else if(twoHanded){
			//There is a primary
			//Two handed only allowed when true.
			secondaryHandle = handle;
		}
		reassignHandles ();//Just to be safe, remove after testing.
		setGravity();
	}
	void OnCollisionEnter(Collision col){
		if (this.isHeld()&&col.gameObject.tag == "player") {
			Physics.IgnoreCollision (col.contacts[0].otherCollider,col.contacts[0].thisCollider);
		}
	}
	private void releaseHandle(ObjectHandle handle, bool canDepool){
		// Do not call from this script except from full and object releases.
		//When a handle says it is released release it and reassign primary and secondary.
		bool startedHeld=this.primaryHandle||this.secondaryHandle;
		Vector3 offset= Vector3.zero;
		float dropForceMultiplier = 1f;
		if(handle==primaryHandle){
			dropForceMultiplier = primaryHandle.getThrowMultiplier ();
			offset = primaryHandle.angleOffset;
			primaryHandle = secondaryHandle;
		}
		secondaryHandle = null;
		setGravity ();
		if (primaryHandle == null) {//Separated for now
			if(this.changeAllChildrenLayers){
				stringentAssignLayer (this.notHeldLayerInt);
			}
			if (this.framePositions.Count >= this.averagedFramesCount) {
				//Averaging nFrames
				float totalTime = frameTimings.Dequeue ();
				Vector3 distanceVector = Vector3.zero;
				Vector3 lastPos = framePositions.Dequeue ();
				while (framePositions.Count > 0) {
					Vector3 thisPos = framePositions.Dequeue ();
					distanceVector = distanceVector + thisPos - lastPos;
					lastPos = thisPos;
					totalTime += frameTimings.Dequeue ();
				}

				//Working
				if (rigid) {
					rigid.velocity = Quaternion.EulerAngles (-offset) *
					distanceVector /
					totalTime * dropForceMultiplier;
				}
				//rigid.AddForce (distanceVector/totalTime*this.dropForceMultiplier*3f);
				resetFramePositions ();
			}
			if (this.poolOnDrop && startedHeld && canDepool) {
				//this.transform.parent = inventory;
				//this.inventory.GetComponent<InventoryPool>().addToPool(this.gameObject);
				//this.transform.localPosition = Vector3.zero;
				this.gameObject.SetActive (false);
			}
		}
	}
	void OnDisable(){
		releaseAll (true);
	}
	public void releaseAll(bool canDepool){
		foreach (ObjectHandle handle in this.handles) {
			this.fullReleaseHandle (handle, canDepool);
		}
	}
	private void fullReleaseHandle(ObjectHandle handle,bool canDepool){
		//Called from this script.
		this.releaseHandle (handle, canDepool);
		handle.HoldableReleaseHold();
	}
	public void objectHandleRelease(ObjectHandle handle, bool canDepool){
		//Called from Object Handle
		this.releaseHandle(handle, canDepool);
	}
	private void setGravity (){
		if (rigid) {
			if (primaryHandle) {
				rigid.useGravity = false;
			} else {
				rigid.useGravity = true;
			}
		}
	}
	private void resetFramePositions(){
		framePositions = new Queue<Vector3> (this.averagedFramesCount);
		frameTimings = new Queue<float> (this.averagedFramesCount);
	}
	private void reassignHandles(){
		//Reassigns handles to primary or secondary depending on their priorityLevel.
		if(primaryHandle!=null){
			if (secondaryHandle != null) {
				if (secondaryHandle.priority > primaryHandle.priority) {
					ObjectHandle newHandle = primaryHandle;
					primaryHandle = secondaryHandle;
					secondaryHandle = newHandle;
					resetFramePositions ();
				}
			}
		} else if(this.secondaryHandle!=null){
			//If there is no primary Handle and there is a secondary handle
			this.primaryHandle=secondaryHandle;
			this.secondaryHandle = null;
			resetFramePositions ();
		}
	}
	void LateUpdate () {
		if (doMovement) {
			//Object is transformed so that the primaryHandle is always at the primary hand.
			if (primaryHandle != null) {
				
				this.frameTimings.Enqueue (Time.deltaTime);
				this.framePositions.Enqueue (this.transform.position);
				if (this.framePositions.Count > this.averagedFramesCount) {
					this.framePositions.Dequeue ();
					this.frameTimings.Dequeue ();
				}
				if (this.secondaryHandle != null) {
					//If there is a secondary handle rotate to face it.
					/*If the distance between the two handles is too great then
				  the secondaryHandle will release.*/
					if (secondaryHandle.CheckBounds (this.primaryHandle.hand.transform)) {
						this.transform.rotation = Quaternion.LookRotation (
							secondaryHandle.hand.transform.position
							- primaryHandle.hand.transform.position) *
						Quaternion.EulerAngles (primaryHandle.angleOffset);
						switch(primaryHandle.controllerZToAxis){
							case 1:
								this.transform.rotation*= Quaternion.Euler(primaryHandle.hand.transform.rotation.eulerAngles.z,0, 0);		//WARNING TESTING THIS LINE. REMOVE IF SHIT GOES WRONG.
								break;
							case 2:
								this.transform.rotation*= Quaternion.Euler(0,primaryHandle.hand.transform.rotation.eulerAngles.z, 0);		//WARNING TESTING THIS LINE. REMOVE IF SHIT GOES WRONG.
								break;
							case 3:
								this.transform.rotation*= Quaternion.Euler(0, 0, primaryHandle.hand.transform.rotation.eulerAngles.z);		//WARNING TESTING THIS LINE. REMOVE IF SHIT GOES WRONG.
								break;
						}
					} else {
						//Break connection if the secondaryHandle is out of its bounds.
						this.fullReleaseHandle (secondaryHandle, false);
					}
				} else {
					Quaternion handOffset= primaryHandle.hand.transform.rotation;
					if (primaryHandle.transform.parent != null) {
						handOffset *= primaryHandle.transform.localRotation;
					}
					this.transform.rotation = handOffset
						* Quaternion.EulerAngles (primaryHandle.angleOffset);
				}
				//WORKING
				this.transform.position = primaryHandle.hand.transform.position
				- (primaryHandle.holdTrans.position - this.transform.position);
				//Rigidbody thisRigid = this.GetComponent<Rigidbody> ();	//Rigidbodies can be removed by attachments so we're getting it here instead of initializing them.
				//thisRigid.
			}
		}
	}
	public bool isHeld(){
		return (this.primaryHandle !=null || this.secondaryHandle != null);
	}
	public void assignLayer(int layer){
		//Changes layer for all non-objectHandle children.
		if (this.changeAllChildrenLayers) {
			recursiveChildLayerAssignment (this.transform, layer);
		} else {
			foreach (GameObject obj in layerChangingChildren) {
				//obj.layer = layer;
				recursiveChildLayerAssignment(obj.transform, layer);
			}
		}
		if(changeOwnLayer){
			this.gameObject.layer = layer;
		}
	}
	public void stringentAssignLayer(int layer){
		if (this.primaryHandle == null && this.secondaryHandle == null) {
			this.assignLayer (layer);
		}
	}

	private void recursiveChildLayerAssignment(Transform obj, int layer){
		//Changes the layer of every child object that is not a objectHandle
		foreach (Transform child in obj) {
			bool notHandle = true;
			foreach (ObjectHandle handle in handles) {
				if (child == handle.transform) {
					notHandle = false;
					break;
				}
			}
			if (notHandle) {
				child.gameObject.layer = layer;
				recursiveChildLayerAssignment (child, layer);
			}
		}
	}
}