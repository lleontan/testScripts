using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SliderEvent : HandleEvents {
	//Events for a sliderElement;
	protected positionSlider controller;
	//XYZ are in magnitude percentages from localPosition (0,0,0)
	public abstract void PositionUpdate (float x, float y, float z);
	public virtual void onStart(positionSlider thisController){
		controller = thisController;
		this.ChildOnStart ();
	}
	public virtual void ChildOnStart(){}
}
