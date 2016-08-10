using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileSpriteController : MonoBehaviour {

	public Sprite tileSprite;
	public float xBorder = 0.2f;
	public float yBorder = 0.05f; 
	public float vertOffset = 0.47f;
	public Dictionary<Tile,GameObject> tileGameObjectMap;


	void Start () {
		tileGameObjectMap = new Dictionary<Tile, GameObject>();

		foreach( Tile t in Grid.Instance.GetAllTiles()) {
			GameObject tileGO = new GameObject("Tile " + t.x + " " + t.y);
			tileGO.AddComponent<SpriteRenderer>().sprite = tileSprite;
			tileGO.transform.parent = this.transform;
			tileGO.transform.position = HexPosFromTileCoords(t.x,t.y);
			tileGameObjectMap.Add(t,tileGO);
		}
	}
	
	void Update () {
	
	}

	public Vector2 HexPosFromTileCoords(int x, int y) {
		float yPos = y - (y* yBorder);
		float xPos = x - (x* xBorder);
		if (x % 2 == 0) {
			yPos += vertOffset;
		}

		return new Vector2(xPos,yPos);
	}

	//find a hex's tile coord using "closest wins" algorithm. Doesnt Work
	public Vector2 TileCoordsFromHexPos(Vector2 pos){

		//vars for best guess
		float yGuessDiff = Mathf.Infinity;
		float xGuessDiff = Mathf.Infinity;
		int closestX = -1;
		int closestY = -1;

		//guess to solve eq: yPos = y - y * yBorder 
		//where "y" is the integer grid pos
		for (int tryX = 0; tryX < Grid.Instance.Width; tryX++) {
			float thisGuess =  Mathf.Abs(tryX - (tryX * xBorder) - pos.x);
			if( thisGuess  < xGuessDiff) {
				closestX = tryX;
				xGuessDiff = thisGuess;
			}
		}

		//now that x is known, see if we need to undo the vertOffset
		if (closestX % 2 == 0) {
			pos.y -= vertOffset;
		}

		//now solve for y in the same manner as x
		for (int tryY = 0; tryY < Grid.Instance.Width; tryY++) {
			float thisGuess = Mathf.Abs(tryY - (tryY * yBorder)  - pos.y);
			if( thisGuess < yGuessDiff) {
				closestY = tryY;
				yGuessDiff = thisGuess;

			}
		}
			
		return new Vector2(closestX,closestY);
	}

	public Tile GetTileFromGameObject(GameObject go) {
		foreach(Tile t in tileGameObjectMap.Keys) {
			if (tileGameObjectMap[t] == go) {
				return t;
			}
		}
		Debug.LogError("No Tile found for gameobject: " + go.name);
		return null;
	}
}
