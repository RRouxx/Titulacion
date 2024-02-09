using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -40f;
    [SerializeField]
    private float rotationSpeed = 8f;
    [SerializeField]
    private GameObject bulletPrf;
    [SerializeField]
    private Transform barrelTransform;
    [SerializeField]
    private Transform bulletParent;
    [SerializeField]
    private float bulletHitMissDistance = 25f;


    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;


    private void Awake()    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable() {
        shootAction.performed += _ => ShootGun();
    }

    private void OnDisable()    {
        shootAction.performed -= _ => ShootGun();
    }

    void Update()   {
        GroundDetection();
        MovePlayer();
        JumpPlayer();
        PlayerRotate();

    }

    private void ShootGun() {
        RaycastHit hit;
        GameObject bullet = GameObject.Instantiate(bulletPrf, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))    { 
            bulletController.target = hit.point;
            bulletController.hit = true;       
        }
        else   {
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;
            bulletController.hit = false;
        }
    }

    /// <summary>
    /// Usando el isGrounded del CharacterController se verifica si esta tocando el piso.
    /// </summary>
    public void GroundDetection()   {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
    }

    /// <summary>
    /// Se le da el input para que el jugador sepa con que se mueve.
    /// También se le esta dando la fuerza necesaria para que se pueda mover dependiendo de la velocidad.
    /// </summary>
    public void MovePlayer()    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * playerSpeed);

    }

    /// <summary>
    /// Si se presiona el input de espacio y el jugador esta tocando el ground entonces se le da la fuerza necesaria para que de el salto.
    /// </summary>
    public void JumpPlayer()    {
        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Se le esta dando la rotación dependiendo de hacía donde esta viendo la cámara,
    /// usando un Lerp para que sea de manera mas suavizada y no sea instantaneó.
    /// </summary>
    void PlayerRotate() {
        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}