using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour {

	public Grid grid;

	void OnEnable () {
		grid = new Grid(4,6);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
