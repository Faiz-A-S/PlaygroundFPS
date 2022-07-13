using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public Vector2 inputVector;

    [Header("Jump")]
    public float gravity = -30f;
    public Transform spherePosition;
    public LayerMask groundMask;
    public float sphereRadius = 0.4f;
    public float jumpHeight = 0.8f;
    public Vector3 velocity;

    [Header("Shoot")]
    public int gunDamage = 1;
    public float bulletRate = 0.25f;
    public float bulletSpeed = 1000f;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public int bulletCount;
    public Rigidbody bullet;
    public Transform muzzle;

    private float maxLifeTime = 5f;
    private Camera fpsCam;  
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    private float nextFire;

    private PlayerInput playerInput;
    private CharacterController charcon;
    private bool onGround;
    


    // Start is called before the first frame update
    void Awake()
    {
        charcon = GetComponent<CharacterController>();
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInChildren<Camera>();

        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        //playerInput.Player.Movement.performed += x => inputVector = x.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        JumpandFall();
        GroundCheck();
        Move();
        Shoot();
    }

    private void GroundCheck()
    {
        //groundcheck
        onGround = Physics.CheckSphere(spherePosition.position, sphereRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {
        inputVector = playerInput.Player.Movement.ReadValue<Vector2>();

        Vector3 move = transform.right * inputVector.x + transform.forward * inputVector.y;
        
        charcon.Move((move  * speed * Time.deltaTime) + velocity * Time.deltaTime);
        //Debug.Log(charcon.velocity);
    }

    private void JumpandFall()
    {
        if (onGround)
        {
            if(velocity.y < 0f)
            {
                velocity.y = -6f;
            }

            //jump
            if (playerInput.Player.Jump.IsPressed())
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -6f * gravity);
            }
        }
        //gravity
        velocity.y += gravity * Time.deltaTime;
    }

    private void Shoot()
    {
        //playerInput.Player.Shoot.performed += 
        //     context =>
        //     {
        //         Rigidbody bulletClone = Instantiate(bullet, muzzle.position, Quaternion.identity);
        //         bulletClone.AddForce(transform.forward * bulletSpeed);

        //         Destroy(bulletClone.gameObject, maxLifeTime);
        //     };
        if (playerInput.Player.Shoot.IsPressed() && Time.time > nextFire)
        {
            Debug.Log("Shoot");
            nextFire = Time.time + bulletRate;
            StartCoroutine(ShotEffect());
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit hit;

            // Set the start position for our visual effect for our laser to the position of gunEnd
            laserLine.SetPosition(0, muzzle.position);

            // Check if our raycast has hit anything
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                // Set the end position for our laser line 
                laserLine.SetPosition(1, hit.point);

                // Get a reference to a health script attached to the collider we hit
                EnemyHealth health = hit.collider.GetComponent<EnemyHealth>();

                // If there was a health script attached
                if (health != null)
                {
                    // Call the damage function of that script, passing in our gunDamage variable
                    health.Damage(gunDamage);
                }

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null)
                {
                    // Add force to the rigidbody we hit, in the direction from which it was hit
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
            }
            else
            {
                // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
            }
        }
    }
    private IEnumerator ShotEffect()
    {
        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }
}
