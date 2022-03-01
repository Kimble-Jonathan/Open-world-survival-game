using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController Controller;
    Animator animator;

    public Transform cam;

    int isWalkingHash;
    int isRunningHash;
    
    Vector3 currentMovementInput;
    
    Vector3 groundedGravity;
    Vector3 gravity;
    
    Vector3 jumpVelocity;
    bool isMovementPressed, isRunPressed,isJumpPressed,hasJumped;
    
    

    public float playerSpeed = 4f;
    float directionY;
    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    

    void Awake()
    {
        playerInput = new PlayerInput();
        Controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    public void getRun(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }
    public void getJump(InputAction.CallbackContext ctx)
    {
        isJumpPressed =  ctx.ReadValueAsButton();
    }
    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        currentMovementInput = ctx.ReadValue<Vector3>();
    }

    // handles movement
    void Move()
    {   
        
        handleAnimation();
        

        bool isGrounded = Controller.isGrounded; 
        gravity.y = 8f;
        jumpVelocity.y = 2.5f;
        
        float horzontal = currentMovementInput.x;
        float vertical = currentMovementInput.z;
        Vector3 direction = new Vector3(horzontal,0f, vertical);
        

        float targetAngle = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg+ cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,ref turnSmoothVelocity,turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f,angle,0f);

        Vector3 moveDirection = Quaternion.Euler(0f,targetAngle,0f) *Vector3.forward;

        
       
        
        Vector3 standingSpeed = new Vector3(0,0,0);
        standingSpeed.y -= gravity.y;
        
        if(isJumpPressed&& isGrounded)
        {
            directionY = jumpVelocity.y;
        }
            directionY -= gravity.y*Time.deltaTime;
            moveDirection.y = directionY;
            standingSpeed.y = directionY;


        Debug.Log(currentMovementInput.magnitude);
        if(direction.magnitude >=0.1f)
        {// walking statement
            Controller.Move(moveDirection*playerSpeed*Time.deltaTime);
        }
        if(direction.magnitude >=0.1f && isRunPressed)
        {// walking statement
            Controller.Move(moveDirection*(playerSpeed*1.5f)*Time.deltaTime);
        }
        if(direction.magnitude ==0f)
        {

            Controller.Move(standingSpeed*playerSpeed*Time.deltaTime);
        }
        
        if(currentMovementInput.magnitude >0)
        {
            isMovementPressed = true;
        }
        else
        {
            isMovementPressed = false;
        }         
        
        
    
       
    }
   void handleAnimation()
   {
       bool isWalking = animator.GetBool(isWalkingHash);
       bool isRunning = animator.GetBool(isRunningHash);

       if(isMovementPressed && !isWalking)
       {
           animator.SetBool("isWalking" ,true);
       }
       else if(!isMovementPressed && isWalking)
       {
           animator.SetBool("isWalking", false);
       }

       if ((isMovementPressed && isRunPressed) && !isRunning)
       {
           animator.SetBool(isRunningHash, true);
       }
       else if (( !isMovementPressed || !isRunPressed) && isRunning)
       {
           animator.SetBool(isRunningHash, false);
       }

   }

 

    

    // Update is called once per frame
    void Update()
    {
        if(Controller.isGrounded)
        {
            Debug.Log("grounded");
        }
        
         Move();
        
    }

    void OnEnable()
    {
    playerInput.Player.Enable();
    }

    void OnDisable()
    {
    playerInput.Player.Disable();
    }
}
