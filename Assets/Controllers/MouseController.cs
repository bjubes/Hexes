using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {

	TileSpriteController tsp;

	Vector2 lastMousePos, currMousePos;
	const bool debugging = true;

	void Start () {
		tsp = GameObject.FindObjectOfType<TileSpriteController>();
	}
	
	void Update () {
		//Dont do anything before setting new mouse pos
		currMousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition );

		Dcommand ();
		MoveCameraUsingPan ();

		DetectTileClicks ();
//		//testing purposes
//		if (Input.GetMouseButtonUp(0) && debugging){
//			currMousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition);
//			Vector2 tilePos = tsp.TileCoordsFromHexPos(currMousePos);
//			Tile tile = Grid.Instance.GetTile((int)tilePos.x, (int)tilePos.y);
//			Tile[] neighbors =tile.GetNeighbors();
//			foreach (Tile t in neighbors) {
//				tsp.tileGameObjectMap[t].gameObject.GetComponent<SpriteRenderer>().enabled = false;
//				//Debug.Log(tsp.tileGameObjectMap[t].gameObject.name);
//			}
//		}
//		//end testing code

	}

	void LateUpdate() {
		lastMousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
	}


	void MoveCameraUsingPan() {
		// Handle screen panning
		if( Input.GetMouseButton(1) || Input.GetMouseButton(2) ) {	// Right or Middle Mouse Button

			Vector3 diff = lastMousePos - currMousePos;
			Camera.main.transform.Translate( diff );

		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");

		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
	}

	void DetectTileClicks() {
		if (Input.GetMouseButtonUp (0)) {
			Tile tile = GetTileUnderMouse ();
			if (tile != null) {
				tile.CallOnTileClickedOnSelf ();
			}
		}
	}

	Tile GetTileUnderMouse() {
		Vector2 tilePos = tsp.TileCoordsFromHexPos (currMousePos);
		return Grid.Instance.GetTile ((int)tilePos.x, (int)tilePos.y);
	}

	void Dcommand() {
		if (debugging && Input.GetKeyDown(KeyCode.D)){
			Debug.Log(GetTileUnderMouse().tileState);
		}
	}
}
