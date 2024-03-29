using Scripts.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input System Controls")]
    [SerializeField] 
    private InputActionReference movementControl;
    [SerializeField] 
    private InputActionReference jumpControl;
    
    [Header("Player Stats")]
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float minSpeed = 1.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField] 
    private float rotationSpeed = 4f;
    public Vector3 playerVelocity;
    [SerializeField]
    private vector3Data lastGroundedPosition;
    
    private Animator animator;
    private CharacterController controller;
    public bool groundedPlayer;
    private bool isJumping;
    private bool isGrounded;
    private bool hasDoubleJumped = false;
    private bool doubleJumpedPurchessed = false;
    public  InventoryItem item;
    private Transform cameraMainTransform;
    [SerializeField]
    private UnityEvent jumpEvent;
    

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
        if (item.UsedOrPurchase)
        {
            doubleJumpedPurchessed = true;
        }
        animator = GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        cameraMainTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Info to allow Jump
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            hasDoubleJumped = false; // Reset double jump when grounded
            animator.SetBool("DoubleJumped", false);
            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            isGrounded = false;
            
        }

        // Basic Movement
        Vector2 movement = movementControl.action.ReadValue<Vector2>(); 
        Vector3 move = new Vector3(movement.x, 0, movement.y); 
        float inputMagnitude = Mathf.Clamp01(move.magnitude);
    
        animator.SetFloat("Input Magnitude", inputMagnitude, 0.5f, Time.deltaTime);
        float speed = Mathf.Lerp(minSpeed, playerSpeed, inputMagnitude);
        move.Normalize();
    
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * move.x;
        move.y = 0f;

        controller.Move( speed * Time.deltaTime * move);
        if (groundedPlayer)
        {
            lastGroundedPosition.value = transform.position;
        }

        // Jump
        if (jumpControl.action.triggered)
        {
            animator.SetBool("IsJumping", true);
            isJumping = true;
            if (item.UsedOrPurchase)
            {
                doubleJumpedPurchessed = true;
            }
            if (groundedPlayer || !hasDoubleJumped && doubleJumpedPurchessed)
            {
                if (!groundedPlayer)
                {
                    hasDoubleJumped = true;
                    animator.SetBool("DoubleJumped", true);
                }
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
        }
        
        if ((isJumping && playerVelocity.y < 0) || playerVelocity.y < -2)
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("DoubleJumped", false);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Makes the character move in the direction of the camera
        if (movement != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
    
    public void DoubleJumpControl()
    {
        doubleJumpedPurchessed = true;
    }
}

//Script derived from Samyam: https://www.youtube.com/watch?v=ImuCx_XVaEQ&list=PLKUARkaoYQT2lJLbQjU6_Uz-A_Qh28Cdj&index=25