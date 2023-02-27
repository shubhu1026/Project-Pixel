using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character.Animator;
using System;

public class CharacterController : MonoBehaviour
{
    [SerializeField] internal SimpleCharacterAnimator characterAnimator;
    [SerializeField] LayerMask groundLayer;
    internal Vector2 inputVector;
    
    private StateMachine stateMachine;
    private Rigidbody2D characterRB;
    public bool jumpPressed;    
    private bool isGrounded;
    private void Awake() {
        characterRB = GetComponent<Rigidbody2D>();
        var mover = GetComponent<Mover>();

        stateMachine = new StateMachine();
        var idle = new IdleState(this, mover, GetComponent<Attacker>());
        var move = new MoveState(this, mover);
        var air = new AirState(this, mover);
        stateMachine.AddAnyTransition(idle, IsIdle());
        stateMachine.AddTransition(idle, move, MovingWithInput());
        stateMachine.AddAnyTransition(air, InAir());
        stateMachine.AddTransition(air, move, MovingWithInput());
        Func<bool> IsIdle() => () => isGrounded && inputVector == Vector2.zero && characterRB.velocity.x < 0.01f;
        Func<bool> MovingWithInput() => () => isGrounded && inputVector != Vector2.zero;
        Func<bool> InAir() => () => !isGrounded;
    }
    private void Update() {
        var raycast = Physics2D.Raycast(transform.position + Vector3.up, Vector2.down, 2f, groundLayer);
        isGrounded = raycast;
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        stateMachine.Tick();

    }

}
