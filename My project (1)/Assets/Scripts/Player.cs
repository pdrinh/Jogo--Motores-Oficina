using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public int health = 3;
    public float speed;
    public float jumpForce;

    public GameObject bow;
    public Transform firePoint;
    
    private bool isJumping;
    private bool doubleJump;
    private bool isFire;
    
    private Rigidbody2D rig;
    private Animator anim;

    private float movement;
    
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        GameController.instance.UpdateLives(health);
    }

    // Update is called once per frame
    void Update()
    {
        BowFire();
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // se nao pressionar nada valor igual é 0. Pressionar direita, valor maximo 1. Esquerda valor maximo -1
        movement = Input.GetAxis("Horizontal");

        //adiciona velocidade ao corpo do personagem no eixo x e y
        rig.velocity = new Vector2(movement * speed, rig.velocity.y);

        // andando pra direita
        if (movement > 0)
        {
            if (!isJumping)
            {
                anim.SetInteger("transition", 1);
            }
            
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        //andando pra esquerda
        if (movement < 0)
        {
            if (!isJumping)
            {
                anim.SetInteger("transition", 1);
            }
            
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (movement == 0 && !isJumping && !isFire)
        {
            anim.SetInteger("transition", 0);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!isJumping)
            {
                anim.SetInteger("transition", 2);
                rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                doubleJump = true;
                isJumping = true;
            }
            else
            {
                if (doubleJump)
                {
                    anim.SetInteger("transition", 2);
                   rig.AddForce(new Vector2(0,jumpForce), ForceMode2D.Impulse);
                   doubleJump = false;
                }
            }
        }
    }

    void BowFire()
    {
        StartCoroutine("Fire");
    }

    IEnumerator Fire()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            isFire = true;
            anim.SetInteger("transition", 3);
            GameObject Bow =Instantiate(bow, firePoint.position, firePoint.rotation);

            if (transform.rotation.y == 0)
            {
                Bow.GetComponent<Flecha>().isRight = true;
            }
            if (transform.rotation.y == 180)
            {
                Bow.GetComponent<Flecha>().isRight = false;
            }
            
            yield return new WaitForSeconds(0.18f);
            isFire = false;
            anim.SetInteger("transition", 0);
        }
    }

    public void Damage(int dmg)
    {
        health -= dmg;
        GameController.instance.UpdateLives(health);
        anim.SetTrigger("hit");
        
        if (transform.rotation.y == 0)
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        if (transform.rotation.y == 180)
        {
            transform.position += new Vector3(1, 0, 0);
        }

        if (health <= 0)
        {
            //chamar game over
            GameController.instance.GameOver();
        }
    }

    public void IncreaseLife(int value)
    {
        health += value;
        GameController.instance.UpdateLives(health);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == 8)
        {
            isJumping = false;
        }
        
        if (coll.gameObject.layer == 9)
        {
            GameController.instance.GameOver();
        }
    }
}
