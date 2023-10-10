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
    [SerializeField] private float moveSpeed = 5.0f; //Karakterin Hýzý
    private float myMoveSpeed; // Belli durumlarda karakterýn hýzý deðiþir.Temp deðiþken

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 5.0f; //zýplama gücü
    [SerializeField] private int maxJumps = 2; // maksimum zýplama sayýsý
    private int jumpsRemaining; // Kalan zýplama sayýsý
    private bool isGrounded; //Yerde mi kontrolü

    [Header("Sound Variables")]
    [SerializeField]/**/ AudioSource footStepSound; // Yürüme sesi
    [SerializeField] private AudioSource jumpSound; // Zýplama sesi
    [SerializeField] private AudioSource collectSound; // Toplama sesi

    [Header("UI Variables")]
    [SerializeField] private TextMeshProUGUI countText; // Kaç adet toplandý Texti
    [SerializeField] private Transform finishPoint; //Progress bar için finish point
    [SerializeField] private Slider progressBar;  //Progress Bar sliderý

    [Header("")]
    [SerializeField] private LayerMask obstacleLayer; //Engel Layerý
    int coinCounter; //Total toplanan coin sayýsý
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