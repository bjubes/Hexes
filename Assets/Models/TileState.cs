using UnityEngine;
using System.Collections;


// the state of the tile
//						Empty - the tile has no letter and no functionality
//						Neutral - Tile has no owner and has a letter on it
//						Selected - Tile was neutral but was tapped by a player during turn
//						Endangered - Tile is neighbor of a "Selected" tile(s) and its own
//									 state will be changed if the user inputs a valid word
//						Taken - Tile has an owner and has no words on it, only a pure color

public enum TileState {Empty, Neutral, Selected, Endangered, Taken};
