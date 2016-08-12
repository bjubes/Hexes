using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	[System.Serializable]
	public struct Team {
		public Color32 color;
		public string name;
	}

	public Team[] teams;

	void Start () {
		
	}

}
