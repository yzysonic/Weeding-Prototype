using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControll : MonoBehaviour {

    public GameObject game_ui;
    public GameObject result_ui;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GameClear()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>().enabled = false;
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().KillAllEnemy();
        GameObject.Find("EnemyManager").SetActive(false);
        GameObject.Find("TimeManager").SetActive(false);
        game_ui.SetActive(false);
        result_ui.SetActive(true);
    }
}
