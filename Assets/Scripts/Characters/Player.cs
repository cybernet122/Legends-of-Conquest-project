using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] float movementSpeed = 5;
    public string transitionName;
    Vector3 bottomLeftEdge, topRightEdge;
    Rigidbody2D playerRigidBody;
    Animator animator;
    PlayerStats player;
    public readonly string playersName = "Jimmy";
    float horizontalSpeed, verticalSpeed;
    public PlayerInput playerInput;
    public static event UnityAction IncreaseHealingPotency;
    public static void UpdatePotency() => IncreaseHealingPotency?.Invoke();

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        playerRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerStats>();
        Invoke("RenamePlayer", 0.2f);
    }

    private void RenamePlayer()
    {
        if (PlayerPrefs.HasKey("Players_name_"))
        {
            string name = PlayerPrefs.GetString("Players_name_");
            gameObject.name = name;
            player.playerName = name;
        }
    }

    public void MovePlayer(float x,float y)
    {
        //Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        horizontalSpeed = x;
        verticalSpeed = y;
    }

    public void FaceDown()
    {
        animator.SetFloat("movementY", -1f);
        animator.SetFloat("LastY", -1f);
    }

    private void Update()
    {
        if (GameManager.instance.enableMovement)
        {
            MovePlayer();
        }
        else
        {
            horizontalSpeed = 0;
            verticalSpeed = 0;
            playerRigidBody.velocity = Vector2.zero;
            animator.SetFloat("movementX", 0);
            animator.SetFloat("movementY", 0);
        }
    }

    private void MovePlayer()
    {
        var velocity = new Vector2(horizontalSpeed * movementSpeed, verticalSpeed * movementSpeed);
        
        playerRigidBody.velocity = velocity;
        animator.SetFloat("movementX", playerRigidBody.velocity.x);
        animator.SetFloat("movementY", playerRigidBody.velocity.y);
        if (horizontalSpeed != 0 || verticalSpeed != 0)
        {
            animator.SetFloat("LastY", verticalSpeed);
            animator.SetFloat("LastX", horizontalSpeed);
        }
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, bottomLeftEdge.x, topRightEdge.x),
            Mathf.Clamp(transform.position.y, bottomLeftEdge.y, topRightEdge.y),
            transform.position.z);
    }

    public void SetLimit(Vector3 bottomEdge, Vector3 topEdge)
    {
        bottomLeftEdge = bottomEdge;
        topRightEdge = topEdge;
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MenuManager.instance.ToggleMenu();
        }
    }

    public PlayerStats ReturnPlayerStats()
    {
        return player;
    }

    public void Levelup()
    {
        IncreaseHealingPotency?.Invoke();
    }
}
