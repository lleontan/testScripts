using System;
using UnityEngine;
public abstract class OnControllerTouch : MonoBehaviour {
	abstract public void onTriggerEnter(Controller controller);
	abstract public void onTriggerStay (Controller controller);
	abstract public void onTriggerExit (Controller controller);
	abstract public void onCollisionEnter(Controller controller);
	abstract public void onCollisionStay (Controller controller);
	abstract public void onCollisionExit (Controller controller);

}

