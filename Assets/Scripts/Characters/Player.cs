using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] float movementSpeed = 5;
    public string transitionName;
    Vector3 bottomLeftEdge, topRightEdge;
    Rigidbody2D playerRigidBody;
    Animator animator;
    public bool enableMovement = true;
    PlayerStats player;

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
    }



    private void Update()
    {
        if (enableMovement)
        {
            float horizontalSpeed = Input.GetAxisRaw("Horizontal");
            float verticalSpeed = Input.GetAxisRaw("Vertical");
            playerRigidBody.velocity = new Vector2(horizontalSpeed * movementSpeed, verticalSpeed * movementSpeed);
            animator.SetFloat("movementX", playerRigidBody.velocity.x);
            animator.SetFloat("movementY", playerRigidBody.velocity.y);
            if (horizontalSpeed == 1 || horizontalSpeed == -1 || verticalSpeed == 1 || verticalSpeed == -1)
            {
                animator.SetFloat("LastY", verticalSpeed);
                animator.SetFloat("LastX", horizontalSpeed);
            }
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, bottomLeftEdge.x, topRightEdge.x),
                Mathf.Clamp(transform.position.y, bottomLeftEdge.y, topRightEdge.y),
                transform.position.z);
        }
        else
        {
            playerRigidBody.velocity = new Vector2(0, 0);
            animator.SetFloat("movementX", 0);
            animator.SetFloat("movementY", 0);
        }
    }

    public void SetLimit(Vector3 bottomEdge, Vector3 topEdge)
    {
        bottomLeftEdge = bottomEdge;
        topRightEdge = topEdge;
    }


    public PlayerStats ReturnPlayerStats()
    {
        return player;
    }
}
