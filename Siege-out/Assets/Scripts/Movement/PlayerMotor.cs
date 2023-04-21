using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isSprinted;
    private bool isGrounded;
    public float speed = 5f;
    public float sprintSpeed = 1.25f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    [SerializeField] private float smoothTime = 0.1f;
    private Vector2 smoothInput;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (Input.GetKey(KeyCode.LeftControl) == false && isSprinted == true) { speed /= sprintSpeed; isSprinted = false; }
    }
    public void Sprint()
    {
        isSprinted = true;
        speed *= sprintSpeed;
    }
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime * 2;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}