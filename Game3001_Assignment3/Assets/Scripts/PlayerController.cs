using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0||Input.GetAxisRaw("Vertical")!=0)
        {
            transform.Translate(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized*moveSpeed*Time.deltaTime);
        }

    }
}
