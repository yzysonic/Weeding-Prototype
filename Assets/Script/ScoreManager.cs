using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public GameObject score_ui;

    private Text score_text;
    private int score_value;
    private int score
    {
        get { return score_value; }
        set
        {
            score_value = value;
            score_text.text = value.ToString();
        }
    }

    // Use this for initialization
    private void Awake()
    {
        score_text = score_ui.GetComponent<Text>();
        score = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddScore(int value)
    {
        score += value;
    }

    public int GetScore()
    {
        return score;
    }
}
