using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    [SerializeField] float moveSpeed;


    void Update()
    {
        transform.Translate(Vector2.right *moveSpeed*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ship"))
        {
            collision.GetComponent<RangedCombatEnemy>().TakeDamage(25);
            Destroy(gameObject);
        }
    }
}
