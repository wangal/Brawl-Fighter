using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    int score;
    Text scoreText;

	// Use this for initialization
	void Start () {
        score = 0;
        scoreText = GetComponent<Text>();
        scoreText.text = "Score: " + score;
    }
	
	public void AddScore () {
        score += 1;
        scoreText.text = "Score: " + score;
    }
}
