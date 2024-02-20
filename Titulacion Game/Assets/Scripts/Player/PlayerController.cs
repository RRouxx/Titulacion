using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerReferee;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput), typeof(BasicAgent))]

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float sprintSpeed;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -40f;
    [SerializeField]
    private float rotationSpeed = 8f;

    

    [Header("Animations")]
    [SerializeField]
    private float animationSmoothTime  = 0.05f;
    [SerializeField]
    private float animationPlayerTransition = 0.15f;
    [SerializeField]
    private Transform aimTarget;
    [SerializeField]
    private float aimDistance = 10f;

    private Animator animator;

    int jumpAnimation;
    int recoilAnimation;
    int CrouchAnimation;

    int moveXAnimationParameterId;
    int moveZAnimationParameterId;

    Vector2 currentAnimationBlendVector;
    Vector2 animationVelocity;

    [Header("Crouch State")]
    [SerializeField]
    private GameObject HeadPosition;
    [SerializeField]
    private bool Crouch = false;
    [SerializeField]
    private bool canStand;

    [Header("Scripts")]
    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    [Header("Input Actions")]
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    private InputAction CrouchAction;
    private InputAction sprintAction;


    private void Awake()    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        cameraTransform = Camera.main.transform;

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        //shootAction = playerInput.actions["Shoot"];
        CrouchAction = playerInput.actions["Crouch"];
        sprintAction = playerInput.actions["Sprint"];

        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponent<Animator>();
        //jumpAnimation = Animator.StringToHash("Pistol Jump");
        //recoilAnimation = Animator.StringToHash("Pistol Shoot Recoil");
        CrouchAnimation = Animator.StringToHash("Crouching Movement");
        moveXAnimationParameterId = Animator.StringToHash("MoveX");
        moveZAnimationParameterId = Animator.StringToHash("MoveZ");
    }

    private void OnEnable() {
        //shootAction.performed += _ => ShootGun();
        sprintAction.performed += SprintPerformed;
        sprintAction.canceled += SprintCanceled;
    }

    private void OnDisable()    {
        //shootAction.performed -= _ => ShootGun();
        sprintAction.performed -= SprintPerformed;
        sprintAction.canceled -= SprintCanceled;

    }

    void Update()   {
        GroundDetection();
        MovePlayer();
        JumpPlayer();
        PlayerRotate();
        Crouching();
    }

    private void ShootGun() {

        //RaycastHit hit;
        //GameObject bullet = GameObject.Instantiate(bulletPrf, barrelTransform.position, Quaternion.identity, bulletParent);
        //BulletController bulletController = bullet.GetComponent<BulletController>();

        //if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))    { 
        //    bulletController.target = hit.point;
        //    bulletController.hit = true;       
        //}
        //else   {
        //    bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
        //    bulletController.hit = false;
        //}

        //    animator.CrossFade(recoilAnimation, animationPlayerTransition);
    }

    /// <summary>
    /// Usando el isGrounded del CharacterController se verifica si esta tocando el piso.
    /// </summary>
    public void GroundDetection()   {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }
    }

    /// <summary>
    /// Se le da el input para que el jugador sepa con que se mueve.
    /// También se le esta dando la fuerza necesaria para que se pueda mover dependiendo de la velocidad.
    /// </summary>
    public void MovePlayer()    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);

        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;

        controller.Move(move * Time.deltaTime * playerSpeed);
        animator.SetFloat(moveXAnimationParameterId, currentAnimationBlendVector.x);
        animator.SetFloat(moveZAnimationParameterId, currentAnimationBlendVector.y);

    }

    /// <summary>
    /// Si se presiona el input de espacio y el jugador esta tocando el ground entonces se le da la fuerza necesaria para que de el salto.
    /// </summary>
    public void JumpPlayer()    {
        if (jumpAction.triggered && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.CrossFade(jumpAnimation, animationPlayerTransition);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    private void SprintPerformed(InputAction.CallbackContext context)   {
        if (groundedPlayer) {
            playerSpeed = sprintSpeed;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    private void SprintCanceled(InputAction.CallbackContext context)    {
        playerSpeed = walkSpeed; // O playerSpeed = crouchSpeed si prefieres
    }

    /// <summary>
    /// Se le esta dando la rotación dependiendo de hacía donde esta viendo la cámara,
    /// usando un Lerp para que sea de manera mas suavizada y no sea instantaneó.
    /// </summary>
    void PlayerRotate() {
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        aimTarget.position = cameraTransform.position + cameraTransform.forward * aimDistance;
    }


    /// <summary>
    /// Verificacion si puede o no agacharse
    /// </summary>
    void Crouching()    {

        if (Physics.Raycast(HeadPosition.transform.position, Vector3.up, .5f)) { 
            canStand = false;
        }
        else{
            canStand = true;
        }

        if (CrouchAction.triggered) {
            if (Crouch == true && canStand == true) {
                Crouch = false;
                playerSpeed = walkSpeed;
                animator.SetBool("Crouch", false);
                controller.height = 2.76f;
                controller.center = new Vector3(0f, 1f, 0f);
                animator.CrossFade(CrouchAnimation, animationPlayerTransition);

            }
            else{
                Crouch = true;
                playerSpeed = crouchSpeed;
                controller.height = 2f;
                controller.center = new Vector3(0f, .9f, 0f);
                animator.SetBool("Crouch", true);
            }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        //Bullet bullet = other.GetComponent<Bullet>();
        //if (bullet != null)
        //{
        //    if ((bullet.bulletTeam == Bullet.BulletTeam.Blue && this.CompareTag("PlayerRed")) ||
        //        (bullet.bulletTeam == Bullet.BulletTeam.Red && this.CompareTag("Player")))
        //    {
        //        fltLife -= 10f;
        //    }
        //}
    }
}