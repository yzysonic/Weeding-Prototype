using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFollowTarget : MonoBehaviour {

    public Transform target;
    public float y_offset = 3.0f;

    private RectTransform rect_transform;
    private Slider slider;

    public float hp_value
    {
        get { return slider.value; }
        set
        {
            if (value >= 0.0f && value <= 1.0f)
                slider.value = value;
        }
    }

    private void Awake()
    {
        rect_transform = GetComponent<RectTransform>();
        slider = GetComponent<Slider>();
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 screen_pos = Camera.main.WorldToScreenPoint(target.position + new Vector3(0.0f, y_offset, 0.0f)); ;
        rect_transform.position = screen_pos;
        if (screen_pos.z >= 0.0f)
            rect_transform.localScale = Vector3.one;
        else
            rect_transform.localScale = Vector3.zero;
    }
}
