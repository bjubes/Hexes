using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile  {

	public int x;
	public int y;

	public Tile (int x, int y) {
		this.x = x;
		this.y = y;
	}

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
}
