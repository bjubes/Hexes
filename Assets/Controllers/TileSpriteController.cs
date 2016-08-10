using UnityEngine;
using System.Collections;

public class TileSpriteController : MonoBehaviour {

	public Sprite tileSprite;
	public float xBorder = 0.2f;
	public float yBorder = 0.05f; 
	public float vertOffset = 0.47f;



	void Start () {
		foreach( Tile t in Grid.Instance.GetAllTiles()) {
			GameObject tileGO = new GameObject("Tile " + t.x + " " + t.y);
			tileGO.AddComponent<SpriteRenderer>().sprite = tileSprite;
			tileGO.transform.parent = this.transform;
			tileGO.transform.position = HexPosFromTileCoords(t.x,t.y);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Vector2 HexPosFromTileCoords(int x, int y) {
		float yPos = y - (y* yBorder);
		float xPos = x - (x* xBorder);
		if (x % 2 == 0) {
			yPos += vertOffset;
		}

		return new Vector2(xPos,yPos);
	}
}
