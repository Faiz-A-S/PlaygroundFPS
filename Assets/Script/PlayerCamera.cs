using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector2 sensitivity;
    public Vector2 inputVector;
    public Transform player;
    public float minC = -90f;
    public float maxC = 90f;
    public Transform weapon;

    private PlayerInput playerInput;
    private float inputVectorY;
    private float inputVectorX;

    public float weaponRange = 50f;

    private Camera fpsCam;

    // Start is called before the first frame update
    void Awake()
    {
        fpsCam = GetComponentInParent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
    }

    void Update()
    {
        // Create a vector at the center of our camera's viewport
        Vector3 lineOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        // Draw a line in the Scene View  from the point lineOrigin in the direction of fpsCam.transform.forward * weaponRange, using the color green
        Debug.DrawRay(lineOrigin, fpsCam.transform.forward * weaponRange, Color.green);
    }

    void LateUpdate()
    {       
        inputVector = playerInput.Player.Look.ReadValue<Vector2>();
        inputVectorX = (inputVector.x * sensitivity.x * 1.0f);
        inputVectorY += (inputVector.y * sensitivity.y * 1.0f);

        //velocity = new Vector2(Mathf.MoveTowards(velocity.x, inputVectorX, acceleration.x * Time.deltaTime), 
        //    Mathf.MoveTowards(velocity.y, inputVectorY, acceleration.y * Time.deltaTime));

        inputVectorY = Mathf.Clamp(inputVectorY, minC, maxC);
        //transform.localRotation = Quaternion.Euler(inputVectorY, 0f, 0f);
        transform.localEulerAngles = new Vector3(inputVectorY, 0, 0);
        //player.Rotate(Vector3.up * inputVectorX );
        player.Rotate(new Vector3(0f, (inputVectorX), 0f), Space.Self);
    }
}
