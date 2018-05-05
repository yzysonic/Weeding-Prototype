using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<AtkTarget>().event_death += () =>
        {
            GetComponent<Rigidbody>().isKinematic = false;
            var game_ctrl = GameObject.Find("SceneManager").GetComponent<GameControll>();
            game_ctrl.GameOver();
            game_ctrl.gameover_wait_time = 10.0f;
        };
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
