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
				foreach(Tile t in GetEndangeredNeighbors(Grid.Instance.currTeam)) {
					t.SetTileState (TileState.Endangered);
				}
				//TODO:recheck taken tiles to see if they are now in danger

				//TODO:on the same note, recheck endagered tiles to see if they are no longer in danger

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
				foreach(Tile t in GetNeighbors()) {
					if (t.tileState == TileState.Endangered && !t.IsEndangered ()) {
						t.SetTileState (TileState.Taken);
					}
				}

				tileState = TileState.Neutral;
				success = true;
			}
			break;

		case TileState.Taken:
			if (tileState != TileState.Selected && tileState != TileState.Endangered && init == false) {
				//since we have not selected this tile, it should not be changing ownership.
				Debug.LogError ("attempted to make tile 'taken' tile without selecting it first");
			} else {
				tileState = TileState.Taken;
				letter = '\0';
				//tell our neighbors that they should be seeing if they have to change state as well
				//i.e. a taken neighbor of the other color has to now become neutral.
				//PushBackNeighbors ();
				//tell neighbors that if they are empty, they now must become neutral
				foreach(Tile t in GetNeighbors()){
					if (t.tileState == TileState.Empty) {
						t.SetTileState (TileState.Neutral);
					}
					if (t.tileState == TileState.Endangered) {
						t.SetTileState (TileState.Taken);
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

		case TileState.Endangered:
			if (tileState != TileState.Taken) {
				Debug.LogError ("Tile cannot be endangered! it has no owner");
			} else {
				tileState = TileState.Endangered;
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


	//will turn neighbors who are taken by the enemy into neutral tiles
	public void PushBackNeighbors() {
		//maybe instead just set all endangered neighbors to taken with currTeam ??
		throw new NotImplementedException ();
	}

	//returns whether this tile is touching a tile of the team given
	public bool IsTouchingTileOfTeam(Team team) {
		//throw new NotImplementedException ();
		foreach(Tile t in GetNeighbors()) {
			if (t.tileState == TileState.Taken && t.team == team) {
				return true;
			}
		}
		return false;
	}

	//FIXME:this method is called too much, as each tile in the trail back calls this.
	bool IsConnectedToTileOfTeam(Team team){
		//this method starts on this tile, then branches to neighbors to see if they are
		//selected. if so it adds them to the new head and repeates until it finds a taken tile of 
		//the team given.

		List<Tile> head = new List<Tile> ();
		head.Add (this);
		while (head.Count > 0) {
			List <Tile> nextHead = new List<Tile> ();
			foreach (Tile headTile in head) {
				if (headTile.IsTouchingTileOfTeam(team)) {
					return true;
				}
				//at this point we know none of our neighbors are our color
				foreach (Tile t in headTile.GetNeighbors()) {
					if (t.tileState == TileState.Selected) {
						nextHead.Add (t);
					}
				}
			}
			//at this point all tiles in headTile have been exhausted
			//delete them and replace them with the new tiles we found
			head = new List<Tile>(nextHead);
			nextHead.Clear ();
		}
		return false;
	}

	//gets a list of endangered neighbors, assuming that this instance of tile is causing them danger
	public List<Tile> GetEndangeredNeighbors(Team myTeam) {
		List<Tile> endangeredTiles = new List<Tile>();

		if (!IsConnectedToTileOfTeam (myTeam)) {
			Debug.Log ("not touching");
			return endangeredTiles;
		}

		foreach (Tile t in GetNeighbors()) {
			if (t.tileState == TileState.Taken && t.team != myTeam ) {
				endangeredTiles.Add (t);
			} 
		}
		return endangeredTiles;
	}

	//am i in danger? unlike GetEndangeredNeighbors, this checks for danger of surroudings,
	//not alerts others of danger

	bool IsEndangered() {
		if (tileState != TileState.Taken) {
			return false;
		}

		foreach (Tile t in GetNeighbors()) {
			//TODO: this does not check if the selected tile is linked back to a tile
			//that is taken and of the enemy color
			if (t.tileState == TileState.Selected && Grid.Instance.currTeam != this.team && t.IsConnectedToTileOfTeam(Grid.Instance.currTeam)) {
				return true;	
			}
		}
		return false;
	}

	void SetRandomLetter() {
		letter = Grid.Instance.randomChar.GetRandomLetter ();

	}

}
