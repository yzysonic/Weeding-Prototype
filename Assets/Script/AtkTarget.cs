using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkTarget : MonoBehaviour
{
    public int max_hp = 100;
    public float radius = 1.0f;
    public bool show_hp = true;
    public GameObject hp_ui_prefab;
    public AudioClip clip_damage;
    public System.Action<int> event_damage;
    public System.Action event_death;

    private UIFollowTarget hp_ui;
    private int hp;

    public void Start()
    {
        hp = max_hp;

        // HPゲージの初期化
        GameObject hp_bar = Instantiate(hp_ui_prefab);
        hp_bar.transform.SetParent(GameObject.FindGameObjectWithTag("UICanvas").transform);

        hp_ui = hp_bar.GetComponent<UIFollowTarget>();
        hp_ui.target = gameObject.transform;
        hp_ui.hp_value = 1.0f;

        hp_bar.SetActive(show_hp);
    }

    public void Damage(int point)
    {
        if(hp > 0)
        {
            hp = Mathf.Max(hp - point, 0);
            hp_ui.hp_value = (float)hp / max_hp;
            GetComponent<AudioSource>().PlayOneShot(clip_damage);

            if (hp <= 0)
                event_death();
        }

        if(event_damage != null)
            event_damage(point);
        
    }

    public int GetHP()
    {
        return hp;
    }

    public bool IsDeath()
    {
        return hp <= 0;
    }

}