using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Tile  {

	public TileState tileState{ get; protected set;}

	public int x;
	public int y;
	public char letter = 'C';  //  "\0" is unicode for "null"

	public Team team;

	public Tile (int x, int y) {
		this.x = x;
		this.y = y;
		tileState = TileState.Neutral;
	}

	public Action<Tile> OnTileClicked;
	public Action<Tile> OnTileStateChanged;

	public Tile[] GetNeighbors() {
		List<Tile> tiles = new List<Tile>();
		int x = this.x;
		int y = this.y;
		Grid g = Grid.Instance;

		//add four direct neighbors
		tiles.Add(g.GetTile(x,y+1));
		tiles.Add(g.GetTile(x,y-1));
		tiles.Add(g.GetTile(x+1,y));
		tiles.Add(g.GetTile(x-1,y));

		//get the two non-obvious neighbors. these two extra tiles are
		//neighbors due to the visual offset from a hex grid effect.

		if (x % 2 == 0) {
			//the last two tiles are, in a square grid, diag up left and diag up right
			tiles.Add(g.GetTile(x+1,y+1));
			tiles.Add(g.GetTile(x-1,y+1));
		} else {
			//the last two tiles are, in a square grid, diag down left and diag down right
			tiles.Add(g.GetTile(x+1,y-1));
			tiles.Add(g.GetTile(x-1,y-1));
		}

		return tiles.ToArray();

	}

	public void CallOnTileClickedOnSelf() {
		if (OnTileClicked != null) {
			OnTileClicked (this);
		}
	}

	//returns whether set request suceeded
	public bool SetTileState(TileState state, bool init = false) {
		bool success;

		switch (state) {
			
		case TileState.Selected:
				//we can only be selected if we were previously neutral
			if (tileState != TileState.Neutral) {
				success = false;
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
				success = false;

			} else {
				tileState = TileState.Neutral;
				//some neighbors may have to be alerted that they are no longer in danger

				success = true;
			}
			break;

		case TileState.Taken:
			 if (tileState != TileState.Selected && init == false) {
				//since we have not selected this tile, it should not be changing ownership.
				Debug.LogError ("attempted to make tile 'taken' tile without selecting it first");
				success = false;
			} else {
				tileState = TileState.Taken;
				//tell our neighbors that they should be seeing if they have to change state as well
				//i.e. a taken neighbor of the other color has to now become neutral.
				success = true;
			}
			break;
				

		default:
			Debug.LogError ("State Invalid");
			success = false;
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
	public bool IsTouchingTileOfTeam(GameManager team) {
		throw new NotImplementedException ();
	}
}
