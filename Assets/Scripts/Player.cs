using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRigidBody;
    private void Update()
    {
        float horizontalSpeed = Input.GetAxisRaw("Horizontal");
        float verticalSpeed = Input.GetAxisRaw("Vertical");
        playerRigidBody.velocity = new Vector2(horizontalSpeed * 5, verticalSpeed * 5);
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y, -10);
    }
}
