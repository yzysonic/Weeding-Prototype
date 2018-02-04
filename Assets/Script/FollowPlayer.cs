using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public float speed = 5.0f;

    private Transform target;
    private Vector3 offset;
    private float theta;
    private float phi;
    private float distance;

	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = (transform.position - target.position);
        theta = Mathf.Atan2(offset.y, (new Vector2(offset.x, offset.z)).magnitude);
        phi = Mathf.Atan2(offset.x, offset.z);
        distance = offset.magnitude;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);

    }
}
