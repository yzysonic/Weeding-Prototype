using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    public int max_hp = 100;
    public int score = 300;
    public float speed = 1.0f;
    public float rotate_speed = 0.05f;
    public float fry_speed = 5.0f;
    public float disappear_time = 1.5f;
    public float attack_distance = 2.0f;
    public float attack_interval = 1.5f;
    public Transform target;
    public GameObject hp_ui_prefab;
    public System.Action event_damage;
    public System.Action event_death;
    public AudioClip clip_attack;
    public AudioClip clip_step;
    public AudioClip clip_been_kicked;
    public AudioClip clip_falling_into_water;

    private Rigidbody rb;
    private SphereCollider sc;
    private Animator animator;
    private AudioSource audio_souce;
    private Vector3 VecToTarget;
    private UIFollowTarget hp_ui;
    private int hp;
    private float disappear_timer;
    private float ignore_timer;
    private float attack_timer;
    private float step_timer;
    private bool ignore_damage;
    private Collider ignore_collider;

    private enum State
    {
        Fry,
        Move,
        Attack,
        Death
    }
    private State state;

	// Use this for initialization
	void Start () {
        // エネミーの初期化
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        animator = transform.GetChild(0).gameObject.GetComponent<Animator>();
        audio_souce = GetComponent<AudioSource>();
        state = State.Fry;
        hp = max_hp;
        disappear_timer = 0.0f;
        attack_timer = 0.0f;
        step_timer = 0.0f;
        ignore_damage = false;

        // HPゲージの初期化
        GameObject hp_bar = Instantiate(hp_ui_prefab);
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

        switch(state)
        {
            case State.Move:
                if (target == null)
                    break;

                // ターゲットまでのベクトル
                VecToTarget = target.position - transform.position;

                // 攻撃範囲に入ったら攻撃
                float distance = VecToTarget.magnitude;
                if(distance-target.gameObject.GetComponent<AtkTarget>().radius <= attack_distance)
                {
                    ToAttack();
                    break;
                }

                // 移動の実行
                VecToTarget /= distance;
                rb.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(VecToTarget.x, 0.0f, VecToTarget.z)), rotate_speed));
                rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
                if(step_timer >= 0.367f)
                {
                    audio_souce.PlayOneShot(clip_step);
                    step_timer = 0.0f;
                }
                step_timer += Time.deltaTime;

                break;

            case State.Attack:

                // 攻撃範囲から逃げられたら移動
                if ((target.position - transform.position).magnitude - target.gameObject.GetComponent<AtkTarget>().radius > attack_distance+0.1f)
                    ToMove();

                // 攻撃の実行
                if(attack_timer >= attack_interval)
                {
                    AtkTarget atkt = target.gameObject.GetComponent<AtkTarget>();
                    if (atkt.IsDeath())
                        Stop();
                    atkt.Damage(10);
                    animator.SetTrigger("attack");
                    audio_souce.PlayOneShot(clip_attack);
                    attack_timer = 0.0f;
                }

                attack_timer += Time.deltaTime;
                break;

            // 消失処理（フェイドアウト）
            case State.Death:
                //float opacity = 1.0f - disappear_timer / disappear_time;
                //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].color = new Color(1.0f, 1.0f, 1.0f, opacity);
                //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[1].color = new Color(1.0f, 1.0f, 1.0f, opacity);
                if (disappear_timer >= disappear_time)
                    Destroy(gameObject);
                disappear_timer += Time.deltaTime;
                break;
        }

    }

    public void Stop()
    {
        target = null;
        animator.SetBool("move", false);
        state = State.Move;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 着地処理
        if (collision.gameObject.tag == "Ground" && state == State.Fry)
        {
            sc.isTrigger = true;
            rb.isKinematic = true;
            rb.MoveRotation(Quaternion.LookRotation(new Vector3(VecToTarget.x, 0.0f, VecToTarget.z)));
            transform.LookAt(new Vector3(VecToTarget.x, 0.0f, VecToTarget.z));
            rb.MovePosition(new Vector3(transform.position.x, collision.contacts[0].point.y, transform.position.z));
            transform.position = new Vector3(transform.position.x, collision.contacts[0].point.y, transform.position.z);
            ToMove();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(state)
        {
            case State.Move:
            case State.Attack:
                // プレイヤーに飛ばされた処理
                if (other.gameObject.tag == "PlayerAttack")
                {
                    sc.isTrigger = false;
                    rb.isKinematic = false;
                    float angle = GameObject.FindGameObjectWithTag("Player").transform.eulerAngles.y * Mathf.Deg2Rad;
                    rb.AddForce(new Vector3(fry_speed * 7.0f * Mathf.Sin(angle), fry_speed, fry_speed * 7.0f * Mathf.Cos(angle)), ForceMode.VelocityChange);
                    rb.AddTorque(new Vector3(Random.Range(0.0f, 5000.0f), Random.Range(0.0f, 5000.0f), Random.Range(0.0f, 5000.0f)), ForceMode.VelocityChange);
                    rb.MovePosition(transform.position + Vector3.up);

                    // 一時的当たり判定を無効化
                    Physics.IgnoreCollision(sc, other);
                    ignore_damage = true;
                    ignore_collider = other;
                    ignore_timer = 0.0f;

                    target = GameObject.FindGameObjectWithTag("Player").transform;

                    state = State.Fry;
                    Damage(50);

                    audio_souce.PlayOneShot(clip_been_kicked);
                }

                break;

            case State.Fry:
                //if (other.gameObject.tag == "Enemy")
                //{
                //    // 一時的当たり判定を無効化
                //    Physics.IgnoreCollision(sc, other);
                //    ignore_damage = true;
                //    ignore_collider = other;
                //    ignore_timer = 0.0f;

                //    Damage(10);
                //}
                break;

        }

        if(other.gameObject.tag == "Water")
        {
            Damage(max_hp);
            audio_souce.PlayOneShot(clip_falling_into_water);
        }
    }

    private void OnDestroy()
    {
        if(hp_ui != null)
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

    private void ToMove()
    {
        animator.SetBool("move" ,true);
        state = State.Move;
    }

    private void ToAttack()
    {
        attack_timer = 0.0f;
        animator.SetBool("move", false);
        state = State.Attack;
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
