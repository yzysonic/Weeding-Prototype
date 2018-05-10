using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEventrReceiver : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void Step()
    {
        var parent = transform.parent.gameObject;
        parent.GetComponent<AudioSource>().PlayOneShot(parent.GetComponent<BossBehaviour>().clip_step);
    }
}
