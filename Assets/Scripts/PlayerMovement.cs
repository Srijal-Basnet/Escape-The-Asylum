using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 3f;
    public float lookSpeed = 2f;
    public float jumpForce = 5f;
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchTransitionSpeed = 10f;
    public CharacterController controller;

    [Header("Audio Settings")]
    public AudioSource footstepAudioSource;
    public AudioClip footstepClip;
    public float footstepVolume = 0.5f;

    private float xRotation = 0f;
    private Transform cameraTransform;
    private bool isCrouching = false;
    private float currentSpeed;
    private Vector3 originalCameraPosition;
    private Vector3 crouchedCameraPosition;

    public bool IsCrouching => isCrouching; // Property to check crouching status

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = Camera.main.transform;
        currentSpeed = moveSpeed;
        originalCameraPosition = cameraTransform.localPosition;
        crouchedCameraPosition = new Vector3(
            originalCameraPosition.x,
            originalCameraPosition.y - (standingHeight - crouchHeight) / 2f,
            originalCameraPosition.z
        );

        if (footstepAudioSource != null)
        {
            footstepAudioSource.clip = footstepClip;
            footstepAudioSource.volume = footstepVolume;
            footstepAudioSource.loop = true;
        }
    }

    void Update()
    {
        HandleCrouch();
        MovePlayer();
        RotateCamera();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed = runSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (move.magnitude > 0 && controller.isGrounded)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }

        if (isCrouching)
        {
            controller.height = Mathf.Lerp(controller.height, crouchHeight, Time.deltaTime * crouchTransitionSpeed);
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                crouchedCameraPosition,
                Time.deltaTime * crouchTransitionSpeed
            );
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, standingHeight, Time.deltaTime * crouchTransitionSpeed);
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                originalCameraPosition,
                Time.deltaTime * crouchTransitionSpeed
            );
        }
    }
}





