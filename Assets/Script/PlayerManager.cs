using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public Vector2 inputVector;

    [Header("Jump")]
    public float gravity = -30f;
    public Transform spherePosition;
    public LayerMask groundMask;
    public float sphereRadius = 0.4f;
    public float jumpHeight = 0.8f;
    public Vector3 velocity;

    private PlayerInput playerInput;
    private CharacterController charcon;
    private bool onGround;
    


    // Start is called before the first frame update
    void Awake()
    {
        charcon = GetComponent<CharacterController>();

        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        //playerInput.Player.Movement.performed += x => inputVector = x.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        JumpandFall();
        GroundCheck();
        Move();
    }

    private void GroundCheck()
    {
        //groundcheck
        onGround = Physics.CheckSphere(spherePosition.position, sphereRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {
        inputVector = playerInput.Player.Movement.ReadValue<Vector2>();

        Vector3 move = transform.right * inputVector.x + transform.forward * inputVector.y;
        
        charcon.Move((move  * speed * Time.deltaTime) + velocity * Time.deltaTime);
        //Debug.Log(charcon.velocity);
    }

    private void JumpandFall()
    {
        if (onGround)
        {
            if(velocity.y < 0f)
            {
                velocity.y = -6f;
            }

            //jump
            if (playerInput.Player.Jump.IsPressed())
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -6f * gravity);
            }
        }
        //gravity
        velocity.y += gravity * Time.deltaTime;
    }
}
