using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Team {
	public Color32 color;
	public Color32 endangerColor;
	public string name;
	public int startPosX;
	public int startPosY;

	public static bool operator ==(Team t1, Team t2) 
	{
	    return t1.Equals(t2);
	}

	public static bool operator !=(Team t1, Team t2) 
	{
	   return !t1.Equals(t2);
	}
}