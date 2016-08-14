using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Tile  {

	public TileState tileState{ get; protected set;}

	public int x;
	public int y;
	public char letter;  //  "\0" is unicode for "null"

	public Team team;

	public Tile (int x, int y) {
		this.x = x;
		this.y = y;
		SetTileState (TileState.Empty, true);

		//letter = (char)UnityEngine.Random.Range (65, 91);  //a to z in ascii
	}

	public Action<Tile> OnTileClicked;
	public Action<Tile> OnTileStateChanged;

	public Tile[] GetNeighbors() {
		List<Tile> tiles = new List<Tile>();
		int x = this.x;
		int y = this.y;
		Grid g = Grid.Instance;

		//add four direct neighbors
		tiles.AddIfNotNull(g.GetTile(x,y+1));
		tiles.AddIfNotNull(g.GetTile(x,y-1));
		tiles.AddIfNotNull(g.GetTile(x+1,y));
		tiles.AddIfNotNull(g.GetTile(x-1,y));

		//get the two non-obvious neighbors. these two extra tiles are
		//neighbors due to the visual offset from a hex grid effect.

		if (x % 2 == 0) {
			//the last two tiles are, in a square grid, diag down left and diag down right
			tiles.AddIfNotNull(g.GetTile(x+1,y-1));
			tiles.AddIfNotNull(g.GetTile(x-1,y-1));
		} else {
			//the last two tiles are, in a square grid, diag up left and diag up right
			tiles.AddIfNotNull(g.GetTile(x+1,y+1));
			tiles.AddIfNotNull(g.GetTile(x-1,y+1));

		}
		return tiles.ToArray();

	}

	public void CallOnTileClickedOnSelf() {
		if (OnTileClicked != null) {
			OnTileClicked (this);
		}
	}

	/// <summary>
	/// Sets the state of the tile.
	/// </summary>
	/// <returns><c>true</c>, if tile state was set, <c>false</c> otherwise.</returns>
	/// <param name="init">overrides usual logic rules for cases where a tile should be regarded as "new"</param>
	public bool SetTileState(TileState state, bool init = false) {
		bool success = false;

		switch (state) {
			
		case TileState.Selected:
				//we can only be selected if we were previously neutral
			if (tileState != TileState.Neutral) {
			} else {
				//we can be selected, so change the tile state
				tileState = TileState.Selected;

				//tell neighbors to check if they are now endangered.
				success = true;
			}
			break;
		
		case TileState.Neutral:
			if (tileState == TileState.Taken) {
				//a tile cannot be taken, it would have to previously be endangered...
				//so this is clearly wrong
				Debug.LogError ("Tile in 'Taken' State is being set to neutral");

			} else {
				if (tileState == TileState.Taken || tileState == TileState.Empty || init) {
					SetRandomLetter ();
				}
				//some neighbors may have to be alerted that they are no longer in danger
			
				tileState = TileState.Neutral;
				success = true;
			}
			break;

		case TileState.Taken:
			 if (tileState != TileState.Selected && init == false) {
				//since we have not selected this tile, it should not be changing ownership.
				Debug.LogError ("attempted to make tile 'taken' tile without selecting it first");
			} else {
				tileState = TileState.Taken;
				letter = '\0';
				//tell our neighbors that they should be seeing if they have to change state as well
				//i.e. a taken neighbor of the other color has to now become neutral.

				//tell neighbors that if they are empty, they now must become neutral
				foreach(Tile t in GetNeighbors()){
					if (t.tileState == TileState.Empty) {
						t.SetTileState (TileState.Neutral);
					}	
				}

				success = true;
			}
			break;
				
		case TileState.Empty:
			if (!init) {
				//cannot occour unless this is a refresh of the board
				Debug.LogError ("Tile cannot be changed to empty");
			} else {
				letter = '\0';
				tileState = TileState.Empty;
				success = true;
			}
			break;

		default:
			Debug.LogError ("State Invalid");
			break;
		}


		if (success && OnTileStateChanged != null) {
			OnTileStateChanged (this);
		}
		return success;
	}

	//tile sees if it is in danger of being unclaimed. i.e. will set endangered if it has a neighbor that is
		//selected and touching the blob of an enemy color
	public void CheckIfEndangered() {
		throw new NotImplementedException ();
	}

	//will turn neighbors who are taken by the enemy into neutral tiles
	public void PushBackNeighbors() {
		throw new NotImplementedException ();
	}

	//returns whether this tile is touching a tile of the team given
	public bool IsTouchingTileOfTeam(Team team) {
		//throw new NotImplementedException ();
		foreach(Tile t in GetNeighbors()) {
			if (t != null) { //if this is an edge tile, some neighbors will be null
				if (t.tileState == TileState.Taken && t.team == team) {
					return true;
				}
			}
		}
		return false;
	}

	void SetRandomLetter() {
		letter = Grid.Instance.randomChar.GetRandomLetter ();

	}

}
