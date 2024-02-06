using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Example : MonoBehaviour
{
    //These InputActionReference's are used to gather movement from the GAMEPAD, rather than like a Input.GetButtonDown.
    [SerializeField] 
    private InputActionReference movementControl;
    [SerializeField] 
    private InputActionReference jumpControl;
    
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField] 
    private float rotationSpeed = 4f;

    private Animator animator;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool hasDoubleJumped = false;
    private Transform cameraMainTransform;
    

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
    }

    void Update()
    {
        // Info to allow Jump
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            hasDoubleJumped = false; // Reset double jump when grounded
        }

        // Basic Movement
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Jump
        if (jumpControl.action.triggered)
        {
            if (groundedPlayer || !hasDoubleJumped)
            {
                if (!groundedPlayer)
                {
                    hasDoubleJumped = true;
                }
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Makes the character move in the direction of the camera
        if (movement != Vector2.zero)
        {
            animator.SetBool("Walk", true);
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            animator.SetBool("Walk", false);
        }
    }
}

//Script derived from Samyam: https://www.youtube.com/watch?v=ImuCx_XVaEQ&list=PLKUARkaoYQT2lJLbQjU6_Uz-A_Qh28Cdj&index=25