using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    public GameObject time_ui;
    public int init_time = 61;

    private Text time_text;
    private float timer;
    private int time_value;
    private int time
    {
        get { return time_value; }
        set
        {
            time_value = value;
            time_text.text = value.ToString();
        }
    }

    // Use this for initialization
    private void Awake()
    {
        time_text = time_ui.GetComponent<Text>();
        time = init_time;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (time <= 0)
            return;

        timer += Time.deltaTime;
        time = (int)(init_time - timer);

    }
}
