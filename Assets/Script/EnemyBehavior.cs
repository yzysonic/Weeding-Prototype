using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

    public int max_hp = 100;
    public int score = 300;
    public float speed = 1.0f;
    public float rotate_speed = 0.05f;
    public float fry_speed = 5.0f;
    public float disappear_time = 1.5f;
    public GameObject hp_ui_perfab;
    public System.Action event_damage;
    public System.Action event_death;

    private GameObject target;
    private Rigidbody rb;
    private SphereCollider sc;
    private Vector3 VecToTarget;
    private UIFollowTarget hp_ui;
    private int hp;
    private float disappear_timer;
    private float ignore_timer;
    private bool ignore_damage;
    private Collider ignore_collider;

    private enum State
    {
        Fry,
        Normal,
        Death
    }
    private State state;

	// Use this for initialization
	void Start () {
        // エネミーの初期化
        target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        state = State.Fry;
        hp = max_hp;
        disappear_timer = 0.0f;
        ignore_damage = false;

        // HPゲージの初期化
        GameObject hp_bar = Instantiate(hp_ui_perfab);
        hp_bar.transform.SetParent(GameObject.FindGameObjectWithTag("UICanvas").transform);

        hp_ui = hp_bar.GetComponent<UIFollowTarget>();
        hp_ui.target = gameObject.transform;
        hp_ui.hp_value = 1.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // 当たり判定を回復
        if (ignore_damage)
        {
            if (ignore_timer >= 0.5f)
            {
                Physics.IgnoreCollision(sc, ignore_collider, false);
                ignore_damage = false;
            }
            ignore_timer += Time.deltaTime;
        }

        // 消失処理（フェイドアウト）
        if (state == State.Death)
        {
            //float opacity = 1.0f - disappear_timer / disappear_time;
            //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].color = new Color(1.0f, 1.0f, 1.0f, opacity);
            //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[1].color = new Color(1.0f, 1.0f, 1.0f, opacity);
            if (disappear_timer >= disappear_time)
                Destroy(gameObject);
            disappear_timer += Time.deltaTime;
        }

	}

    private void FixedUpdate()
    {
        // ターゲットへ移動
        if (target != null && state == State.Normal)
        {
            VecToTarget = (target.transform.position - transform.position).normalized;
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(VecToTarget.x, 0.0f, VecToTarget.z)), rotate_speed));
            rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // 着地処理
        if (collision.gameObject.tag == "Ground" && state == State.Fry)
        {
            sc.isTrigger = true;
            rb.isKinematic = true;
            rb.MoveRotation(Quaternion.LookRotation(new Vector3(VecToTarget.x, 0.0f, VecToTarget.z)));
            state = State.Normal;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに飛ばされた処理
        if (other.gameObject.tag == "PlayerAttack" && state == State.Normal)
        {
            sc.isTrigger = false;
            rb.isKinematic = false;
            rb.AddForce(new Vector3(VecToTarget.x*fry_speed*-7.0f, fry_speed, VecToTarget.z*fry_speed*-7.0f), ForceMode.VelocityChange);
            rb.AddTorque(new Vector3(Random.Range(0.0f, 5000.0f), Random.Range(0.0f, 5000.0f), Random.Range(0.0f, 5000.0f)), ForceMode.VelocityChange);

            // 一時的当たり判定を無効化
            Physics.IgnoreCollision(sc, other);
            ignore_damage = true;
            ignore_collider = other;
            ignore_timer = 0.0f;


            state = State.Fry;
            Damage(100);
        }

    }

    private void OnDestroy()
    {
        Destroy(hp_ui.gameObject);
    }

    private void Death()
    {

        //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetOverrideTag("RenderType", "Transparent");
        ////transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ////transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetInt("_ZWrite", 0);
        //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[1].SetOverrideTag("RenderType", "Transparent");
        ////transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[1].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        ////transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[1].SetInt("_ZWrite", 0);
        state = State.Death;

        if (event_death != null)
            event_death();
    }

    public void Damage(int point)
    {
        if (state == State.Death)
            return;

        hp = Mathf.Max(hp - point, 0);
        hp_ui.hp_value = (float)hp/max_hp;

        if(event_damage != null)
            event_damage();

        if (hp == 0)
            Death();
    }
}
