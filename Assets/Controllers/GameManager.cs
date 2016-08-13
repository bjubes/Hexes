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
	Team turn; //which player is currently going

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

	List<Tile> selectedTiles = new List<Tile>();

	void Start () {
		turn = teams [0];
		foreach (Tile t in Grid.Instance.GetAllTiles()) {
			t.OnTileClicked += OnTileClicked;
		}
		wordText.text = "";

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
		throw new NotImplementedException ();	
	}


	bool ValidateWord(string word) {
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
}

