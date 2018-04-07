using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float speed = 10;
    public float jump_speed = 5;
    public float rotate_speed = 10;
    public float gravity = 9.8f;
    public GameObject attack_foot;

    public State state { get; private set; }
    private Animator animator;
    private CharacterController controller;
    private Quaternion target_quat;
    private Vector3 movement;
    private SphereCollider collider_foot;
    private int anime_state_kick;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        state= new StateIdle(this);
        anime_state_kick = Animator.StringToHash("Base Layer.Kick");

        if (attack_foot != null)
            collider_foot = attack_foot.GetComponent<SphereCollider>();
    }
	
	// Update is called once per frame
	void Update () {
        state.Update();
    }

    private void OnDisable()
    {
        animator.SetFloat("Movement", 0.0f);
        state.ToIdle();
    }

    public void UpdateMove()
    {

        // 移動入力の取得
        Vector2 control = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        control = speed * control.normalized;

        movement.x = control.x;
        movement.z = control.y;
        //movement.Normalize();

        // アニメーターの設定
        animator.SetFloat("Movement", control.sqrMagnitude);

        // 入力値がゼロ以上の場合
        if (control.sqrMagnitude > 0.0f)
        {
            // ステート遷移
            state.ToRun();

            // 目標向きの計算
            target_quat = Quaternion.LookRotation(new Vector3(control.x, 0.0f, control.y));

            // 回転処理
            transform.rotation = Quaternion.Slerp(transform.rotation, target_quat, Time.deltaTime * rotate_speed);

            // 移動処理
            controller.Move(movement * Time.deltaTime);

        }
        else
            // ステート遷移
            state.ToIdle();

    }

    public abstract class State
    {
        public PlayerControl player { get; protected set; }

        public State(PlayerControl player)
        {
            this.player = player;
        }
        
        protected void Set(State state)
        {
            player.state.OnExit();
            state.OnEnter();
            player.state = state;
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void OnExit() { }

        public virtual void ToIdle() { }
        public virtual void ToRun() { }
        public virtual void ToJump() { }
        public virtual void ToKick() { }
    }

    private class StateIdle : State
    {
        public StateIdle(PlayerControl player) : base(player) { }
        public override void Update()
        {
            player.UpdateMove();

            // ジャンプ入力の検出
            if (Input.GetButtonDown("Jump"))
                ToJump();

            // 攻撃入力の検出
            if (Input.GetButtonDown("Fire1"))
                ToKick();
        }
        public override void ToRun()
        {
            Set(new StateRun(player));
        }
        public override void ToJump()
        {
            Set(new StateJump(player));
        }
        public override void ToKick()
        {
            Set(new StateKick(player));
        }
    }

    private class StateKick : State
    {
        public StateKick(PlayerControl player) : base(player) { }
        public override void OnEnter()
        {
            player.animator.SetTrigger("Kick");
            player.collider_foot.enabled = true;
        }
        public override void Update()
        {
        }
        public override void ToIdle()
        {
            Set(new StateIdle(player));
            player.collider_foot.enabled = false;
        }
    }

    private class StateRun : State
    {
        public StateRun(PlayerControl player) : base(player) { }
        public override void Update()
        {
            player.UpdateMove();

            // ジャンプ入力の検出
            if (Input.GetButton("Jump"))
                ToJump();

            // 攻撃入力の検出
            if (Input.GetButtonDown("Fire1"))
                ToKick();

        }
        public override void ToIdle()
        {
            Set(new StateIdle(player));
        }
        public override void ToJump()
        {
            Set(new StateJump(player));
        }
        public override void ToKick()
        {
            Set(new StateKick(player));
        }

    }

    private class StateJump : State
    {
        public StateJump(PlayerControl player) : base(player) { }
        public override void OnEnter()
        {
            player.movement.y = player.jump_speed;
            player.animator.SetTrigger("Jump");
        }
        public override void Update()
        {
            player.movement.y -= player.gravity * Time.deltaTime;

            // 移動入力の取得
            Vector2 control = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            control = player.speed * control.normalized;

            player.movement.x = control.x;
            player.movement.z = control.y;
            //movement.Normalize();

            // アニメーターの設定
            player.animator.SetFloat("Movement", control.sqrMagnitude);

            // 入力値がゼロ以上の場合
            if (control.sqrMagnitude > 0.0f)
            {
                // 目標向きの計算
                player.target_quat = Quaternion.LookRotation(new Vector3(control.x, 0.0f, control.y));

                // 回転処理
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, player.target_quat, Time.deltaTime * player.rotate_speed);

            }
            // 移動処理
            player.controller.Move(player.movement * Time.deltaTime);

            //player.controller.Move(player.movement * Time.deltaTime);

            if (player.movement.y < 0.0f && Physics.Raycast(player.transform.position, Vector3.down, 1.0f))
                player.animator.SetBool("JumpExit", true);
            else
                player.animator.SetBool("JumpExit", false);

            if (player.controller.isGrounded)
                ToIdle();
        }
        public override void ToIdle()
        {
            Set(new StateIdle(player));
        }

    }


}

