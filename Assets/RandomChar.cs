using UnityEngine;
using System.Collections;

public class RandomChar {

	public struct Letter{
		public char l;
		public int freq;

		public Letter(char l, int freq) {
			this.l = l;
			this.freq = freq;
		}
	}

	public RandomChar() {
		InitRandomLettersMap();
	}

	const int total = 98;
	char[] randomMap = new char[total];

	static Letter[] freqChart = new Letter[] {
		new Letter('A',9),
		new Letter('B',2),
		new Letter('C',2),
		new Letter('D',4),
		new Letter('E',12),
		new Letter('F',2),
		new Letter('G',3),
		new Letter('H',2),
		new Letter('I',9),
		new Letter('J',1),
		new Letter('K',1),
		new Letter('L',4),
		new Letter('M',2),
		new Letter('N',6),
		new Letter('O',8),
		new Letter('P',2),
		new Letter('Q',1),
		new Letter('R',6),
		new Letter('S',4),
		new Letter('T',6),
		new Letter('U',4),
		new Letter('V',2),
		new Letter('W',2),
		new Letter('X',1),
		new Letter('Y',2),
		new Letter('Z',1)
	};

	void InitRandomLettersMap() {
		int count = 0;
		string chars = "";
		foreach (Letter letter in freqChart) {
			for (int freq = 0; freq < letter.freq; freq++) {
				chars += letter.l.ToString();
			}
		}
		randomMap = chars.ToCharArray();
	}

	public char GetRandomLetter(){
		return randomMap[Random.Range (0, total)]; 
	}
}
