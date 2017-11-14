using System;

//WARNING: CANNOT USE NULL CHECKS WITH REGULAR CLASSES.
// Handles round information for a mag.
[System.Serializable]
public class mag
{
	
	public int maxRounds = 30;
	public int rounds = 30;
	public mag(mag newMag):this(newMag.rounds,newMag.maxRounds){
	}
	mag ()
	{		
		checkRounds ();
	}
	public mag(int currentRounds):this(){
		this.rounds = currentRounds;
	}
	public mag (int currentRounds, int maxRounds):this(currentRounds)
	{		
		this.maxRounds = maxRounds;
	}

	public bool chamberRound ()
	{
		//true for chambered round false for no rounds to chamber;
		if (this.rounds > 0) {
			this.rounds--;
			return true;
		}
		return false;
	}

	public bool checkRounds ()
	{
		//Returns true for has rounds
		if (this.rounds > this.maxRounds) {
			this.rounds = this.maxRounds;
		} else if (this.rounds < 1) {
			this.rounds = 0;
		} else {
			return true;
		}
		return false;
	}
}

