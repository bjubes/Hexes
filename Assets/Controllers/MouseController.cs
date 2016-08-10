using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour {

	TileSpriteController tsp;

	void Start () {
		tsp = GameObject.FindObjectOfType<TileSpriteController>();
	}
	
	void Update () {
		if (Input.GetMouseButtonUp(0)){
			Vector2 mousePos = Camera.main.ScreenToWorldPoint( Input.mousePosition);
			Vector2 tilePos = tsp.TileCoordsFromHexPos(mousePos);
			Tile tile = Grid.Instance.GetTile((int)tilePos.x, (int)tilePos.y);
			Tile[] neighbors =tile.GetNeighbors();
			foreach (Tile t in neighbors) {
				tsp.tileGameObjectMap[t].gameObject.GetComponent<SpriteRenderer>().enabled = false;
				Debug.Log(tsp.tileGameObjectMap[t].gameObject.name);
			}
		}

	}
}
