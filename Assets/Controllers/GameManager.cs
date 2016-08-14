using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using System.IO;


public class GameManager : MonoBehaviour {
	
	//the starting positons for each team

	public Team[] teams; //set in inspector


	int round;
	int _turn;
	int Turn {
		get {
			return _turn;
		}
		set { 
			if (value !=_turn + 1) {
				Debug.LogWarning ("Turn was incremented by more than one!");
			}
			if (value > teams.Length) {
				round++;
			}
			_turn = value % teams.Length;
			UpdateTurnIndicator ();
		}
	} //which player is currently going as index of teams array.

	public Team currTeam {
		get {
			return teams[Turn];
		}
	}

	string _word;
	public string Word {
		get{ 
			return _word;
		}
		set { 
			_word = value;
			wordText.text = _word;
		}
	}
	public Text wordText;
	public Text turnText;

	List<Tile> selectedTiles = new List<Tile>();

	//grid stuff
	public Grid grid;
	public int width = 6;
	public int  height = 8;

	void OnEnable () {
		grid = new Grid(width,height);
	}

	void Start () {
		Turn = 0;
		foreach (Tile t in Grid.Instance.GetAllTiles()) {
			t.OnTileClicked += OnTileClicked;
		}
		wordText.text = "";
		UpdateTurnIndicator ();

		foreach (Team team in teams) {
			Tile tile = Grid.Instance.GetTile (team.startPosX, team.startPosY);
			tile.team = team;
			tile.SetTileState (TileState.Taken, true);
		}

	}

	//debug
	void Update(){
		if (Input.GetKeyDown(KeyCode.P)) {
			OnSubmitPressed();
		}
	}

	void OnTileClicked(Tile t) {
		if (!selectedTiles.Contains (t) && t.tileState == TileState.Neutral) { //selected new tile
			selectedTiles.Add (t);
			Word += t.letter.ToString ();
			t.SetTileState (TileState.Selected);
			//make tile appear selected


		} else if (t.tileState == TileState.Selected) {// this tile was previously selected, so unselect it 
			
			RemoveTileFromWord (t);
		}
	}

	void RemoveTileFromWord(Tile t) {
		int index = selectedTiles.IndexOf(t);
		Word = Word.Remove (index, 1);
		selectedTiles.Remove (t);
		t.SetTileState (TileState.Neutral);
	}

	public void OnDeletePressed() {
		RemoveTileFromWord (selectedTiles.Last());
	}


	public void OnSubmitPressed() {
		ProcessTurn ();
	}

	public void ProcessTurn(bool pass = false) {
		if (IsWordValid (Word)) { 
			print ("valid turn!");
			TakeTilesInWord (selectedTiles);
			foreach (Tile t in selectedTiles) {
				t.SetTileState (TileState.Neutral, true);
			}
		} 
		if (IsWordValid (Word) || pass) {
			Turn++;
			selectedTiles = new List<Tile> ();
			Word = null;
		}
		else {
			//invalid word and we are not passing this turn
		}
	}

	void TakeTilesInWord (List<Tile> tiles) {
		bool recurse = false;
		List<Tile> takenTiles = new List<Tile> ();;
		print ("working");

		foreach (Tile t in tiles) {
			if (t.IsTouchingTileOfTeam(currTeam)) {
				takenTiles.Add (t);
				recurse = true;
			}
		}

		foreach (Tile t in takenTiles) {

			print ("touching");
			t.team = currTeam;
			t.SetTileState(TileState.Taken);
			tiles.Remove(t);
		}

		if (recurse) {
			TakeTilesInWord(tiles);
		}

		selectedTiles = tiles;
	}

	bool IsWordValid(string word) {
		//return true;
		word = word.ToUpper ();
		long end;
		long beg = 0;
		long length;
		long split;

		FileStream fs = new FileStream (Path.Combine (Application.streamingAssetsPath, "dictionary.txt"), FileMode.Open);
		StreamReader sr = new StreamReader (fs);

		end = length = fs.Length;
		split = length / 2;
		string lastLine = "";

		//the dictionary file has 45333 lines
		while(true) {		

			fs.Seek (split, SeekOrigin.Begin);
			sr = new StreamReader (fs);
			sr.ReadLine (); //read partial line

			string line = sr.ReadLine ();
			//print (line);
			//check for true condition
			if (line == word) {
				fs.Close ();
				sr.Close ();
				return true;
			}

			//check for false condition
			if (line == lastLine) {
				fs.Close ();
				sr.Close ();
				return false;
			} else {
				lastLine = line;
			}

			String[] a = { word, line };
			Array.Sort (a);

			if (a [0] == word) {
				//the word is in between here and the beginning of the file
				end = split;
			} else {
				beg = split;
			}

			split = beg + ((end - beg) / 2);
		}
		fs.Close ();
		sr.Close ();
		return false;
	}

	void UpdateTurnIndicator() {
		turnText.text = currTeam.name + "'s Turn!";
		turnText.color = currTeam.color;
	}
}

