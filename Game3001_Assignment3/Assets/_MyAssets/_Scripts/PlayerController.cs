using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float health, moveSpeed, coolDown, turnRate, lifeSpan;
    [SerializeField] GameObject torpedoPrefab;
    bool canFire = true;
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward, -turnRate * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward, turnRate * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.right*moveSpeed*Time.deltaTime);
        }
        if (canFire && Input.GetKey(KeyCode.Space))
        {
            Fire();
        }
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
    void Fire()
    {
        canFire = false;
        Game.Instance.SOMA.PlaySound("Torpedo");
        Invoke("TorpedoReload", coolDown);
        GameObject torpedoInst = Instantiate(torpedoPrefab, transform.position, transform.rotation);
        Destroy(torpedoInst, lifeSpan);
    }

    void TorpedoReload()
    {
        canFire = true;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
    }

}
