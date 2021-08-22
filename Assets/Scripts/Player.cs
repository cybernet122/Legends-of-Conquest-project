using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] float movementSpeed = 5;
    public string transitionName;
    Rigidbody2D playerRigidBody;
    Animator animator;

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
    }
    private void Update()
    {
        float horizontalSpeed = Input.GetAxisRaw("Horizontal");
        float verticalSpeed = Input.GetAxisRaw("Vertical");
        playerRigidBody.velocity = new Vector2(horizontalSpeed * movementSpeed, verticalSpeed * movementSpeed);
        animator.SetFloat("movementX", playerRigidBody.velocity.x);
        animator.SetFloat("movementY", playerRigidBody.velocity.y);
        if(horizontalSpeed == 1 || horizontalSpeed == -1 || verticalSpeed == 1 || verticalSpeed == -1)
        {
            animator.SetFloat("LastY", verticalSpeed);
            animator.SetFloat("LastX", horizontalSpeed);
        }
    }
}
