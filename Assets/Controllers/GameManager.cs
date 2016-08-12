using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	[System.Serializable]
	public struct Team {
		public Color32 color;
		public string name;
	}


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
		print ("Start");
		turn = teams [0];
		foreach (Tile t in Grid.Instance.GetAllTiles()) {
			t.OnTileClicked += OnTileClicked;
		}
		wordText.text = "";
	}

	void OnTileClicked(Tile t) {
		if (!selectedTiles.Contains (t)) { //selected new tile
			selectedTiles.Add (t);
			Word += t.letter.ToString ();
			t.SetTileState (TileState.Selected);
			//make tile appear selected


		} else {// this tile was previously selected, so unselect it 
			
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
}

