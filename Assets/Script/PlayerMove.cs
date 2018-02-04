using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    public float speed = 10;
    public float rotate_speed = 10;

    private Quaternion target_quat;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x, 0.0f, z);
        movement.Normalize();

        if (movement.sqrMagnitude > 0.0f)
            target_quat = Quaternion.LookRotation(movement);

        var controller = GetComponent<CharacterController>();
        controller.SimpleMove(movement * speed);

        transform.rotation = Quaternion.Slerp(transform.rotation, target_quat, Time.deltaTime * rotate_speed);

    }

}
