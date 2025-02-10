using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2.5f;
    public float gravity = 20f;
    public bool isGrounded;

    private float verticalRotation = 0f;
    private float cameraRotation = 0f;

    private bool isSprinting = false;
    private bool isCrouching = false;
    private float originalCameraHeight;
    private float originalMoveSpeed;
    private CharacterController characterController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();

        originalCameraHeight = playerCamera.localPosition.y;
        originalMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleSprint();
        HandleCrouch();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraRotation += mouseX;

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, cameraRotation, 0);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        if (isSprinting)
        {
            characterController.SimpleMove(moveDirection.normalized * sprintSpeed);
        }
        else if (isCrouching)
        {
            characterController.SimpleMove(moveDirection.normalized * crouchSpeed);
        }
        else
        {
            characterController.SimpleMove(moveDirection.normalized * moveSpeed);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Vector3 jumpVector = Vector3.up * jumpForce;
            characterController.Move(jumpVector * Time.deltaTime);
        }
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            if (isCrouching)
            {
                isCrouching = false;
                playerCamera.localPosition = new Vector3(0, originalCameraHeight, 0);
                moveSpeed = originalMoveSpeed;
            }
            else
            {
                isCrouching = true;
                playerCamera.localPosition = new Vector3(0, originalCameraHeight / 2f, 0);
                moveSpeed = crouchSpeed;
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void FixedUpdate()
    {
        if (!characterController.isGrounded)
        {
            isGrounded = false;
        }
    }
}
