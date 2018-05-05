using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControll : MonoBehaviour {

    public GameObject game_ui;
    public GameObject result_ui;
    public GameObject gameover_ui;
    public float gameover_wait_time;

    private float timer;
    private bool gameover;
    private int state;

	// Use this for initialization
	void Start () {
        timer = 0.0f;
        state = 0;
        gameover_wait_time = 1.0f;
        gameover = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameover)
        {
            switch(state)
            {
                case 0:
                    if (timer >= gameover_wait_time)
                    {
                        gameover_ui.SetActive(true);
                        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().KillAllEnemy();
                        timer = 0.0f;
                        state = 1;
                    }
                    break;

                case 1:
                    if (timer >= 3.0f || Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Return))
                        state = 2;
                    break;

                case 2:
                    SceneManager.LoadScene("Title");
                    break;

            }

            timer += Time.deltaTime;
        }
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

    public void GameOver()
    {
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().StopAllEnemy();
        game_ui.SetActive(false);
        gameover = true;
    }
}
