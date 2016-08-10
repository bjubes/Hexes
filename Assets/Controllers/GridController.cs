using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour {

	public Grid grid;
	public int width, height;
	void OnEnable () {
		grid = new Grid(width,height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
