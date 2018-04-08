using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    public GameObject waterdrop_prefab;

    private void OnTriggerEnter(Collider other)
    {
        // 水滴のパーティクル生成
        GameObject waterdrop = Instantiate(waterdrop_prefab);
        waterdrop.transform.position = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);

    }
}
