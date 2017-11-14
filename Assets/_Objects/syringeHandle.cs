using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class syringeHandle : HandleEvents {
	/* Onclick raycasts from the syringe transform if the animation state is in idle.
	 * Checks all top level objects for entity and initiates a heal if successful.
		//WARNING, INDICATOR MATERIAL INDEX IS HARDCODED AS 1;
	*/
	public float needleLength=.2f;
	public Transform needleTransform;	//Where the thing is being raycast from.
	public int healAmount = 50;
	public int charges=5;
	private int remainingCharges;
	public Animator thisAnimator;
	private Material chargesGauge;
	private Color originalColor;
	public MeshRenderer syringeRenderer;
	public string[] raycastLayers={"Default, UI, player, ignoreRaycast, onlyDefault"};
	private int physicsLayers;
	// Use this for initialization
	void Start () {
		physicsLayers = LayerMask.GetMask (this.raycastLayers);
		remainingCharges = charges;
		if (charges == 0) {
			charges = 1;
		}
		if (thisAnimator == null) {
			thisAnimator = this.transform.root.GetComponent<Animator> ();
		}
		this.originalColor = this.syringeRenderer.materials[1].color;//new Color(chargesGauge.color.r,chargesGauge.color.g,chargesGauge.color.b);
		this.syringeRenderer.materials [1] = new Material (this.syringeRenderer.materials[1]);
		this.chargesGauge = this.syringeRenderer.materials [1];

	}
	public override void onPickup(Hand controller){
	}
	public override void onDrop (Hand controller){
	}
	public override void onTriggerClickUp (Controller controller){
		this.thisAnimator.SetBool ("depressSyringe",false);
	}
	public override void onTriggerPress (Controller controller){}
	public override void onTriggerClickDown (Controller controller){
		if (!this.thisAnimator.GetBool ("depressSyringe")&&this.remainingCharges>0) {
			this.remainingCharges--;
			float modifier = this.remainingCharges / this.charges;
			this.chargesGauge.color = new Color (
				originalColor.r*modifier,
				originalColor.g*modifier,
				originalColor.b*modifier);
			thisAnimator.SetBool ("depressSyringe", true);
			this.healRaycast ();
		}
	}
	public override bool onGripUp(Controller controller){
		return true;
	}
	public override void touchPadPress (Controller controller, bool triggerUp,
		bool triggerDown, bool triggerPress, bool gripUp, Vector2 touchpad){

	}
	public void healRaycast(){
		Debug.DrawRay (this.needleTransform.position,this.needleTransform.forward, Color.cyan,5f);
		RaycastHit[] hits = Physics.RaycastAll (needleTransform.position,
			this.needleTransform.forward,this.needleLength,this.physicsLayers);
		foreach (RaycastHit hit in hits) {
			entity newEntity = hit.transform.GetComponent<entity> ();
			if (newEntity) {
				newEntity.Heal (this.healAmount,hit.point);
				break;
			}
		}
	}
}
