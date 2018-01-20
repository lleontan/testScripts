using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles touchHoldEvents
public abstract class touchHoldHandler : MonoBehaviour {
	public abstract void onTouchHold();
	public abstract void onRelease();

}
