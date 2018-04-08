using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public GameObject enemy;
    public Transform init_enemy_target;
    public Transform swap_center;
    public int swap_max = 30;
    public float swap_interval = 3.0f;
    public float swap_radius = 35.0f;
    public float swap_height = 100.0f;

    private float swap_timer;
    private int enemy_count;
    private List<GameObject> enemy_list;

	// Use this for initialization
	void Start () {
        swap_timer = 0.0f;
        enemy_count = 0;
        enemy_list = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if(swap_timer >= swap_interval && enemy_count < swap_max)
        {
            Swap();
            swap_timer = 0.0f;
        }

        swap_timer += Time.deltaTime;
	}

    void Swap()
    {
        GameObject new_enemy = GameObject.Instantiate(enemy);
        float theta = Random.Range(0.0f, 2 * Mathf.PI);
        new_enemy.transform.position = swap_center.transform.position + new Vector3(swap_radius * Mathf.Sin(theta), swap_height, swap_radius * Mathf.Cos(theta));
        new_enemy.GetComponent<EnemyBehavior>().target = init_enemy_target;
        new_enemy.GetComponent<EnemyBehavior>().event_damage += ()=>
        {
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddScore(new_enemy.GetComponent<EnemyBehavior>().score);
        };
        new_enemy.GetComponent<EnemyBehavior>().event_death += () =>
        {
            enemy_list.Remove(new_enemy);
            enemy_count--;
        };

        enemy_list.Add(new_enemy);
        enemy_count++;
    }

    public void KillAllEnemy()
    {
        foreach(var enemy in enemy_list)
        {
            Destroy(enemy);
        }
    }
}
