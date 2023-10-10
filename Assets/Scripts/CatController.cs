using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CatController : MonoBehaviour
{
    private Rigidbody rb;//Karakter rigidbody'si
    private Animator anim; //Karakter Animator'u
    

    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed = 5.0f; //Karakterin H�z�
    private float myMoveSpeed; // Belli durumlarda karakter�n h�z� de�i�ir.Temp de�i�ken

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 5.0f; //z�plama g�c�
    [SerializeField] private int maxJumps = 2; // maksimum z�plama say�s�
    private int jumpsRemaining; // Kalan z�plama say�s�
    private bool isGrounded; //Yerde mi kontrol�

    [Header("Sound Variables")]
    [SerializeField]/**/ AudioSource footStepSound; // Y�r�me sesi
    [SerializeField] private AudioSource jumpSound; // Z�plama sesi
    [SerializeField] private AudioSource collectSound; // Toplama sesi

    [Header("UI Variables")]
    [SerializeField] private TextMeshProUGUI countText; // Ka� adet topland� Texti
    [SerializeField] private Transform finishPoint; //Progress bar i�in finish point
    [SerializeField] private Slider progressBar;  //Progress Bar slider�

    [Header("")]
    [SerializeField] private LayerMask obstacleLayer; //Engel Layer�
    int coinCounter; //Total toplanan coin say�s�
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        jumpsRemaining = maxJumps;
        myMoveSpeed = moveSpeed;
        countText.text = coinCounter.ToString() ;

        float distance = Vector3.Distance(transform.position, finishPoint.transform.position);
        progressBar.maxValue = distance-.5f;

    }

    void Update()
    {
        Movement();
        Jump();
        ProgressBars();
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Debug.DrawRay(transform.position + new Vector3(0, .21f, 0), -Vector3.right, Color.blue);

        if (horizontalInput != 0)
        {
            if (!footStepSound.isPlaying)
            {
                footStepSound.pitch = Random.Range(.95f, 1.05f);
                footStepSound.Play();
            }
            if (horizontalInput > 0)
            {
                anim.SetBool("isBackWalking", false);
                anim.SetBool("isWalking", true);
                moveSpeed = myMoveSpeed;

                RaycastHit hit;
                if (Physics.Raycast(transform.position/*+ new Vector3(0, 0.2f, 0)*/, Vector3.right, out hit, .20f, obstacleLayer))
                    moveSpeed = 0;
            }
            else if (horizontalInput < 0)
            {
                anim.SetBool("isBackWalking", true);
                anim.SetBool("isWalking", false);
                moveSpeed = myMoveSpeed / 2;

                RaycastHit hit;
                if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), -Vector3.right, out hit, .13f, obstacleLayer))
                    moveSpeed = 0;
            }
        }
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isBackWalking", false);

            if (footStepSound.isPlaying)
                footStepSound.Stop();
        }
     

        Vector3 movement = new Vector3(0, 0, horizontalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);


    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if ((isGrounded || jumpsRemaining > 0))
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
                anim.SetBool("isJumping", true);
                isGrounded = false;
                jumpsRemaining--;
                jumpSound.Play();
            }
            if (jumpsRemaining == 0)
                anim.SetBool("isDoubleJump", true);
            else
                anim.SetBool("isDoubleJump", false);
        }
    }

    void ProgressBars()
    {
        float distance = Vector3.Distance(transform.position, finishPoint.transform.position);
        progressBar.value = progressBar.maxValue - distance+.5f ;
        
    }
    private void OnCollisionEnter(Collision collision)
    {

        isGrounded = true;
        jumpsRemaining = maxJumps;
        anim.SetBool("isJumping", false);


    }
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.gameObject.tag == "Coin")
        {
            other.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            other.gameObject.transform.GetChild(0).SetParent(null);
            Destroy(other.gameObject);

            collectSound.Play();
            coinCounter++;
            countText.text = coinCounter.ToString();

        }
    }
}