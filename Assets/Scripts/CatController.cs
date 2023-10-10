using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public int maxJumps = 2; // maksimum �ift z�plama say�s�
    public int jumpsRemaining; // Kullan�labilir �ift z�plama say�s�

    private Rigidbody rb;
    private bool isGrounded;
    private Animator anim;

    private float myMoveSpeed;
    private float horizontalInput;
    public LayerMask layer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        jumpsRemaining = maxJumps;
        myMoveSpeed = moveSpeed;
    }

    void Update()
    {
        Movement();
        Jump();
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // -1 1 
        if (horizontalInput != 0)
        {
            if (horizontalInput > 0)
            {
                anim.SetBool("isBackWalking", false);
                anim.SetBool("isWalking", true);
                moveSpeed = myMoveSpeed;
            }
            else if (horizontalInput < 0)
            {
                anim.SetBool("isBackWalking", true);
                anim.SetBool("isWalking", false);
                moveSpeed = myMoveSpeed / 2;
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isBackWalking", false);
        }
        // Yatay eksende hareket vekt�r�n� olu�turun
        Vector3 movement = new Vector3(0, 0, horizontalInput) * moveSpeed * Time.deltaTime;

        // Hareketi uygulay�n
        transform.Translate(movement);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || jumpsRemaining > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                anim.SetBool("isJumping", true);
                isGrounded = false;
                jumpsRemaining--;
            }
            if (jumpsRemaining==0)
            {
                anim.SetBool("isDoubleJump", true);

            }
            else
            {
                anim.SetBool("isDoubleJump", false);

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        isGrounded = true;
        jumpsRemaining = maxJumps;
        anim.SetBool("isJumping", false);

    }
}