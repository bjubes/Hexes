using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class TileSpriteController : MonoBehaviour {

	public GameObject TileUIText;
	public Sprite tileSprite;
	public float xBorder = 0.2f;
	public float yBorder = 0.05f; 
	public float vertOffset = 0.47f;
	public Dictionary<Tile,GameObject> tileGameObjectMap;
	public Dictionary<Tile, Text> tileUITextmap;


	void OnEnable () {
		Transform letterParent = GameObject.FindWithTag ("Letter UI Parent").transform;

		tileGameObjectMap = new Dictionary<Tile, GameObject>();
		tileUITextmap = new Dictionary<Tile, Text> ();

		foreach( Tile t in Grid.Instance.GetAllTiles()) {
			//create and register new game object
			GameObject tileGO = new GameObject("Tile " + t.x + " " + t.y);
			tileGO.AddComponent<SpriteRenderer>().sprite = tileSprite;
			tileGO.transform.parent = this.transform;
			tileGO.transform.position = HexPosFromTileCoords(t.x,t.y);
			tileGameObjectMap.Add(t,tileGO);
			t.OnTileStateChanged += TileStateChanged;

			//create and register new UIText
			GameObject tileText = (GameObject)Instantiate(TileUIText);
			tileText.name = "Tile UI Text " + t.x + " " + t.y;
			Text UIText = tileText.GetComponent<Text> ();
			UIText.text = t.letter.ToString();
			tileText.transform.SetParent(letterParent,false);
			tileText.GetComponent<UIFollowGameObject> ().objectToFollow = tileGO.transform;
			tileUITextmap.Add(t,UIText);
		}
	}
	
	void TileStateChanged (Tile t) {
		if (t.tileState == TileState.Selected) {
			tileGameObjectMap[t].GetComponent<SpriteRenderer>().color = Color.grey;
		}
		else if (t.tileState == TileState.Neutral) {
			tileGameObjectMap[t].GetComponent<SpriteRenderer>().color = Color.white;
			tileUITextmap[t].text = t.letter.ToString();

		}
		else if (t.tileState == TileState.Taken) {
			tileUITextmap[t].text = t.letter.ToString();
			tileGameObjectMap[t].GetComponent<SpriteRenderer>().color = t.team.color;
		}

		else if (t.tileState == TileState.Endangered) {
			tileUITextmap[t].text = t.letter.ToString();
			tileGameObjectMap[t].GetComponent<SpriteRenderer>().color = t.team.endangerColor;
		}
	}

	public Vector2 HexPosFromTileCoords(int x, int y) {
		float yPos = y - (y* yBorder);
		float xPos = x - (x* xBorder);
		if (x % 2 == 0) {
			yPos -= vertOffset;
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
			pos.y += vertOffset;
		}

		//now solve for y in the same manner as x
		for (int tryY = 0; tryY < Grid.Instance.Height; tryY++) {
			float thisGuess = Mathf.Abs(tryY - (tryY * yBorder)  - pos.y);
			if( thisGuess < yGuessDiff) {
				closestY = tryY;
				yGuessDiff = thisGuess;

			}
		}

		if (yGuessDiff + xGuessDiff > 0.7f) {
			//we have clicked too far away for this to be considered any tile at all
			return Vector2.one * -1 ; //this is a symbol for "null"
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
