using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public GameObject enemy;
    public GameObject boss;
    public Transform init_enemy_target;
    public Transform swap_center;
    public int swap_max = 30;
    public float swap_interval = 3.0f;
    public float swap_radius = 35.0f;
    public float swap_height = 100.0f;

    private float boss_time;
    private float boss_timer;
    private float swap_timer;
    private int enemy_count;
    private int boss_count;
    private bool stop;
    private List<GameObject> enemy_list;

	// Use this for initialization
	void Start () {
        boss_time = GameObject.Find("TimeManager").GetComponent<TimeManager>().init_time;
        swap_timer = 0.0f;
        boss_timer = 0.0f;
        enemy_count = 0;
        boss_count = 0;
        enemy_list = new List<GameObject>();
        stop = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (stop)
            return;

        if(boss_timer < boss_time)
        {
            if (swap_timer >= swap_interval && enemy_count < swap_max)
            {
                Swap();
                swap_timer = 0.0f;
            }
        }
        else
        {
            if(boss_count < 1)
            {
                GameObject new_boss = GameObject.Instantiate(boss);
                new_boss.transform.position = new Vector3(-15.0f, swap_height, 10.0f);
                new_boss.GetComponent<BossBehaviour>().target = init_enemy_target;
                new_boss.GetComponent<BossBehaviour>().event_death += () =>
                {
                    GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddScore(new_boss.GetComponent<BossBehaviour>().score);
                    GameObject.Find("SceneManager").GetComponent<GameControll>().GameClear();
                };
                boss_count++;
            }
        }

        boss_timer += Time.deltaTime;
        swap_timer += Time.deltaTime;
	}

    void Swap()
    {
        GameObject new_enemy = GameObject.Instantiate(enemy);
        float theta = Random.Range(0.0f, 2 * Mathf.PI);
        new_enemy.transform.position = swap_center.transform.position + new Vector3(swap_radius * Mathf.Sin(theta), swap_height, swap_radius * Mathf.Cos(theta));
        new_enemy.GetComponent<EnemyBehaviour>().target = init_enemy_target;
        new_enemy.GetComponent<EnemyBehaviour>().event_damage += ()=>
        {
            GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddScore(new_enemy.GetComponent<EnemyBehaviour>().score);
        };
        new_enemy.GetComponent<EnemyBehaviour>().event_death += () =>
        {
            enemy_list.Remove(new_enemy);
            enemy_count--;
        };

        enemy_list.Add(new_enemy);
        enemy_count++;
    }

    public void KillAllEnemy()
    {
        if (enemy_count == 0)
            return;

        foreach (var enemy in enemy_list)
        {
            Destroy(enemy);
        }
    }

    public void StopAllEnemy()
    {
        if (enemy_count == 0)
            return;

        foreach (var enemy in enemy_list)
        {
            enemy.GetComponent<EnemyBehaviour>().Stop();
        }
        stop = true;
    }
}
