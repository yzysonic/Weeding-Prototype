using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public float speed = 5.0f;
    public float angle_speed = 3.0f;
    public float y_offset = 3.0f;
    public float max_theta = 80.0f;
    public float min_theta = 10.0f;

    private Transform target;
    private Vector3 offset;
    private Vector3 at;
    private float theta;
    private float phi;
    private float target_theta;
    private float target_phi;
    private float distance;

	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = (transform.position - target.position);
        target_theta = Mathf.Atan2((new Vector2(offset.x, offset.z)).magnitude, offset.y);
        target_phi = Mathf.Atan2(offset.z, offset.x);
        distance = offset.magnitude;
	}
	
	// Update is called once per frame
	void Update () {
        target_theta = Mathf.Clamp(target_theta - Input.GetAxis("Vertical2") * 0.1f, Mathf.Deg2Rad * min_theta, Mathf.Deg2Rad * max_theta);
        target_phi += -Input.GetAxis("Horizontal2") * 0.1f;

        theta = Mathf.Lerp(theta, target_theta, Time.deltaTime * angle_speed);
        phi = Mathf.Lerp(phi, target_phi, Time.deltaTime * angle_speed);
        at = Vector3.Lerp(at, target.position, Time.deltaTime * speed);

        Vector3 pos = new Vector3();
        pos.x = distance * Mathf.Sin(theta) * Mathf.Cos(phi);
        pos.z = distance * Mathf.Sin(theta) * Mathf.Sin(phi);
        pos.y = distance * Mathf.Cos(theta);
        pos += at;

        transform.position = pos;
        transform.LookAt(at + new Vector3(0.0f, y_offset, 0.0f));
    }

    public float GetPhi()
    {
        return phi;
    }
}
