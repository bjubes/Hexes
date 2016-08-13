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

	void OnTileClicked(Tile t) {
		if (!selectedTiles.Contains (t) && t.tileState == TileState.Neutral) { //selected new tile
			selectedTiles.Add (t);
			Word += t.letter.ToString ();
			t.SetTileState (TileState.Selected);
			//make tile appear selected


		} else if (t.tileState == TileState.Selected) {// this tile was previously selected, so unselect it 
			
			int index = selectedTiles.IndexOf(t);
			Word = Word.Remove (index, 1);
			selectedTiles.Remove (t);
			t.SetTileState (TileState.Neutral);
		}
	}

	public void OnDeletePressed() {
		throw new NotImplementedException ();	
	}

	public void OnSubmitPressed() {
		//throw new NotImplementedException ();	
		//if (IsWordValid(Word)) { //for now skip validation
			TakeTilesInWord(selectedTiles);
		foreach (Tile t in selectedTiles) {
			t.SetTileState (TileState.Neutral, true);
		}
		//}

		Turn++;
		selectedTiles = new List<Tile> ();
		Word = null;
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
		word = word.ToUpper ();
		long end;
		long beg = 0;
		long length;
		long split;

		FileStream fs = new FileStream (Path.Combine (Application.streamingAssetsPath, "dictionary.txt"), FileMode.Open);
		end = length = fs.Length;
		split = length / 2;

		for (int i = 0; i < 100; i++) {
			
		

			fs.Seek (split, SeekOrigin.Begin);
//			print ((char)fs.ReadByte ());
			StreamReader sr = new StreamReader (fs);
			sr.ReadLine (); //read partial line

			string line = sr.ReadLine ();
			print (line);
			if (line == word) {
				return true;
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
		return false;
	}

	void UpdateTurnIndicator() {
		turnText.text = currTeam.name + "'s Turn!";
		turnText.color = currTeam.color;
	}
}

