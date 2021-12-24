﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Stats
    public float playerMaxSpeed = 5;
    [Range(0,100)]
    public int sprintSpeed = 20; ////////////////////// not sure if i need to add it 
    public bool DebugLines = true;
    [Range(0f,1f)]
    public float smoothVariable = 0.1f;

    public bool isPlayerCastSpell = false;

    public float playerSpeed = 0;
    private Vector3 move;
    private Vector3 postMove;

    private bool isPlayerGrounded;
    private float gravityValue = -9.81f;
    private CharacterController controller = null;
    private Vector3 playerVelocity;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        move = new Vector3(0, 0, 0);
        postMove = move;
    }

    private void FixedUpdate()
    {
        playerMaxSpeed = 5;
        if (Input.GetKey(KeyCode.LeftShift)) playerMaxSpeed = 7;
        if(!isPlayerCastSpell) PlayerMove();
    }

    private void PlayerMove()
    {
        isPlayerGrounded = controller.isGrounded;
        if (isPlayerGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector3 smoothMove = Vector3.Lerp(postMove, move, smoothVariable);

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && playerSpeed <= playerMaxSpeed)
        {
            playerSpeed += 0.1f;
            controller.Move(smoothMove * Time.deltaTime * playerSpeed);
            if (DebugLines)
            {
                Debug.DrawRay(transform.position, smoothMove * playerSpeed, Color.red);
            }
        }
        else if (playerSpeed >= 0)
        {
            playerSpeed -= 0.3f;
            controller.Move(postMove * Time.deltaTime * playerSpeed);
            if (DebugLines)
            {
                Debug.DrawRay(transform.position, postMove * playerSpeed, Color.red);
            }
        }

        if (smoothMove != Vector3.zero)
        {
            gameObject.transform.forward = smoothMove;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        postMove = smoothMove;
    }



}
