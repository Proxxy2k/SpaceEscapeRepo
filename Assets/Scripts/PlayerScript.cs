using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    public float forwardSpeed;
    public float moveSpeed;
    public float jumpForce;
    public float gravityModifier;
    public Animator playerAnimator;
    public GameObject playerModel;
    public GameObject deathParticle;
    public GameObject deathPipe;
    public GameObject outerWallPipe;
    public Transform playerCamera;

    //for input
    private bool firstTapBool;

    private float deltaX;
    private Vector3 prevPos;
    private Vector3 gravityDir;
    private Rigidbody rb;
    bool leftRaycol;
    bool rightRaycol;
    bool frontRaycol;
    bool raycastBool = true;
    bool jumpBool;
    bool isGrounded;
    bool isInvincible;
    bool isAlive = true;

    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        ChangeValues();
    }

    void Update()
    {
        #region Controls region

        //uncomment for pc controls

        /*if(Input.GetMouseButtonDown(0))
        {
            prevPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            //input 
            deltaX = Input.mousePosition.x - prevPos.x;
            deltaX = Mathf.Clamp(deltaX, -0.5f, 0.5f);
            transform.Translate(deltaX * Vector3.right * Time.deltaTime * moveSpeed);
        }
        if (deltaX == 0)
        {
            prevPos = Input.mousePosition;
        }*/

        if (Input.touchCount>0)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.position.x<=Screen.width/2)
                {
                    if(firstTapBool==false)
                    {
                        prevPos = touch.position;
                        firstTapBool = true;
                    }
                    deltaX = touch.position.x - prevPos.x;
                    deltaX = Mathf.Clamp(deltaX, -0.5f, 0.5f);
                    transform.Translate(deltaX * Vector3.right * Time.deltaTime * moveSpeed);
                    if(deltaX==0)
                    {
                        prevPos = touch.position;
                    }
                }
            }

        }
        else
        {
            firstTapBool = false;
        }

        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        #endregion

        #region Raycast region
        RaycastHit lefthit;
        leftRaycol = Physics.Raycast(transform.position, -transform.right,out lefthit,0.083f);

        if(leftRaycol&&raycastBool)
        {
            Vector3 initRot = transform.localEulerAngles;

            if(lefthit.transform.CompareTag("Wall"))
            {
                raycastBool = false;
                transform.DORotate(initRot+new Vector3(0, 0, -60), 0.25f).OnComplete(()=> { raycastBool=true; });
                //for not falling while in air
                rb.velocity = Vector3.zero;
            }

        }

        RaycastHit righthit;
        rightRaycol = Physics.Raycast(transform.position, transform.right, out righthit, 0.083f);

        if (rightRaycol&&raycastBool)
        {
            Vector3 initRot = transform.localEulerAngles;

            if (righthit.transform.CompareTag("Wall"))
            {
                raycastBool = false;
                transform.DORotate(initRot+new Vector3(0, 0, 60), 0.25f).OnComplete(() => { raycastBool = true; });
                //for not falling while in air
                rb.velocity = Vector3.zero;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded==true)
        {
            jumpBool = true;
            playerAnimator.SetTrigger("JumpTrigger");
        }

        RaycastHit downhit;
        bool downCol = Physics.Raycast(transform.position, -transform.up, out downhit, 3f);
        if (downCol)
        {
            //to get ground object
            float dist = Vector3.Distance(downhit.point, transform.position);
            gravityDir = downhit.point - transform.position;
            //rb.AddForce(dir.normalized * (1 - dist / gravityModifier) * Time.deltaTime * 300);
            rb.AddForce(-transform.up * (1 - dist / gravityModifier) * Time.deltaTime * 300);

            if(Vector3.Distance(transform.position, downhit.point)<0.09f)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        #endregion

        #region Shooting region

        RaycastHit fronthit;
        frontRaycol = Physics.Raycast(transform.position, transform.forward, out fronthit, 10f);
        if (frontRaycol)
        {
            if(fronthit.transform.CompareTag("Enemy"))
            {
                GunScript.instance.shoot();
            }

            if(fronthit.transform.CompareTag("Door")&&Vector3.Distance(transform.position,fronthit.point)<5f)
            {
                fronthit.transform.GetComponent<Animator>().SetTrigger("DoorOpenTrigger");
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //jumping
        if(jumpBool)
        {
            rb.AddForce(transform.up * jumpForce * Time.deltaTime);
            jumpBool = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("EnemyBullet") &&isAlive && !isInvincible)
        {
            KillPlayer();
        }

        if(collision.transform.CompareTag("Enemy"))
        {
            KillPlayer();
        }
        if(collision.transform.CompareTag("Space"))
        {
            playerAnimator.SetTrigger("KillTrigger");
            GetComponent<PlayerScript>().enabled = false;
            Destroy(deathPipe);
            rb.AddForce(gravityDir.normalized * Time.deltaTime * 800);
            //rotate clockwise or anticlockwise randomly
            int randomiser = Random.Range(0, 2);
            if(randomiser==0)
                transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + Random.Range(100,200)), 4f);
            else
                transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - Random.Range(100, 200)), 4f);

            Destroy(outerWallPipe);
            Invoke("KillPlayer", 1f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Finish"))
        {
            GameManager.instance.ShowFinishMenu();
        }
    }

    public void Jump()
    {
        if (isGrounded == true)
        {
            jumpBool = true;
            playerAnimator.SetTrigger("JumpTrigger");
        }
    }

    void KillPlayer()
    {
        isAlive = false;
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        GameManager.instance.PlaySfx(GameManager.instance.PlayerKill);
        GetComponent<PlayerScript>().enabled = false;
        Destroy(playerModel);
        StartCoroutine(GameManager.instance.RestartScene(2));
    }

    public void ChangeValues()
    {
        forwardSpeed = GameManager.instance.ForwardSpeedSlider.value;
        moveSpeed = GameManager.instance.SideSpeedSlider.value;
        jumpForce = GameManager.instance.JumpSpeedSlider.value;
        isInvincible = GameManager.instance.InvincibiltyToggle.isOn;

        playerCamera.eulerAngles = new Vector3(GameManager.instance.XRotationSlider.value, playerCamera.eulerAngles.y, playerCamera.eulerAngles.z);
        playerCamera.localPosition = new Vector3(0, playerCamera.localPosition.y, GameManager.instance.DistanceSlider.value);
    }
}
