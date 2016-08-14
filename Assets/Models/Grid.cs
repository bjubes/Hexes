using UnityEngine;
using System.Collections;

public class Grid {

	public int Width {get; private set;}
	public int Height {get; private set;}

	Tile[,] tiles;

	public static Grid Instance;

	public RandomChar randomChar;

	public Grid (int width, int height) {

		//set the "singleton" grid to ourselves
		if (Instance == null) {
			Instance = this;
		} else {
			Debug.LogError("Another Grid was Constructed without removing the first one!");
		}

		this.Width = width;
		this.Height = height;

		tiles = new Tile[width,height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles[x,y] = new Tile(x,y);
			}
		}
		randomChar = new RandomChar();
		Debug.Log("created grid with " + width*height + " tiles.");
	}

	public Tile GetTile(int x, int y) {
		if (x < 0 || x == Width || y < 0 || y == Height) {
			return null;
		} else {
			return tiles [x, y];
		}
	}

	public Tile[] GetAllTiles() {
		Tile[] t = new Tile[Width*Height];

		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				t[(y*Width) + x] = GetTile(x,y);
			}
		}

		return t;
	}
}
