using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour {

    public GameObject score_ui;

    private Text score_text;

    // Use this for initialization
    private void Awake()
    {
        score_text = score_ui.GetComponent<Text>();
    }

    private void OnEnable()
    {
        score_text.text = GameObject.Find("ScoreManager").GetComponent<ScoreManager>().GetScore().ToString();
    }
}
