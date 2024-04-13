using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Vector2 movement;
    Rigidbody2D rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
        RotateTowardsMousePointer();
    }

    private void Move()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));

        if (movement.magnitude > 1)
            movement = movement.normalized;

        rb.AddForce(movement * moveSpeed);
    }

    void RotateTowardsMousePointer()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f ;
        rb.rotation = angle;
    }
}
