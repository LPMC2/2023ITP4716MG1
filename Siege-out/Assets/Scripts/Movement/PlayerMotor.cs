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
    public float sprintSpeedMultiplier = 1.25f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private float speedMultiplier = 1f;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private AudioClip[] FootstepSounds;
    private float footstepTimer = 0.0f;
    [SerializeField] private float footstepDelay = 0.5f;
    [SerializeField] private AudioClip JumpSound;         
    [SerializeField] private AudioClip LandSound;

    private AudioSource PlayerAudioSource;
    private bool isJumped = false;
    private Vector2 smoothInput;
    // Start is called before the first frame update
    void Start()
    {
        PlayerAudioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
    }
    public void setSpeed(float speedM)
    {
        speedMultiplier = speedM;
        speedMultiplier = speed - (speed * speedMultiplier);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = controller.isGrounded;
        if (Input.GetKey(KeyCode.LeftShift) == false && isSprinted == true) { speed /= sprintSpeedMultiplier; isSprinted = false; }
        if(isGrounded && isJumped == true)
        {
            PlayerAudioSource.clip = LandSound;
            PlayerAudioSource.Play();
            isJumped = false;
        }
    }
    public void Sprint()
    {
        isSprinted = true;
        speed *= sprintSpeedMultiplier;
    }

    public void ProcessMove(Vector2 input)
    {

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

            controller.Move(transform.TransformDirection(moveDirection) * (speed - speedMultiplier) * Time.deltaTime);
        if (moveDirection.magnitude > 0.0f)
        {
            PlayFootStepSound();
        }
        playerVelocity.y += gravity * Time.deltaTime * 2;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private bool isPlayingFootstep = false;
    private void PlayFootStepSound()
    {
        if (!controller.isGrounded)
        {
            return;
        }
   
            int n = Random.Range(1, FootstepSounds.Length);
            PlayerAudioSource.clip = FootstepSounds[n];
            PlayerAudioSource.PlayOneShot(PlayerAudioSource.clip);
            FootstepSounds[n] = FootstepSounds[0];
            FootstepSounds[0] = PlayerAudioSource.clip;

        
    }

    public void Jump()
    {
        if (isGrounded)
        {
            PlayerAudioSource.clip = JumpSound;
            PlayerAudioSource.Play();
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            isJumped = true;
        }
    }
}
